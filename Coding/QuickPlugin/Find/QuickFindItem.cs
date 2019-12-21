using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace Froser.Quick.Plugins.Find
{
    internal class QuickFindItem : QuickFind
    {
        public QuickFindItem(QuickFind parent, string fullpath)
        {
            m_parent = parent;
            m_fullpath = fullpath;
        }

        public override string GetName()
        {
            return Path.GetFileName(m_fullpath);
        }

        public override string GetDescription(IQuickWindow quickWindow)
        {
            return m_fullpath;
        }

        public override int GetPage()
        {
            return GetRoot().GetPage();
        }

        public override void SetPage(int p)
        {
            GetRoot().SetPage(p);
        }

        public override void Invoke(object sender, IQuickWindow quickWindow)
        {
            if (m_directoryOpenedToogle)
            {
                m_directoryOpenedToogle = false;
                return;
            }
            Execute(false);
        }

        public override QuickFind GetRoot()
        {
            return m_parent;
        }

        public override bool GetIcon(IQuickWindow quickWindow, out System.Windows.Media.ImageSource icon)
        {
            try
            {
                var tmp = Icon.ExtractAssociatedIcon(m_fullpath);     //获得主线程图标
                icon = tmp.ToBitmapSource();
                return true;
            }
            catch { }
            icon = null;
            return false;
        }

        
        public override void KeyDown(IQuickWindow quickWindow, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    // Ctrl+Enter 打开文件夹
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        OpenDirectory();
                    break;
            }
        }

        public override DispatcherTimer GetTimer(IQuickWindow quickWindow)
        {
            return GetRoot().GetTimer(quickWindow);
        }

        private void Execute(bool openDirectoryOnly)
        {
            var path = openDirectoryOnly ? Path.GetDirectoryName(m_fullpath) : m_fullpath;
            Process.Start(path);
        }

        private void OpenDirectory()
        {
            m_directoryOpenedToogle = true;
            Execute(true);
        }

        public override DescriptionType CurrentDescriptionType
        {
            get
            {
                return GetRoot().CurrentDescriptionType;
            }
            set
            {
                GetRoot().CurrentDescriptionType = value;
            }
        }

        private QuickFind m_parent;
        private string m_fullpath;
        private bool m_directoryOpenedToogle = false;
    }
}
