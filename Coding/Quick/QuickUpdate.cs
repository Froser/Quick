using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Froser.Quick
{
    internal static class QuickUpdate
    {
        /// <summary>
        /// 静默更新，有更新且换完成后返回true，否则返回false
        /// </summary>
        public static bool SilenceUpdate()
        {
            try
            {
                XDocument resp = null;
                string latestVersion = null;
                if (HasUpdate(out resp, out latestVersion))
                {
                    string updateUrlRoot = "http://10.20.133.13/download/update/";
                    var filelist = resp.Element("info").Element("version").Element ("filelist").Descendants();

                    string updateDir = Path.Combine (AppDir, "update");
                    
                    WebClient wc = new WebClient();
                    foreach (var file in filelist)
                    {
                        string fileAddress = updateUrlRoot + file.Value;
                        string downloadPath = Path.Combine(updateDir, fileAddress.Replace(updateUrlRoot, ""));
                        string dir = Path.GetDirectoryName(downloadPath);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        wc.DownloadFile(fileAddress, downloadPath);
                        // 因为服务器设置，不能直接下载的文件类型加上.update
                        // 在配置服务器的时候，应该添加.update的MIME关联
                        if (Path.GetExtension(downloadPath) == ".update")
                            File.Move(downloadPath, Path.Combine(Path.GetDirectoryName(downloadPath), Path.GetFileNameWithoutExtension(downloadPath)));
                    }

                    // 下载完所有更新文件后，写updatenotify文件
                    string notifyFilePath = Path.Combine(AppDir, "updatenotify");
                    using (var writer = File.CreateText(notifyFilePath))
                    {
                        writer.WriteLine(latestVersion);
                    }

                    // 准备退出程序，调用QuickUpdate

                    return true;
                }
            }
            catch { }
            return false;
        }

        public static bool LocalUpdate()
        {
            try
            {
                if (NeedLocalUpdate())
                {
                    Process proc = Process.Start(UpdateAppPath);
                    return true;
                }
            }
            catch { }
            return false;
        }

        private static bool NeedLocalUpdate()
        {
            if (File.Exists(NotifyFilePath))
            {
                using (var reader = File.OpenText(NotifyFilePath))
                {
                    return !QuickUtilities.IsLatestVersion(Application.ProductVersion, reader.ReadToEnd().Trim());
                }
            }
            return false;
        }

        private static bool HasUpdate(out XDocument resp, out string latestVersion)
        {
            try
            {
                resp = XDocument.Load(@"http://10.20.133.13/Download/Version.xml");
                String version = Application.ProductVersion;
                latestVersion = resp.Element("info").Element("version").Attribute("value").Value;
                return !QuickUtilities.IsLatestVersion(version, latestVersion);
            }
            catch
            {
                //先吞掉更新失败的问题
                resp = null;
                latestVersion = null;
                return false;
            }
        }

        private static string AppDir
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        private static string NotifyFilePath
        {
            get
            {
                return Path.Combine(AppDir, s_notifyFile);
            }
        }

        private static string UpdateAppPath
        {
            get
            {
                return Path.Combine(AppDir, s_updateAppPath);
            }
        }

        private const string s_notifyFile = "updatenotify";
        private const string s_updateAppPath = "quickupdate.exe";
    }
}
