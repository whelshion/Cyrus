using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cyrus.WsApi.Core.Models;
using Cyrus.WsApi.Core.Models.Hebei;
using Cyrus.WsApi.Core.Utilities;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Cyrus.WsApi.Core.Controllers.Hebei
{
    [Produces("text/xml")]
    [Route("api/ROSASTradeService")]
    public class ESBController : SoapBaseController
    {
        private AppSettings AppSettings { get; }

        public ESBController(AppSettings appSettings)
        {
            AppSettings = appSettings;
        }

        public async Task Entry()
        {
            try
            {
                Logger.Info("[HBESBService]入口:--");
                byte[] buffer = new byte[Request.ContentLength.GetValueOrDefault()];
                await Request.Body.ReadAsync(buffer, 0, buffer.Length);
                string reqStr = Encoding.UTF8.GetString(buffer);

                Logger.Info($"[HBESBService]请求信息:--{Environment.NewLine}{reqStr}");
                if (HttpContext.Request.Method.Contains("GET", StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrWhiteSpace(reqStr))
                {
                    await Task.Run(() =>
                    {
                        var strWsdl = System.IO.File.ReadAllBytes("wwwroot/wsdl/ROSASTradeService.wsdl");
                        Response.ContentType = "text/xml";
                        Response.Body.WriteAsync(strWsdl, 0, strWsdl.Length);
                    });
                    return;
                }
                var soapDocument = SoapDocument.FromString(reqStr);
                string actionName = soapDocument.Action.LocalName;
                string soapAction = HttpContext.Request.Headers["SOAPAction"].FirstOrDefault() ?? "";
                HttpContext.Request.Headers["SOAPAction"] = actionName;
                Logger.Info($"[HBESBService]请求信息:--{Environment.NewLine}{soapDocument.ToString()}");
                Logger.Info($"[HBESBService]SOAPAction:--{soapAction}");
                Logger.Info($"[HBESBService]ActionName:--{actionName}");

                MethodInfo method = GetType().GetMethod(actionName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (method == null) method = GetType().GetMethod($"{actionName}Async", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (method == null) throw new NotImplementedException($"[SOAPAction:{actionName}]未实现");
                if (method.ReturnType == typeof(Task)) await (method.Invoke(this, new object[] { soapDocument }) as Task);
                else method.Invoke(this, new object[] { soapDocument });
            }
            catch (Exception ex)
            {
                Logger.Error($"[HBESBService]异常:--{ex.Message}", ex);
                await ErrorRequestAsync(null, ex.Message);
            }
        }

        private async Task ROSASTradeAsync(SoapDocument soapDocument)
        {
            string message = string.Empty;
            string xmlStr = soapDocument.Args.FirstOrDefault(o => o.LocalName == "serviceParas")?.Value;
            try
            {
                var xmlServiceParas = XmlConvert.DeserializeObject<HebeiESBServiceParas>(xmlStr);

                if (xmlServiceParas == null || xmlServiceParas.ProvinceData.Count() == 0)
                    throw new Exception("解析serviceParas失败");

                bool isSuccess = true;
                var ftpConfig = AppSettings.FtpSection.FirstOrDefault(o => o.Provider == "HebeiESB");
                foreach (var p in xmlServiceParas.ProvinceData)
                {
                    if (string.IsNullOrWhiteSpace(p.Path))
                        continue;
                    var success = await FtpUtil.Download(new FtpConfig
                    {
                        Host = p.Ip,
                        UserName = p.User,
                        Password = p.Password,
                        Port = 21,
                        RemotePath = p.Path,
                        BaseDirectory = ftpConfig.BaseDirectory,
                        LocalDirectory = ftpConfig.LocalDirectory,
                        RenameFormat = ftpConfig.RenameFormat,
                        CategoryMaps = ftpConfig.CategoryMaps
                    });
                    isSuccess &= success;
                    string successString = success ? "成功" : "失败";
                    message += string.Format($"下载{successString}：ftp://{p.User}:{p.Password}@{p.Ip}{p.Path}", p, successString);
                }

                if (!isSuccess) await ErrorRequestAsync(null, message);
                else await SuccessRequestAsync(null, message: message);
            }
            catch (Exception ex)
            {
                Logger.Error($"[HBESBService]ROSASTrade异常:--{ex.Message}", ex);
                await ErrorRequestAsync(null, ex.Message);
            }
        }

        /// <summary>
        /// 错误请求处理
        /// </summary>
        /// <param name="requestObj">请求对象</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        private async Task ErrorRequestAsync(object requestObj, string errMsg)
        {
            errMsg = errMsg ?? new StringBuilder().ToString();
            //if (errMsg.Length > 64) errMsg = errMsg.Substring(0, 64);
            await Task.Run(() =>
            {
                var rspHead = new HebeiESBResponseHead()
                {
                    Result = 1,
                    ResultInfo = errMsg,
                    DataType = "none"
                };
                var rspObj = new HebeiESBResponse<HebeiESBResponseHead, string> { Head = rspHead, Data = string.Empty };
                var rspXml = XmlConvert.SerializeObject(rspObj, new UTF8Encoding(false));
                Logger.Warn($"[HBESBService]错误请求:--{Environment.NewLine}{rspXml}");
                rspXml = rspXml.RegexReplace("\\s+\\w+:\\w+=\".*\"", "");
                SoapUtil.CreateSoapResponse(HttpContext, rspXml, "Response");
            });
        }

        /// <summary>
        /// 成功请求处理
        /// </summary>
        /// <param name="requestObj">请求对象</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        private async Task SuccessRequestAsync(object requestObj, int code = 0, string message = "执行任务成功", string resultData = "")
        {
            //if (message.Length > 64) message = message.Substring(0, 64);
            await Task.Run(() =>
            {
                var rspHead = new HebeiESBResponseHead()
                {
                    Result = code,
                    ResultInfo = message,
                    DataType = "none"
                };
                var rspObj = new HebeiESBResponse<HebeiESBResponseHead, string> { Head = rspHead, Data = resultData };
                var rspXml = XmlConvert.SerializeObject(rspObj, new UTF8Encoding(false));
                Logger.Warn($"[HBESBService]处理成功:--{Environment.NewLine}{rspXml}");
                rspXml = rspXml.RegexReplace("\\s+\\w+:\\w+=\".*\"", "");
                SoapUtil.CreateSoapResponse(HttpContext, rspXml, "Response");
            });
        }
    }
}