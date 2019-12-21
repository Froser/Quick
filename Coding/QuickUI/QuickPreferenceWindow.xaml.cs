using System.Windows;
using System.Windows.Controls;

namespace Froser.Quick.UI
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class QuickPreferenceWindow : Window
    {
        public QuickPreferenceWindow(IQuickPreferenceWindowHandler handler)
        {
            InitializeComponent();
            handler.SetHost(this);
            this.Closing += QuickPreferenceWindow_Closing;

            GetQuickTextBoxInGeneral().PreviewKeyDown += handler.ShortcutTextBoxOnPreviewKeyDown;
            GetContextTextBoxInGeneral().PreviewKeyDown += handler.ShortcutTextBoxOnPreviewKeyDown;
            GetSaveInTemplate().Click += QuickPreferenceWindow_SaveTemplate_Click;
            GetRestoreInTemplate().Click +=QuickPreferenceWindow_RestoreTemplate_Click;
            GetMoveUpInTemplate().Click += QuickPreferenceWindow_moveUp_Click;
            GetMoveDownInTemplate().Click += QuickPreferenceWindow_moveDown_Click;
            GetDeleteItemInTemplate().Click += QuickPreferenceWindow_delItem_Click;
            GetCreateItemInTemplate().Click += QuickPreferenceWindow_newItem_Click;
            GetSetToDefaultInGeneral().Click += QuickPreferenceWindow_default_Click;
            m_handler = handler;
        }
        void QuickPreferenceWindow_default_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnGeneralDefault();
        }

        void QuickPreferenceWindow_moveUp_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnMoveUpMethod();
        }

        void QuickPreferenceWindow_moveDown_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnMoveDownMethod();
        }

        void QuickPreferenceWindow_delItem_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnDeleteMethod();
        }

        void QuickPreferenceWindow_newItem_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnCreateNewMethod();
        }

        void QuickPreferenceWindow_SaveTemplate_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnSaveMethod();
        }

        void QuickPreferenceWindow_RestoreTemplate_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnRestoreMethod();
        }

        private void QuickPreferenceWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            m_handler.OnSave();
        }

        private void template_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_handler.OnTemplateSelected(GetTemplateListInTemplate().SelectedIndex);
        }

        private void template_methods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_handler.OnTemplateMethodSelect(GetMethodsListInTemplate().SelectedIndex);
        }

        public void ShowAbout()
        {
            about.IsSelected = true;
            Show();
        }

        public new void Show()
        {
            m_handler.LoadConfig();
            base.Show();
        }

        public TextBox GetContextTextBoxInGeneral()
        {
            return general_contextShortcutTx;
        }

        public TextBox GetQuickTextBoxInGeneral()
        {
            return general_quickShortcutTx;
        }

        public CheckBox GetContextShiftInGeneral()
        {
            return general_contextShift;
        }

        public CheckBox GetContextCtrlInGeneral()
        {
            return general_contextCtrl;
        }

        public CheckBox GetContextAltInGeneral()
        {
            return general_contextAlt;
        }

        public CheckBox GetContextWinInGeneral()
        {
            return general_contextWin;
        }

        public CheckBox GetUseContext()
        {
            return general_useContext;
        }

        public CheckBox GetQuickShiftInGeneral()
        {
            return general_quickShift;
        }

        public CheckBox GetQuickCtrlInGeneral()
        {
            return general_quickCtrl;
        }

        public CheckBox GetQuickAltInGeneral()
        {
            return general_quickAlt;
        }

        public CheckBox GetQuickWinInGeneral()
        {
            return general_quickWin;
        }

        public Button GetSetToDefaultInGeneral()
        {
            return general_setToDefault;
        }

        public ListView GetContextMenuInGeneral()
        {
            return general_contextMenu;
        }

        public ComboBox GetTemplateListInTemplate()
        {
            return template_list;
        }

        public ListBox GetMethodsListInTemplate()
        {
            return template_methods;
        }

        public TextBox GetMethodNameTextBoxInTemplate()
        {
            return tmp_methodname;
        }

        public TextBox GetMethodDescriptionTextBoxInTemplate()
        {
            return tmp_methoddesc;
        }

        public TextBox GetMethodScriptTextBoxInTemplate()
        {
            return tmp_methodscpt;
        }

        public TextBox GetMethodRegexTextBoxInTemplate()
        {
            return tmp_methodreg;
        }

        public TextBox GetMethodDefaultArgumentTextBoxInTemplate()
        {
            return tmp_methoddefarg;
        }

        public Button GetRestoreInTemplate()
        {
            return tmp_restore;
        }

        public Button GetSaveInTemplate()
        {
            return tmp_save;
        }

        public Button GetMoveUpInTemplate()
        {
            return tmp_moveUp;
        }

        public Button GetMoveDownInTemplate()
        {
            return tmp_moveDown;
        }

        public Button GetDeleteItemInTemplate()
        {
            return tmp_delItem;
        }

        public Button GetCreateItemInTemplate()
        {
            return tmp_newItem;
        }

        public TextBlock GetVersionTextBlockInAbout()
        {
            return tb_version;
        }

        private void newContextMenu_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnCreateNewContext();
        }

        private void removeContextMenu_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnRemoveContext();
        }

        private void modifyContextMenu_Click(object sender, RoutedEventArgs e)
        {
            m_handler.OnModifyContext();
        }

        private IQuickPreferenceWindowHandler m_handler;
    }
}
