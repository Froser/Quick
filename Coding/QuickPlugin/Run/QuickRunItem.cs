using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Froser.Quick.Plugins.Run
{
    internal class QuickRunItem : QuickRun
    {
        public QuickRunItem(QuickRun parent, string progName, string fullpath)
        {
            m_progName = progName;
            m_fullpath = fullpath;
            m_parent = parent;
        }

        public override string GetName()
        {
            return "运行(run)[yunxing] " + m_progName;
        }

        public override string GetDescription(IQuickWindow quickWindow)
        {
            return m_fullpath;
        }

        protected override void Execute(bool openDirectoryOnly, IQuickWindow quickWindow)
        {
            string arg = quickWindow.GetArgument();
            if (arg == null)
                return;

            string[] splits = arg.Split(new char[] { ' ', '\t', '\r', '\n' }, 2);
            string path = m_fullpath;

            if (openDirectoryOnly)
                path = Path.GetDirectoryName(m_fullpath);

            string shellarg = splits.Length > 1 ? splits[1] : null;
            Process.Start(path, shellarg ?? "");
        }

        public override bool GetIcon(IQuickWindow quickWindow, out System.Windows.Media.ImageSource icon)
        {
            try
            {
                if (Directory.Exists(m_fullpath))
                {
                    icon = GetRoot().DefaultFolderIcon;
                }
                else
                {
                    string dest = m_fullpath;
                    if (!Path.IsPathRooted(m_fullpath))
                        dest = MatchCommonShortcutPath(m_fullpath);

                    var tmp = Icon.ExtractAssociatedIcon(dest);     //获得主线程图标
                    icon = tmp.ToBitmapSource();
                }
                return true;
            }
            catch { }
            icon = null;
            return false;
        }

        protected override QuickRun GetRoot()
        {
            return m_parent;
        }

        protected override void Init()
        {
            // C#中是允许在构造的时候调用派生类虚函数的
        }

        public override List<String> GetCommonShortcuts()
        {
            return m_parent.GetCommonShortcuts();
        }

        public override Dictionary<String, String> GetCommonShortcutsPath()
        {
            return m_parent.GetCommonShortcutsPath();
        }

        public override bool TextDirty
        {
            get
            {
                return GetRoot().TextDirty;
            }
            set
            {
                GetRoot().TextDirty = value;
            }
        }

        public override Dictionary<String, String> CommonShortcutsPath
        {
            get
            {
                return GetRoot().CommonShortcutsPath;
            }
            set
            {
                GetRoot().CommonShortcutsPath = value;
            }
        }

        public override List<String> CommonShortcuts
        {
            get
            {
                return GetRoot().CommonShortcuts;
            }
            set
            {
                GetRoot().CommonShortcuts = value;
            }
        }

        public override List<String> ResultList
        {
            get
            {
                return GetRoot().ResultList;
            }
            set
            {
                GetRoot().ResultList = value;
            }
        }

        public override int AutoSearchIndex
        {
            get
            {
                return GetRoot().AutoSearchIndex;
            }
            set
            {
                GetRoot().AutoSearchIndex = value;
            }
        }

        public override bool TabKeyPressed
        {
            get
            {
                return GetRoot().TabKeyPressed;
            }
            set
            {
                GetRoot().TabKeyPressed = value;
            }
        }

        private string m_progName;
        private string m_fullpath;
        private QuickRun m_parent;
    }
}
