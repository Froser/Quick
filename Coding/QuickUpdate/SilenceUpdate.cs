using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Froser.Quick.QuickUpdate
{
    public static class SilenceUpdate
    {
        public static void Update()
        {
            while (IsQuickRunning())
            {
                Thread.Sleep(100);
            }

            if (HasNotifyFile())
            {
                try
                {
                    // 准备更新，将文件全部都拷贝出来，然后运行Quick.exe，然后删除updatenotify和update文件夹
                    CopyFile();
                    Clear();
                    Exec();
                    Done();
                } 
                catch
                {
                    // Swallow exceptions
                }
            }
        }
        
        private static bool IsQuickRunning()
        {
            Process[] proc = Process.GetProcessesByName(s_processName);
            return proc.Length > 0;
        }

        private static bool HasNotifyFile()
        {
            
            return File.Exists(NotifyFilePath);
        }

        private static void CopyFile()
        {
            string[] files = Directory.GetFiles(UpdateDir, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string relPath = file.Replace(UpdateDir, "");
                string dest = Path.Combine(AppDir, relPath);
                File.Copy(file, dest, true);
            }
        }

        private static void Clear()
        {
            File.Delete(NotifyFilePath);
            Directory.Delete(UpdateDir, true);
        }

        private static void Exec()
        {
            Debug.Assert(File.Exists(AppPath));
            Process.Start(AppPath);
        }

        private static void Done()
        {
        }

        private static string AppDir
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        private static string UpdateDir
        {
            get
            {
                return Path.Combine(AppDir, s_updateDirName) + @"\";
            }
        }

        private static string NotifyFilePath
        {
            get
            {
                return Path.Combine(AppDir, s_notifyFile);
            }
        }

        private static string AppPath
        {
            get
            {
                return Path.Combine(AppDir, s_appName);
            }
        }

        private const string s_notifyFile = "updatenotify";
        private const string s_appName = "quick.exe";
        private const string s_updateDirName = "update";
        private const string s_processName = "Quick";
    }
}
