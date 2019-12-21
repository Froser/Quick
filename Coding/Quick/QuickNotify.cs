using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Froser.Quick.UI;
using System.Windows.Forms.Integration;
using Froser.Quick.Properties;

namespace Froser.Quick
{
    internal abstract class QuickNotify
    {
        public static NotifyIcon GetNotify()
        {
            NotifyIcon notify = new NotifyIcon();
            notify.Text = "Quick";
            notify.Icon = Icon.FromHandle(Resources.quick.GetHicon());
            notify.MouseClick += (sender, e) => { if (e.Button == MouseButtons.Left) OnAbout(sender, e); };
            notify.ContextMenuStrip = new ContextMenuStrip();
            notify.ContextMenuStrip.Items.AddRange(new ToolStripItem[] {
                    new ToolStripMenuItem(Application.ProductName + " " + Application.ProductVersion ),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("访问主页", null, (sender, e) => Process.Start (QuickMultiVersion.s_homePage )),
                    new ToolStripMenuItem("功能缺失反馈", null, (sender, e) => Process.Start (QuickMultiVersion.s_commentPage )),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("重新载入配置", null, (sender, e) => QuickListener.Listener.Reload()),
                    new ToolStripMenuItem("偏好设置", null, (sender, e) => ShowPreference(false)),
                    new ToolStripMenuItem("关于", null, OnAbout),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("退出", null, (sender, e) => Program.Quit()),
                }
            );
            return notify;
        }

        private static void OnAbout(object sender, EventArgs e)
        {
            ShowPreference(true);
        }

        private static void ShowPreference(bool about)
        {
            if (s_perferenceWindowHandler == null)
                s_perferenceWindowHandler = new QuickPreferenceWindowHandler();

            if (s_perferenceWindow == null)
            {
                s_perferenceWindow = new QuickPreferenceWindow(s_perferenceWindowHandler);
                ElementHost.EnableModelessKeyboardInterop(s_perferenceWindow);
            }

            if (about)
                s_perferenceWindow.ShowAbout();
            else
                s_perferenceWindow.Show();
        }

        static QuickPreferenceWindow s_perferenceWindow;
        static QuickPreferenceWindowHandler s_perferenceWindowHandler;
    }
}
