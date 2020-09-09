using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cyrus.WsApi.Core.Models
{
    [Serializable]
    public class AppSettings
    {
        public List<FtpConfig> FtpSection { get; set; }
        public ConnectionSection ConnectionStrings { get; set; }
    }

    public class ConnectionSection
    {
        /// <summary>
        /// 默认连接
        /// </summary>
        public string Default { get; set; }
    }

    public class FtpConfig
    {
        /// <summary>
        /// 服务提供方
        /// </summary>
        public string Provider { get; set; }
        /// <summary>
        /// 服务主机IP
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 服务发布端口
        /// </summary>
        public int Port { get; set; } = 21;
        /// <summary>
        /// 登录名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 保存目录
        /// </summary>
        public string LocalDirectory { get; set; }
        /// <summary>
        /// 默认工作目录
        /// </summary>
        public string BaseDirectory { get; set; } = "/";
        /// <summary>
        /// 远程文件目录
        /// </summary>
        public string RemotePath { get; set; }
        /// <summary>
        /// 重命名规则
        /// </summary>
        public string RenameFormat { get; set; }
        /// <summary>
        /// 分类保存规则列表
        /// </summary>
        public List<CategoryMap> CategoryMaps { get; set; }
    }

    public class CategoryMap
    {
        /// <summary>
        /// 匹配字符串
        /// </summary>
        public string MatchName { get; set; }
        /// <summary>
        /// 匹配成功保存到子目录
        /// </summary>
        public string SubDirectory { get; set; }
        /// <summary>
        /// 是否是正则匹配
        /// </summary>
        public bool IsPatterm { get; set; }
        /// <summary>
        /// 是否忽略大小写
        /// </summary>
        public bool IgnoreCase { get; set; }
    }
}
