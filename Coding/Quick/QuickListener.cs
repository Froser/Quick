using Froser.Quick.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Froser.Quick
{
    internal class QuickListener
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        public static QuickListener Listener
        {
            get{
                return s_listener ?? (s_listener = new QuickListener());
            }
        }

        private QuickListener()
        {
            m_notify = Program.notify;
            m_quickHotkey = new Hotkey(IntPtr.Zero);
            m_contextHotkey = new Hotkey(IntPtr.Zero);

            Init();
        }

        public void Init()
        {
            QuickConfig.ThisConfig.Reload();
            QuickConfig.ThisConfig.CurrentVersion = Application.ProductVersion;
            QuickConfig.ThisConfig.TrySave();

            m_quickModels = new Dictionary<string, QuickModel>();

            var globalModel = QuickModel.GetModel(QuickModel.GlobalModelName + ".xml");
            m_quickModels.Add(QuickModel.GlobalModelName, globalModel);    //添加全局Model

            foreach (var item in QuickConfig.ThisConfig.ModelName)
            {
                if (!m_quickModels.ContainsKey(item))
                {
                    m_quickModels.Add(item, QuickModel.GetModel(item + ".xml"));
                }
            }

            //装载插件
            QuickPluginLoader.AddAdditionQuickMethodTo(m_quickModels);

            m_quickContextWindowHandler = new QuickContextWindowHandler();
            m_quickContextWindow = new QuickContextWindow(m_quickContextWindowHandler);
            ElementHost.EnableModelessKeyboardInterop(m_quickContextWindow);
        }

        public void Run()
        {
            m_quickHotkey.RegisterHotkey(QuickConfig.ThisConfig.QuickHotKey, (Hotkey.KeyFlags)QuickConfig.ThisConfig.QuickHotKeyFlags);
            m_quickHotkey.OnHotkey += QuickHotkey_OnHotkey;

            m_contextHotkey.RegisterHotkey(QuickConfig.ThisConfig.ContextMenuHotKey, (Hotkey.KeyFlags)QuickConfig.ThisConfig.ContextMenuHotKeyFlags);
            m_contextHotkey.OnHotkey += ContextHotkey_OnHotkey;
        }

        public void Reload()
        {
            m_quickHotkey.UnregisterHotkeys();
            m_contextHotkey.UnregisterHotkeys();
            m_quickHotkey = new Hotkey(IntPtr.Zero);
            m_contextHotkey = new Hotkey(IntPtr.Zero);
            Init();
            Run();
        }

        private void ContextHotkey_OnHotkey(int HotKeyID)
        {
            try
            {
                object obj = GetActiveObject(false);
                if (QuickConfig.ThisConfig.ContextMenuToogle)
                {
                    var textObject = QuickReflection.Invoke(m_currentModel.Search, obj);
                    if (textObject != null)
                    {
                        m_quickContextWindow.Show(textObject.ToString ());
                        QuickVitality.UpdateVitality("context", m_currentModel.ProgramName, textObject.ToString());
                    }
                }
            }
            catch { }
        }

        public void QuickHotkey_OnHotkey(int HotKeyID)
        {
            Trigger(false);
        }

        private void Trigger(bool forceGetGlobalObject)
        {
            QuickContext quickContext = default(QuickContext);
            bool isGlobal;
            Process currentProcess;
            IntPtr windowPtr;
            object obj = GetActiveObject(forceGetGlobalObject, out isGlobal, out windowPtr, out currentProcess);
            SetupQuickMainWindowAndHandler(obj, quickContext, windowPtr, isGlobal, currentProcess);
            TriggerQuickMainWindow(isGlobal);
        }

        private void TriggerQuickMainWindow(bool isGlobal)
        {
            if (m_quickMainWindow != null && m_quickMainWindow.IsVisible)
            {
                m_quickMainWindow.Close(); //如果窗体已存在并在显示状态，则触发FormClosing，Hide()
                if (!isGlobal)
                {
                    // 非公共窗口，再次触发快捷键会打开Global的window
                    Trigger(true);
                }
                return;
            }
            else
            {
                if (m_currentModel.MethodList.Count > 0)
                {
                    QuickVitality.UpdateVitality("showlist", m_currentModel.ProgramName, "");
                    m_quickMainWindow.Show();
                    m_quickMainWindow.Activate();
                }
            }
        }

        private object GetGlobalObject(IntPtr windowPtr, bool isGlobal)
        {
            if (m_commonObject == null)
                m_commonObject = new QuickCommonObject(windowPtr, isGlobal); //调用公共的Model
            return m_commonObject;
        }

        private object GetActiveObject(bool bForceGetGlobalObject)
        {
            bool isGlobal;
            Process currentProcess;
            IntPtr windowPtr;
            return GetActiveObject(false, out isGlobal, out windowPtr, out currentProcess);
        }

        private object GetActiveObject(bool bForceGetGlobalObject, out bool isGlobalModel, out IntPtr windowPtr, out Process currentProcess)
        {
            object currentActiveObject = null;
            windowPtr = GetForegroundWindow();
            int currentPID;
            GetWindowThreadProcessId(windowPtr, out currentPID);
            currentProcess = Process.GetProcessById(currentPID);    //获得活动窗口的进程

            isGlobalModel = true;
            string modelProcessName = string.Empty;
            if (bForceGetGlobalObject || !m_quickModels.ContainsKey(currentProcess.ProcessName, true, out modelProcessName))
            {
                modelProcessName = QuickModel.GlobalModelName;
            }

            if (!isGlobalModel)
                Validate(currentProcess.ProcessName);
            m_currentModel = m_quickModels[modelProcessName];

            if (m_currentModel.ProgramName.Trim() != "")
            {
                bool bFailed = true;
                foreach (var name in m_currentModel.ProgramName.Split(','))
                {
                    try
                    {
                        currentActiveObject = Marshal.GetActiveObject(name.Trim()); //调用当前的Model
                        bFailed = false;
                        break;
                    }
                    catch { }
                }

                if (!bFailed)
                {
                    isGlobalModel = false;
                }
                else
                {
                    m_currentModel = m_quickModels[QuickModel.GlobalModelName];
                    currentActiveObject = GetGlobalObject(windowPtr, true); //调用公共的Model
                }
            }
            else
            {
                currentActiveObject = GetGlobalObject(windowPtr, true); //调用公共的Model
            }

            return currentActiveObject;
        }

        private void SetupQuickMainWindowAndHandler(object obj, QuickContext quickContext, IntPtr windowPtr, bool isGlobal, Process currentProcess)
        {
            if (m_handlerMap == null)
                m_handlerMap = new Dictionary<QuickModel, QuickMainWindowHandler>();
            if (m_windowMap == null)
                m_windowMap = new Dictionary<QuickMainWindowHandler, QuickMainWindow>();

            QuickMainWindowHandler handler = null;
            if (m_handlerMap.ContainsKey(m_currentModel))
            {
                handler = m_handlerMap[m_currentModel];
            }
            else
            {
                handler = new QuickMainWindowHandler(obj, m_currentModel, quickContext, windowPtr, isGlobal, currentProcess);
                m_handlerMap.Add(m_currentModel, handler);
            }

            if (m_windowMap.ContainsKey(handler))
            {
                m_quickMainWindow = m_windowMap[handler];
            }
            else
            {
                m_quickMainWindow = new QuickMainWindow(handler);
                ElementHost.EnableModelessKeyboardInterop(m_quickMainWindow);
                m_windowMap.Add(handler, m_quickMainWindow);
            }

            System.Windows.Media.Color bgColor = System.Windows.Media.Color.FromRgb((byte)m_currentModel.BorderColorR, (byte)m_currentModel.BorderColorG, (byte)m_currentModel.BorderColorB);
            m_quickMainWindow.SetBackgroundColor(bgColor);
        }

        private void Validate(String processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 1)
            {
                m_notify.ShowBalloonTip("您开启了多个" + processName + "进程，当您调用快捷菜单时，只能对第一个进程进行操作，如果要对其他" + processName + "进行操作，请关闭多余的此类进程。");
            }
        }

        private static QuickListener s_listener;
        private Dictionary<String, QuickModel> m_quickModels;
        private Hotkey m_quickHotkey;
        private Hotkey m_contextHotkey;
        private QuickContextWindow m_quickContextWindow;
        private QuickContextWindowHandler m_quickContextWindowHandler;
        private QuickModel m_currentModel;
        private QuickMainWindow m_quickMainWindow;
        private NotifyIcon m_notify;
        private QuickCommonObject m_commonObject;
        private Dictionary<QuickModel, QuickMainWindowHandler> m_handlerMap;
        private Dictionary<QuickMainWindowHandler, QuickMainWindow> m_windowMap;
    }
}
