using Cyrus.WsApi.Core.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyrus.WsApi.Core.Controllers
{
    public abstract class SoapBaseController : BaseController
    {
        /// <summary>
        /// 响应字符串
        /// </summary>
        /// <param name="rspRusult"></param>
        /// <returns></returns>
        protected virtual async Task ResponseAsync(string rspRusult)
        {
            await Task.Run(() =>
            {
                Logger.Warn($"[{GetType().Name}]处理结果:--{Environment.NewLine}{rspRusult}");
                SoapUtil.CreateSoapResponse(HttpContext, rspRusult);
            });
        }

        /// <summary>
        /// 响应JSON
        /// kvs必须成对出现
        /// </summary>
        /// <param name="kvs"></param>
        /// <returns></returns>
        protected virtual async Task ResponseJsonAsync(params object[] kvs)
        {
            await Task.Run(async () =>
            {
                Dictionary<string, object> rspObj = new Dictionary<string, object>();
                for (int i = 0; i < kvs.Length; i++)
                    rspObj.Add(kvs[i].ToString(), kvs[++i]);
                await ResponseJsonAsync(rspObj);
            });
        }

        /// <summary>
        /// 响应JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rspObj"></param>
        /// <returns></returns>
        protected virtual async Task ResponseJsonAsync<T>(T rspObj) where T : class
        {
            await Task.Run(() =>
            {
                var rspJson = JsonConvert.SerializeObject(rspObj);
                Logger.Warn($"[{GetType().Name}]处理结果:--{Environment.NewLine}{rspJson}");
                SoapUtil.CreateSoapResponse(HttpContext, rspJson);
            });
        }

        /// <summary>
        /// 响应XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rspObj"></param>
        /// <returns></returns>
        protected virtual async Task ResponseXmlAsync<T>(T rspObj) where T : class
        {
            await Task.Run(() =>
            {
                var rspXml = XmlConvert.SerializeObject(rspObj, new UTF8Encoding(false), true);
                Logger.Warn($"[{GetType().Name}]处理结果:--{Environment.NewLine}{rspXml}");
                rspXml = rspXml.RegexReplace("\\s+\\w+:\\w+=\".*\"", "");
                SoapUtil.CreateSoapResponse(HttpContext, rspXml);
            });
        }
    }
}
