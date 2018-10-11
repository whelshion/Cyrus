using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cyrus.WsApi.Core.Models.Hebei
{
    [Serializable]
    [XmlRoot("request", Namespace = "")]
    public class HebeiESBRequest<THead, TBody>
    {
        [XmlElement("head")]
        public THead Head { get; set; }
        [XmlElement("data")]
        public TBody Data { get; set; }
    }

    [Serializable]
    [XmlRoot("response", Namespace = "")]
    public class HebeiESBResponse<THead, TBody>
    {
        [XmlElement("head")]
        public THead Head { get; set; }
        [XmlElement("data")]
        public TBody Data { get; set; }
    }

    [Serializable]
    [XmlType("data", Namespace = "")]
    public class HebeiESBResponseHead
    {
        [XmlElement("Result")]
        public int Result { get; set; }
        [XmlElement("ResultInfo")]
        public string ResultInfo { get; set; }
        [XmlElement("DataType")]
        public string DataType { get; set; }
    }

    [Serializable]
    [XmlRoot("WholeMsg")]
    public class HebeiESBServiceParas
    {
        [XmlType("province")]
        public class Province
        {
            [XmlElement("ip")]
            public string Ip { get; set; }
            [XmlElement("user")]
            public string User { get; set; }
            [XmlElement("password")]
            public string Password { get; set; }
            [XmlElement("path")]
            public string Path { get; set; }
        }

        [XmlElement("province")]
        public Province[] ProvinceData { get; set; }
    }
}
