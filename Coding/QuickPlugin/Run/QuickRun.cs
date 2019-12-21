using Froser.Quick.Plugins.Run;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Froser.Quick.Plugins
{
    internal class QuickRun : IQuickPluginMethod
    {
        public QuickRun()
        {
            Init();
        }

        // 接口部分
        public virtual string GetName()
        {
            return "运行(run)[yunxing] 文件";
        }

        public virtual string GetDescription(IQuickWindow quickWindow)
        {
            return "运行文件，按Tab可以自动补全文件路径，Ctrl+Enter打开其所在的文件夹";
        }

        public void Invoke(object sender, IQuickWindow quickWindow)
        {
            if (m_directoryOpenedToogle)
            {
                m_directoryOpenedToogle = false;
                return;
            }
            Execute(false, quickWindow);
        }

        public bool AcceptArgs()
        {
            return true;
        }

        public string AvailableApplicationName()
        {
            return null;
        }

        public virtual bool GetIcon(IQuickWindow quickWindow, out ImageSource icon)
        {
            icon = null;
            return false;
        }

        public void KeyDown(IQuickWindow quickWindow, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    TabKeyPressed = true;
                    CompletePathInTextbox(quickWindow, e);
                    return;
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                    break;
                case Key.Enter:
                    // Ctrl+Enter 打开文件夹
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        OpenDirectory(quickWindow);
                    break;
            }
        }

        public void TextChanged(IQuickWindow quickWindow, TextChangedEventArgs e)
        {
            if (TabKeyPressed)
            {
                TabKeyPressed = false;
                TextDirty = false;
            }
            else
            {
                TextDirty = true;
            }

            // 在没有参数传进来的时候，因为已经退出了SubModel
            var arg = quickWindow.GetArgument ();
            if (arg == null)
            {
                quickWindow.ResetMethods();
                return;
            }

            List<string> matchedFiles = new List<string>();
            matchedFiles = GetCurrentPathFileNames(arg);

            List<QuickRunItem> runItemList = new List<QuickRunItem>();
            foreach (var r in matchedFiles)
            {
                QuickRunItem item = new QuickRunItem(GetRoot(), Path.GetFileNameWithoutExtension(r), MatchCommonShortcutPath(r) ?? r);
                runItemList.Add (item);
            }

            if (runItemList.Count > 0)
                quickWindow.ReplaceMethods(runItemList.ToArray());
            else
                quickWindow.ResetMethods();
        }

        public void Closed(IQuickWindow quickWindow)
        {
            quickWindow.Refresh(0);
        }

        public void PageDown(IQuickWindow quickWindow)
        {
        }

        public void PageUp(IQuickWindow quickWindow)
        {
        }

        // 内部实现
        public virtual List<String> GetCommonShortcuts()
        {
            return CommonShortcuts;
        }

        public virtual Dictionary<String, String> GetCommonShortcutsPath()
        {
            return CommonShortcutsPath;
        }

        protected virtual QuickRun GetRoot()
        {
            return this;
        }

        protected virtual void Init()
        {
            CommonShortcutsPath = new Dictionary<string, string>();
            CommonShortcuts = new List<string>();
            ResultList = new List<string>();
            AutoSearchIndex = 0;
            TabKeyPressed = false;

            GetDrives();

            try
            {
                IntPtr hFolderBitmap = Resource.folder.GetHbitmap();
                DefaultFolderIcon = Imaging.CreateBitmapSourceFromHBitmap(hFolderBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch { }

            //来自注册表
            try
            {
                RegistryKey regkey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
                var appKey = regkey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths");
                if (appKey != null)
                    FillCommonShortcutsInfo(appKey);

                regkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                appKey = regkey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths");
                if (appKey != null)
                    FillCommonShortcutsInfo(appKey);
            }
            catch { }

            // 考虑用一种O(logN)的数据结构代替
            FillSysEnvPATHShortcutsInfo();
            FillAllProgramInfo();
            CommonShortcuts.Sort();
        }

        protected virtual void Execute(bool openDirectoryOnly, IQuickWindow quickWindow)
        {
            string arg = quickWindow.GetArgument();
            if (arg == null)
                return;

            string[] splits = arg.Split(new char[] { ' ', '\t', '\r', '\n' }, 2);
            string path = splits[0];

            if (openDirectoryOnly)
            {
                string tmpPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(tmpPath))
                {
                    tmpPath = MatchCommonShortcutPath(path);
                    if (tmpPath != null)
                    {
                        tmpPath = Path.GetDirectoryName(tmpPath);
                        if (!Directory.Exists(tmpPath))
                            return;
                        path = tmpPath;
                    }
                }
            }

            string shellarg = splits.Length > 1 ? splits[1] : null;
            Process.Start(path, shellarg ?? "");
        }

        protected string MatchCommonShortcutPath(string shortcutName)
        {
            string pathKey = RemoveExeExtension(shortcutName);
            if (GetCommonShortcutsPath().ContainsKey(pathKey.ToLower()))
                return GetCommonShortcutsPath()[pathKey.ToLower()];
            return null;
        }

        private List<string> GetCurrentPathFileNames(string queryPath)
        {
            List<string> resultList = new List<string>();
            try
            {
                resultList.Clear();
                if (queryPath.Length > 0)
                {
                    if (Path.IsPathRooted(queryPath))
                    {
                        string nearestPath = null;
                        // 获得最接近的路径
                        if (QuickUtilities.IsRoot(queryPath))
                        {
                            resultList.AddRange(GetDrives());
                            nearestPath = Path.GetPathRoot(queryPath);
                            if (!nearestPath.EndsWith(@"\"))
                                nearestPath += @"\";
                        }
                        else
                        {
                            nearestPath = Path.GetDirectoryName(queryPath);
                        }
                        resultList.AddRange(Directory.GetDirectories(nearestPath));
                        resultList.AddRange(Directory.GetFiles(nearestPath));
                    }
                    else
                    {
                        // 全局路径
                        resultList.AddRange(GetCommonShortcuts());
                    }
                }
            }
            catch { }

            resultList.RemoveAll((item) =>
            {
                if (Path.IsPathRooted(item))
                {
                    return !item.StartsWith(queryPath, true, CultureInfo.CurrentCulture);
                }
                else
                {
                    return !Path.GetFileName(item).StartsWith(queryPath, true, CultureInfo.CurrentCulture);
                }
            });

            return resultList;
        }

        private void CompletePathInTextbox(IQuickWindow quickWindow, KeyEventArgs e)
        {
            var modifiers = e.KeyboardDevice.Modifiers;
            string queryPath = quickWindow.GetArgument();

            if (queryPath.Length == 0)
                return;

            if (TextDirty)
                ResultList = GetCurrentPathFileNames(queryPath);

            if (ResultList.Count == 0)
                return;

            if (modifiers != ModifierKeys.Shift)
            {
                AutoSearchIndex = ++AutoSearchIndex % ResultList.Count;
            }
            else if (modifiers == ModifierKeys.Shift)
            {
                AutoSearchIndex = --AutoSearchIndex < 0 ? ResultList.Count - 1 : AutoSearchIndex;
            }

            string content = quickWindow.GetQueryText();
            string replaced = content.Replace(queryPath, ResultList[AutoSearchIndex]);
            quickWindow.SetQueryText(replaced);
            var textbox = quickWindow.GetQueryTextBox();
            textbox.CaretPosition = textbox.Document.ContentEnd;
        }

        private void FillCommonShortcutsInfo(RegistryKey key)
        {
            var names = key.GetSubKeyNames();
            foreach (var name in names)
            {
                var subkey = key.OpenSubKey(name);
                var value = subkey.GetValue("");
                var fullpath = value == null ? "" : value.ToString();
                if (fullpath.Trim() == "")
                    continue;

                // 只加载有效项
                try
                {
                    fullpath = Path.GetFullPath(fullpath.Replace ("\"", ""));
                }
                catch 
                { 
                    continue; 
                }
                if (!System.IO.File.Exists(fullpath) || Path.GetExtension(fullpath).ToLower() != ".exe")
                    continue;

                CommonShortcuts.Add(name);
                string pathKey = RemoveExeExtension(name);
                if (!CommonShortcutsPath.ContainsKey(pathKey.ToLower()))
                    CommonShortcutsPath.Add(pathKey.ToLower(), fullpath);
            }
        }

        private void FillSysEnvPATHShortcutsInfo()
        {
            var envPaths = Environment.GetEnvironmentVariable("path").Split(';');
            foreach (var p in envPaths)
            {
                try
                {
                    foreach (var fullpath in Directory.GetFiles(p, "*.exe"))
                    {
                        if (!CommonShortcuts.Contains(fullpath))
                        {
                            string filename = Path.GetFileNameWithoutExtension(fullpath).ToLower();
                            if (!CommonShortcutsPath.ContainsValue(fullpath))
                            {
                                CommonShortcuts.Add(filename);
                                CommonShortcutsPath.Add(filename, fullpath);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void FillAllProgramInfo()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms);
            string[] allPrograms = Directory.GetFiles(path, "*.lnk", SearchOption.AllDirectories);
            foreach (var link in allPrograms)
            {
                WshShell shell = new WshShell();
                IWshShortcut sc = (IWshShortcut)shell.CreateShortcut(link);
                string targetPath = sc.TargetPath;
                if (Path.GetExtension(targetPath).ToLower () == ".exe")
                {
                    string filename = Path.GetFileNameWithoutExtension(link).ToLower();
                    if (!CommonShortcuts.Contains(filename))
                        CommonShortcuts.Add(filename);
                    if (!CommonShortcutsPath.ContainsKey(filename))
                        CommonShortcutsPath.Add(filename, link);
                }
            }
        }

        private List<string> GetDrives()
        {
            var driveList = new List<string>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                // TODO: 当isReady改变的时候，应该重新初始化一次
                if (!drive.IsReady)
                    continue;
                string caption = drive.Name ;
                if (drive.VolumeLabel.Trim() != "")
                    caption += " (" + drive.VolumeLabel + ")";
                string directory = drive.RootDirectory.Name;
                driveList.Add(caption);
                if (!m_driveInited)
                {
                    CommonShortcuts.Add(caption);
                    CommonShortcutsPath.Add(caption, directory);
                    m_driveInited = true;
                }
            }

            return driveList;
        }

        private string RemoveExeExtension(string strIn)
        {
            const string targetString = ".exe";
            if (strIn.EndsWith(targetString, StringComparison.OrdinalIgnoreCase))
                return strIn.Substring(0, strIn.Length - targetString.Length);
            return strIn;
        }

        private void OpenDirectory(IQuickWindow quickWindow)
        {
            m_directoryOpenedToogle = true;
            Execute(true, quickWindow);
        }

        // 文件名自动搜索
        public virtual bool TextDirty
        {
            get
            {
                return m_editedByHand;
            }
            set
            {
                m_editedByHand = value;
                if (value)
                    AutoSearchIndex = 0;
            }
        }
        private bool m_editedByHand = true;
        public virtual Dictionary<String, String> CommonShortcutsPath { get; set; }

        // 不带exe结尾的文件名
        public virtual List<String> CommonShortcuts { get; set; }

        // 文件名和其完整路径组成的关系容器
        public virtual List<String> ResultList { get; set; }

        // Tab键保存路径时的路径索引
        public virtual int AutoSearchIndex { get; set; }

        // 是否按下了Tab导致TextChanged
        public virtual bool TabKeyPressed { get; set; }

        public ImageSource DefaultFolderIcon { get; set; }
        private bool m_directoryOpenedToogle = false;
        private bool m_driveInited = false;
    }

}
