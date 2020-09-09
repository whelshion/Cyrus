using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Cyrus.WsApi.Core.Utilities
{
    public class XmlConvert
    {
        /// <summary>
        /// 序列化对象为Xml字符串
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="obj">实例对象</param>
        /// <param name="nsEmpty">清空根节点命名空间</param>
        /// <param name="placeHolder">占位符:序列化后替换为空字符串</param>
        /// <param name="charset">编码</param>
        /// <returns>Xml字符串</returns>
        public static string SerializeObject<T>(T obj, bool nsEmpty = false, string placeHolder = "", string charset = "utf-8")
        {
            Encoding encoding = Encoding.GetEncoding(charset);
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings setting = new XmlWriterSettings { Encoding = encoding, Indent = true };
            using (var writer = XmlWriter.Create(stream, setting))
            {
                var serializer = new XmlSerializer(typeof(T));
                if (nsEmpty)
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    serializer.Serialize(writer, obj, ns);
                }
                else serializer.Serialize(writer, obj);
            }
            string xmlStr = encoding.GetString(stream.ToArray());
            if (!string.IsNullOrEmpty(placeHolder)) xmlStr = xmlStr.Replace(placeHolder, "");
            return xmlStr;
        }

        /// <summary>
        /// 序列化对象为Xml字符串
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="obj">实例对象</param>
        /// <param name="encoding">编码</param>
        /// <param name="nsEmpty">清空根节点命名空间</param>
        /// <param name="placeHolder">占位符:序列化后替换为空字符串</param>
        /// <returns></returns>
        public static string SerializeObject<T>(T obj, Encoding encoding, bool nsEmpty = false, string placeHolder = "")
        {
            if (encoding == null)
                encoding = new UTF8Encoding(false);
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings setting = new XmlWriterSettings { Encoding = encoding, Indent = true };
            using (var writer = XmlWriter.Create(stream, setting))
            {
                var serializer = new XmlSerializer(typeof(T));
                if (nsEmpty)
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    serializer.Serialize(writer, obj, ns);
                }
                else serializer.Serialize(writer, obj);
            }
            string xmlStr = encoding.GetString(stream.ToArray());
            if (!string.IsNullOrEmpty(placeHolder)) xmlStr = xmlStr.Replace(placeHolder, "");
            return xmlStr;
        }

        /// <summary>
        /// 反序列化Xml字符串
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="xml">xml字符串</param>
        /// <returns>T的实例</returns>
        public static T DeserializeObject<T>(string xml)
             where T : new()
        {
            using (var stringReader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
        }
    }
}
