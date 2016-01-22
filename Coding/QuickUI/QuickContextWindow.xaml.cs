using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Froser.Quick.UI
{
    /// <summary>
    /// QuickContextWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QuickContextWindow : Window
    {
        public QuickContextWindow(IQuickContextWindowHandler handler)
        {
            InitializeComponent();

            handler.SetHost(this);
            m_handler = handler;
            this.Deactivated += handler.OnDeactivate;

            handler.Init();
        }

        public void Show(string context)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            m_handler.BeforeShow(context);
            Show();
            m_handler.AfterShow();
        }

        public QuickListBox GetList()
        {
            return quickContextList;
        }

        private IQuickContextWindowHandler m_handler;
    }
}
