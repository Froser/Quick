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
    /// QuickPerferenceContextEdit.xaml 的交互逻辑。不使用接口，因为功能简单
    /// </summary>
    public partial class QuickPerferenceContextEdit : Window
    {
        public QuickPerferenceContextEdit()
        {
            InitializeComponent();
            btn_done.Click += btn_done_Click;
        }

        void btn_done_Click(object sender, RoutedEventArgs e)
        {
            if (Done != null)
                Done();
            Close();
        }

        public new void Show()
        {
            if (Init != null)
                Init();
            base.ShowDialog();
        }

        public TextBox GetName()
        {
            return tb_name;
        }

        public TextBox GetBehavior()
        {
            return tb_behavior;
        }

        public TextBox GetArgument()
        {
            return tb_argument;
        }

        public event Action Init;
        public event Action Done;

        public object m_data;
    }
}
