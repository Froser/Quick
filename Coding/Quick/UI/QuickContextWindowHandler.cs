using Froser.Quick.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;

namespace Froser.Quick.UI
{
    class QuickContextWindowHandler : IQuickContextWindowHandler
    {
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        public void SetHost(QuickContextWindow host)
        {
            m_host = host;
        }

        public void Init()
        {
            var list = m_host.GetList();
            foreach (var i in QuickConfig.ThisConfig.ContextMenuList)
            {
                QuickListItem item = new QuickListItem(i.Name, null, null);
                item.Tag = i;
                item.CreateListBoxItemTo(list);
            }
            list.ListItemClicked += itemClicked;
        }

        private void itemClicked(object sender, EventArgs e)
        {
            ListBoxItem listItem = (ListBoxItem)sender;
            QuickListItem rawItem = (QuickListItem)listItem.Tag;
            var menuItem = (QuickConfig.ContextMenuItem)rawItem.Tag;
            string concreteCmd = menuItem.Exec.Replace(QuickConfig.ContextMenuItem.Replacement, m_context.ToString());
            Process.Start(concreteCmd, menuItem.Argument);
        }

        public void BeforeShow(string context)
        {
            m_context = context;

            Point mousePos = new Point();
            GetCursorPos(ref mousePos);
            m_host.Left = mousePos.X;
            m_host.Top = mousePos.Y;
            m_host.Activate();
        }

        public void AfterShow()
        {
            if (!m_adjustedHeight)
            {
                var listItems = m_host.GetList().Items;
                if (listItems.Count > 0)
                {
                    ListBoxItem item = (ListBoxItem)m_host.GetList().Items[0];
                    double eachHeight = item.ActualHeight;
                    const int OFFSET = 4;
                    m_host.Height = (eachHeight + OFFSET) * listItems.Count ;
                }
                m_adjustedHeight = true;
            }
        }

        public void OnDeactivate(object sender, EventArgs e)
        {
            m_host.Hide();
        }

        private QuickContextWindow m_host;
        private bool m_adjustedHeight = false;
        private string m_context;
    }
}
