using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Cyrus.WsApi.Core.Utilities
{
    public class SoapUtil
    {
        /// <summary>
        /// 响应Soap请求
        /// </summary>
        /// <param name="httpContext">请求上下文</param>
        /// <param name="result">响应结果</param>
        /// <param name="resultName">响应参数名</param>
        public static void CreateSoapResponse(HttpContext httpContext, object result, string resultName = "")
        {
            Message responseMessage;
            var encoder = new BasicHttpBinding().CreateBindingElements().Find<MessageEncodingBindingElement>()?.CreateMessageEncoderFactory().Encoder;
            var actionName = httpContext.Request.Headers["SOAPAction"].FirstOrDefault()?.Trim('"').Split('/').LastOrDefault();
            var bodyWriter = new ServiceBodyWriter("http://tempuri.org/", actionName + "Response", string.IsNullOrEmpty(resultName) ? (actionName + "Result") : resultName, result);
            responseMessage = Message.CreateMessage(encoder.MessageVersion, null, bodyWriter);
            httpContext.Response.ContentType = httpContext.Request.ContentType;
            httpContext.Response.Headers["SOAPAction"] = responseMessage.Headers.Action;
            encoder.WriteMessage(responseMessage, httpContext.Response.Body);
        }

        /// <summary>
        /// 请求Soap服务
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="soapXml">body</param>
        /// <returns></returns>
        public static async Task<string> SoapRequestAsync(string url, string soapXml, double timespanSeconds = 30)
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(timespanSeconds) };
            HttpContent httpContent = new StringContent(soapXml, Encoding.UTF8);
            httpContent.Headers.ContentType.CharSet = "UTF-8";
            httpContent.Headers.ContentType.MediaType = "text/xml";
            httpContent.Headers.Add("SOAPAction", url);
            var response = await client.PostAsync(url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 请求Soap服务
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="soapAction">方法名</param>
        /// <param name="soapXml">body</param>
        /// <returns></returns>
        public static async Task<string> SoapRequestAsync(string url, string soapAction, string soapXml, double timespanSeconds = 30)
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(timespanSeconds) };
            HttpContent httpContent = new StringContent(soapXml, Encoding.UTF8);
            httpContent.Headers.ContentType.CharSet = "UTF-8";
            httpContent.Headers.ContentType.MediaType = "text/xml";
            httpContent.Headers.Add("SOAPAction", soapAction);
            var response = await client.PostAsync(url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 请求Soap服务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="soapDocument"></param>
        /// <param name="timespanSeconds"></param>
        /// <returns></returns>
        public static async Task<SoapDocument> SoapRequestAsync(string url, SoapDocument soapDocument, double timespanSeconds = 30)
        {
            var rsp = await SoapRequestAsync(url, soapDocument.Action.LocalName, soapDocument.ToString());
            return SoapDocument.FromString(rsp);
        }
    }

    public class ServiceBodyWriter : BodyWriter
    {
        string ServiceNamespace;
        string EnvelopeName;
        string ResultName;
        object Result;

        public ServiceBodyWriter(string serviceNamespace, string envelopeName, string resultName, object result) : base(isBuffered: true)
        {
            ServiceNamespace = serviceNamespace;
            EnvelopeName = envelopeName;
            ResultName = resultName;
            Result = result;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement(EnvelopeName, ServiceNamespace);
            var serializer = new DataContractSerializer(Result.GetType(), ResultName, ServiceNamespace);
            serializer.WriteObject(writer, Result);
            writer.WriteEndElement();
        }
    }

    public class SoapDocument
    {
        public SoapDocument()
        {
            Namespaces = new List<SoapElement>
            {
                new SoapElement
                {
                    Name = "xmlns:soapenv",
                    Value = "http://schemas.xmlsoap.org/soap/envelope/"
                }
            };
        }
        public SoapElement Action { get; set; }
        public List<SoapElement> @Args { get; set; }
        public List<SoapElement> Namespaces { get; set; }
        public override string ToString()
        {
            StringBuilder args = new StringBuilder();
            foreach (var arg in Args)
            {
                args.AppendFormat(@"            <{0}{1}><![CDATA[{2}]]></{0}>{3}"
                    , arg.Name
                    , !arg.HasAttrs ? string.Empty : (" " + string.Join(" ", arg.Attrs.Select(o => o.Name + "=\"" + o.Value + "\"")))
                    , arg.Value
                    , Environment.NewLine);
            }
            string action = string.Format(@"<{0}{1}>{3}{2}        </{0}>"
                    , Action.Name
                    , !Action.HasAttrs ? string.Empty : (" " + string.Join(" ", Action.Attrs.Select(o => o.Name + "=\"" + o.Value + "\"")))
                    , args.ToString()
                    , Environment.NewLine);
            string nameSpance = string.Join(" ", Namespaces.Select(o => o.Name + "=\"" + o.Value + "\""));
            var soapPrefix = Namespaces.FirstOrDefault(o => o.Value.Equals("http://schemas.xmlsoap.org/soap/envelope/", StringComparison.OrdinalIgnoreCase))?.LocalName;
            return $@"<{soapPrefix}:Envelope {nameSpance}>{Environment.NewLine}    <{soapPrefix}:Header/>{Environment.NewLine}    <{soapPrefix}:Body>{Environment.NewLine}        {action}{Environment.NewLine}    </{soapPrefix}:Body>{Environment.NewLine}</{soapPrefix}:Envelope>";
        }
        public static SoapDocument FromString(string soapString)
        {
            if (string.IsNullOrWhiteSpace(soapString)) throw new ArgumentNullException();
            var soapDocument = new SoapDocument();
            XDocument xmlDocument = XDocument.Parse(soapString);
            soapDocument.Namespaces = xmlDocument.Root.Attributes()?.Select(o => new SoapElement { Name = GetXElementFullName(o.Document.Root), Value = o.Value }).ToList();
            XElement bodyElement = xmlDocument.Root.Elements().FirstOrDefault(o => !string.IsNullOrEmpty(o.Name.NamespaceName) && o.Name.LocalName == "Body");
            XElement actionElement = bodyElement.Elements().FirstOrDefault();
            soapDocument.Action = new SoapElement { Name = GetXElementFullName(actionElement) };
            soapDocument.Action.Attrs = actionElement.Attributes()?.Select(o => new SoapElement { Name = GetXElementFullName(o), Value = o.Value }).ToList();
            soapDocument.Args = actionElement.Elements()?.Select(o =>
            {
                return new SoapElement
                {
                    Name = GetXElementFullName(o),
                    Value = o.Value,
                    Attrs = o.Attributes()?.Select(e => new SoapElement { Name = e.Name.LocalName, Value = e.Value }).ToList()
                };
            }).ToList();
            return soapDocument;
        }

        private static string GetXElementFullName(XElement element)
        {
            string localName = element.Name.LocalName;
            string prefixName = element.GetPrefixOfNamespace(element.Name.Namespace);
            return (string.IsNullOrEmpty(prefixName) ? "" : (prefixName + ":")) + localName;
        }

        private static string GetXElementFullName(XAttribute attribute)
        {
            string localName = attribute.Name.LocalName;
            string prefixName = attribute.Document.Root.GetPrefixOfNamespace(attribute.Name.Namespace);
            return (string.IsNullOrEmpty(prefixName) ? "" : (prefixName + ":")) + localName;
        }
    }

    public class SoapElement
    {
        public SoapElement() { }
        public SoapElement(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Prefix
        {
            get
            {
                var nameSplit = Name?.Split(':');
                if (nameSplit != null && nameSplit.Length == 2)
                {
                    return nameSplit[0];
                }
                return null;
            }
        }
        public string LocalName { get { return Name.Split(':').LastOrDefault(); } }
        public List<SoapElement> Attrs { get; set; }
        public bool HasAttrs { get { return Attrs != null && Attrs.Count > 0; } }
    }
}
