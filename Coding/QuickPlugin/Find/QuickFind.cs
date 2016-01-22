using Froser.Quick.Plugins.Find;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace Froser.Quick.Plugins
{
    internal class QuickFind : IQuickPluginMethod
    {
        private enum SearchOption
        {
            All,
            File,
            Folder
        }

        private string EverythingExePath
        {
            get
            {
                return m_everythingExePath;
            }
        }

        private string m_everythingExePath = null;

        public virtual string GetName()
        {
            return "查找(find)[chazhao] 任何文件";
        }

        public virtual string GetDescription(IQuickWindow quickWindow)
        {
            switch (CurrentDescriptionType)
            {
                case DescriptionType.NoResult:
                    return NO_RESULT_DESCRIPTION;
                case DescriptionType.NotInited:
                    return NOT_INITED_YET;
                default:
                    return DEFAULT_DESCRIPTION;
            }
        }

        public string AvailableApplicationName()
        {
            return null;
        }

        public virtual void Invoke(object sender, IQuickWindow quickWindow)
        {
        }

        public bool AcceptArgs()
        {
            return true;
        }

        public virtual bool GetIcon(IQuickWindow quickWindow, out System.Windows.Media.ImageSource icon)
        {
            icon = null;
            return false;
        }

        public virtual void KeyDown(IQuickWindow quickWindow, System.Windows.Input.KeyEventArgs e)
        {
            if (quickWindow != null && m_everythingExePath == null)
                m_everythingExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    quickWindow.GetPluginsPath(), 
                    "3rdparty/everything/everything.exe");
        }

        public void TextChanged(IQuickWindow quickWindow, System.Windows.Controls.TextChangedEventArgs e)
        {
            ResetPage();

            var timer = GetTimer(quickWindow);
            timer.Stop();

            var arg = quickWindow.GetArgument();
            if (arg == null)
            {
                quickWindow.ResetMethods();
                CurrentDescriptionType = DescriptionType.Default;
                return;
            }

            timer.Start();
        }

        public void Closed(IQuickWindow quickWindow)
        {
            ResetPage();
            CurrentDescriptionType = DescriptionType.Default;
            quickWindow.Refresh(0);
        }

        public void PageDown(IQuickWindow quickWindow)
        {
            UpdatePage(quickWindow, true);
        }

        public void PageUp(IQuickWindow quickWindow)
        {
            UpdatePage(quickWindow, false);
        }

        public virtual int GetPage()
        {
            return m_page;
        }

        public virtual void SetPage(int p)
        {
            m_page = p;
        }

        public virtual QuickFind GetRoot()
        {
            return this;
        }

        public virtual DispatcherTimer GetTimer(IQuickWindow quickWindow)
        {
            if (m_timer == null)
            {
                m_timer = new DispatcherTimer(DispatcherPriority.Normal);
                m_timer.Interval = TimeSpan.FromMilliseconds(COUNT_DOWN_TIMEOUT);
                m_timer.Tick += QueryOnTick;
                m_timer.Tag = quickWindow;
            }
            return m_timer;
        }

        private void QueryOnTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            IQuickWindow quickWindow = (IQuickWindow)timer.Tag;
            AsyncQuery(quickWindow);
            timer.Stop();
        }

        private void UpdatePage(IQuickWindow quickWindow, bool isPageDown)
        {
            int backupPage = GetPage();
            if (isPageDown)
            {
                SetPage(GetPage() + 1);
            }
            else
            {
                if (GetPage() > 0)
                    SetPage(GetPage() - 1);
                else
                    return;
            }

            bool success = Query(quickWindow);
            if (!success)
            {
                SetPage(backupPage);
                return;
            }

            if (isPageDown)
                quickWindow.Refresh(0);
            else
                quickWindow.Refresh(MAX_FILE_NUMBER_IN_ONE_QUERY);
        }

        private void ResetPage()
        {
            SetPage(0);
        }

        private void AsyncQuery(IQuickWindow quickWindow)
        {
            quickWindow.AsyncInvoke(
                () =>
                {
                    Query(quickWindow);
                    quickWindow.Refresh(0);
                }
            );
        }

        private bool Query(IQuickWindow quickWindow)
        {
            bool bInited = TestEverything(quickWindow);
            if (!bInited)
            {
                CurrentDescriptionType = DescriptionType.NotInited;
                quickWindow.Refresh(0);
                return false;
            }

            QuickEverything.Everything_Reset();
            QuickEverything.Everything_SetMax(MAX_FILE_NUMBER_IN_ONE_QUERY);
            QuickEverything.Everything_SetOffset(GetPage() * MAX_FILE_NUMBER_IN_ONE_QUERY);

            string arg = quickWindow.GetArgument();
            if (arg == null)
                return false;

            //改善易用性，将/视为\
            arg = arg.Replace('/', '\\');
            SearchOption option = SearchOption.File;
            string a = arg.Trim().ToLower();
            if (a.StartsWith("folder:"))
                option = SearchOption.Folder;
            else if (a.StartsWith("all:"))
                option = SearchOption.All;

            string realQueryText = arg;
            if (option == SearchOption.File)
                realQueryText = "file:" + arg;
            else if (option == SearchOption.Folder)
                realQueryText = "folder:" + arg;

            QuickEverything.Everything_SetSearchW(realQueryText);
            QuickEverything.Everything_QueryW(true);
            int count = QuickEverything.Everything_GetNumResults();
            EVERYTHING_RESULT result = (EVERYTHING_RESULT)QuickEverything.Everything_GetLastError();
            if (result == EVERYTHING_RESULT.EVERYTHING_OK && count > 0)
            {
                ReplaceMethods(quickWindow, count);
                return true;
            }
            CurrentDescriptionType = DescriptionType.NoResult;
            return false;
        }

        private bool TestEverything(IQuickWindow quickWindow)
        {
            // 返回引擎是否已经初始化
            QuickEverything.Everything_Reset();
            QuickEverything.Everything_SetMax(1);
            QuickEverything.Everything_SetOffset(0);
            QuickEverything.Everything_SetSearchW("*");
            bool success = false;
            bool startServer = false;
            int times = 0;
            do
            {
                success = QuickEverything.Everything_QueryW(true);
                if (success)
                {
                    quickWindow.UnlockWindow();
                    int count = QuickEverything.Everything_GetNumResults();
                    return count > 0 ? true : false;
                }
                else
                {
                    quickWindow.LockWindow();
                    if (!startServer)
                    {
                        StartService();
                        startServer = true;
                    }
                    Thread.Sleep(COUNT_DOWN_TIMEOUT);
                    times++;
                    if (times > MAX_TRYING_TIMES)
                        return false;
                }
            } while (!success);
            Debug.Assert(false, "应该在success的时候就返回了");
            return false;
        }

        private void ReplaceMethods(IQuickWindow quickWindow, int count)
        {
            const int bufsize = 260;
            StringBuilder buf = new StringBuilder(bufsize);
            List<QuickFindItem> findItemList = new List<QuickFindItem>();
            for (int i = 0; i < count; i++ )
            {
                QuickEverything.Everything_GetResultFullPathNameW(i, buf, bufsize);
                EVERYTHING_RESULT result = (EVERYTHING_RESULT)QuickEverything.Everything_GetLastError();
                if (result != EVERYTHING_RESULT.EVERYTHING_OK)
                    continue;

                QuickFindItem item = new QuickFindItem(GetRoot(), buf.ToString ());
                findItemList.Add(item);
            }

            quickWindow.ReplaceMethods(findItemList.ToArray());
        }

        private void StartService()
        {
            const string serviceName = "Everything";
            Process.Start(EverythingExePath, "-startup -admin");
            if (!IsServiceExisted(serviceName))
            {
                var service = Process.Start(EverythingExePath, "-install-client-service");
                service.WaitForExit();
            }
        }

        private bool IsServiceExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                    return true;
            }
            return false;
        }

        private const string DEFAULT_DESCRIPTION = "使用Everything服务秒查全盘的文件，Ctrl+Enter打开其所在的文件夹";
        private const string NO_RESULT_DESCRIPTION = "没有找到结果";
        private const string NOT_INITED_YET = "引擎正在初始化，请稍后再重新搜索";
        private const int MAX_FILE_NUMBER_IN_ONE_QUERY = 10;
        private const int COUNT_DOWN_TIMEOUT = 500;
        private const int MAX_TRYING_TIMES = 50;
        public enum DescriptionType { Default, NoResult, NotInited };
        public virtual DescriptionType CurrentDescriptionType { get; set; }
        private DispatcherTimer m_timer ;
        private int m_page = 0;
    }
}
