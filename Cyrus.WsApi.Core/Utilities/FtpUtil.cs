using CoreFtp;
using CoreFtp.Enum;
using CoreFtp.Infrastructure;
using Cyrus.WsApi.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cyrus.WsApi.Core.Utilities
{
    public class FtpUtil
    {
        /// <summary>
        /// FTP下载
        /// </summary>
        /// <param name="ip">ftp服务主机ip</param>
        /// <param name="username">ftp用户名</param>
        /// <param name="password">ftp用户密码</param>
        /// <param name="baseDirectory">基础目录</param>
        /// <param name="remotepath">文件路径</param>
        /// <param name="localDirectory">本地保存目录</param>
        /// <param name="port">端口号，默认21</param>
        /// <param name="ftpEncryption"></param>
        /// <param name="ignoreCertificateErrors"></param>
        /// <returns></returns>
        public static async Task<bool> Download(string ip, string username, string password, string baseDirectory, string remotepath, string localDirectory, int port = 21, FtpEncryption ftpEncryption = FtpEncryption.Implicit, bool ignoreCertificateErrors = true)
        {
            var ftpConfig = new FtpConfig
            {
                Host = ip,
                Port = port,
                UserName = username,
                Password = password,
                BaseDirectory = baseDirectory,
                RemotePath = remotepath,
                LocalDirectory = localDirectory
            };
            return await Download(ftpConfig, ftpEncryption, ignoreCertificateErrors);
        }

        /// <summary>
        /// FTP下载
        /// </summary>
        /// <param name="ftpConfig">ftp配置</param>
        /// <param name="ftpEncryption"></param>
        /// <param name="ignoreCertificateErrors"></param>
        /// <returns></returns>
        public static async Task<bool> Download(FtpConfig ftpConfig, FtpEncryption ftpEncryption = FtpEncryption.Implicit, bool ignoreCertificateErrors = true)
        {

            using (var ftpClient = new FtpClient(new FtpClientConfiguration
            {
                Host = ftpConfig.Host,
                Port = ftpConfig.Port,
                Username = ftpConfig.UserName,
                Password = ftpConfig.Password,
                BaseDirectory = ftpConfig.BaseDirectory,
                EncryptionType = ftpEncryption,
                IgnoreCertificateErrors = true
            }))
            {
                await ftpClient.LoginAsync();

                if (string.IsNullOrWhiteSpace(ftpConfig.RemotePath))
                {
                    //文件路径为空,递归保存工作目录下的全部文件
                    await SaveFileAsync(ftpClient, ftpConfig);
                }
                else
                {
                    //文件路径不为空,只保存这一个文件
                    var localPath = await GetLocalPathAsync(ftpConfig);
                    using (var ftpReadStream = await ftpClient.OpenFileReadStreamAsync(ftpConfig.RemotePath))
                        await SaveFileAsync(ftpReadStream, localPath, ftpConfig.RenameFormat);
                }
            }
            return true;
        }

        /// <summary>
        /// 获取保存路径
        /// </summary>
        /// <param name="ftpConfig"></param>
        /// <returns></returns>
        private static async Task<string> GetLocalPathAsync(FtpConfig ftpConfig)
        {
            string localPath = string.Empty;
            string remoteFileName = Path.GetFileName(ftpConfig.RemotePath);
            await Task.Run(() =>
            {
                var categoryMap = ftpConfig?.CategoryMaps.FirstOrDefault(o =>
                {
                    if (o.IsPatterm)
                    {
                        return Regex.IsMatch(remoteFileName, o.MatchName, o.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                    }
                    else
                    {
                        return remoteFileName.Contains(o.MatchName, o.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.Ordinal);
                    }
                });
                localPath = Path.Combine(ftpConfig.LocalDirectory, categoryMap?.SubDirectory ?? "", remoteFileName);
            });
            return localPath;
        }

        /// <summary>
        /// 递归保存BaseDirectory目录下全部文件
        /// </summary>
        /// <param name="ftpClient"></param>
        /// <param name="ftpConfig"></param>
        /// <returns></returns>
        private static async Task SaveFileAsync(FtpClient ftpClient, FtpConfig ftpConfig)
        {
            var files = await ftpClient.ListFilesAsync();
            foreach (var file in files)
            {
                ftpConfig.RemotePath = file.Name;
                var localPath = await GetLocalPathAsync(ftpConfig);
                using (var ftpReadStream = await ftpClient.OpenFileReadStreamAsync(file.Name))
                {
                    await SaveFileAsync(ftpReadStream, localPath, ftpConfig.RenameFormat);
                }
            }
            var directories = await ftpClient.ListDirectoriesAsync();
            foreach (var directory in directories)
            {
                await ftpClient.ChangeWorkingDirectoryAsync(directory.Name);
                await SaveFileAsync(ftpClient, ftpConfig);
            }
        }

        /// <summary>
        /// 保存FTP文件流到本地
        /// </summary>
        /// <param name="ftpReadStream"></param>
        /// <param name="localPath"></param>
        /// <param name="renameFormat">已存在文件重命名规则,为空则替换</param>
        /// <returns></returns>
        private static async Task SaveFileAsync(Stream ftpReadStream, string localPath, string renameFormat = null)
        {
            if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            if (File.Exists(localPath))
            {
                if (!string.IsNullOrWhiteSpace(renameFormat))
                    localPath += DateTime.Now.ToString(renameFormat);
            }
            if (File.Exists(localPath))
                File.Delete(localPath);
            using (var fileWriteStream = File.OpenWrite(localPath))
                await ftpReadStream.CopyToAsync(fileWriteStream);
        }
    }
}
