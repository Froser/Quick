using System;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Security.Principal;
using System.Windows.Forms;

namespace Froser.Quick
{
    static class Program
    {
        public static NotifyIcon notify = new NotifyIcon();
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (QuickUpdate.LocalUpdate())
            {
                Environment.Exit(0);
                return;
            }

            if (Init())
            {
                Application.Run();
            }
        }

        public static void Quit()
        {
            notify.Visible = false;
            Application.ExitThread();
        }

        static bool Init()
        {
            const string adminComment = "Quick当前是以管理员身份运行。此时，您的宿主程序也必须是以管理员身份运行，这样Quick快捷菜单才能正常出现。";

            var mutexStr = "__QUICK____QUICK____QUICK____QUICK____QUICK__";
            bool canCreateMutex;
            Mutex programMutex = new Mutex(false, mutexStr, out canCreateMutex);
            if (canCreateMutex)
            {
                notify = QuickNotify.GetNotify();
                notify.Visible = true;

                if (QuickConfig.ThisConfig.FirstRun)
                {
                    QuickConfig.ThisConfig.FirstRun = false;
                    QuickConfig.ThisConfig.TrySave();

                    string additional = IsAdministrator() ? adminComment : string.Empty;

                    notify.ShowBalloonTip("Quick已经运行，您可以点击右键查看其选项。" + " " + adminComment);
                }
                else if (IsAdministrator())
                    notify.ShowBalloonTip(adminComment);

                Action silenceUpdate = () =>
                {
                    QuickUpdate.SilenceUpdate();
                };
                silenceUpdate.BeginInvoke(null, null);

                QuickListener.Listener.Run();
                return true;
            }
            return false;
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
