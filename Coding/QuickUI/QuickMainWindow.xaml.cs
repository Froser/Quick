using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Froser.Quick.UI
{
    /// <summary>
    /// QuickMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QuickMainWindow : Window
    {
        public QuickMainWindow(IQuickMainWindowHandler handler)
        {
            InitializeComponent();

            m_handler = handler;
            handler.SetHostWindow(this);

            Closing += handler.OnClosing;
            Deactivated += handler.OnDeactivated;
            Activated += handler.OnActivated;
            GetQueryTextBox().CaptureMouse();
            GetQueryTextBox().TextChanged += handler.OnTextChanged;
            GetQueryTextBox().MouseWheel += handler.OnMouseWheel;
            GetQueryTextBox().PreviewKeyDown += handler.OnKeyDown;
            GetList().ListItemClicked += handler.OnListItemClicked;
            GetList().KeyDown += handler.OnKeyDown;
            GetList().MouseWheel += handler.OnMouseWheel;
            GetLockImageButton().PreviewMouseDown += handler.OnLockedClick;

            handler.Init();

            backgroundBorder.PreviewMouseDown += dragMove;
            innerBorder.PreviewMouseDown += dragMove;
        }

        public new void Show()
        {
            m_handler.OnShowing();
            base.Show();
            m_handler.OnShowed();
        }

        public void SetOpacity(double o)
        {
            mainWindow.Opacity = o;
        }

        public void ResetOpacity()
        {
            mainWindow.Opacity = 1.0;
        }

        public RichTextBox GetQueryTextBox()
        {
            return searchTextBox;
        }

        public QuickListBox GetList()
        {
            return quickListBox;
        }

        public Image GetLockImageButton()
        {
            return iconLock;
        }

        public void SelectNext()
        {
            var list = GetList();
            int nextIndex = list.SelectedIndex + 1;
            if (nextIndex >= list.Items.Count)
                m_handler.OnListPageDown();
            else
                list.SelectedIndex = nextIndex;
        }

        public void SelectPrevious()
        {
            var list = GetList();
            int nextIndex = list.SelectedIndex - 1;
            if (nextIndex < 0)
                m_handler.OnListPageUp();
            else
                list.SelectedIndex = nextIndex;
        }

        public void Select(int index)
        {
            var list = GetList();
            list.SelectedIndex = index;
        }

        public void AutoResize()
        {
            int itemsHeight = GetList().Items.Count * UNIT_HEIGHT;
            if (itemsHeight < MIN_LIST_HEIGHT)
                itemsHeight = MIN_LIST_HEIGHT;
            Height = itemsHeight + WINDOW_OFFSET;
        }

        public void SetBackgroundColor(Color c)
        {
            backgroundBorder.Background = new SolidColorBrush(c);
        }

        private void dragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private const int UNIT_HEIGHT = 55;
        private const int WINDOW_OFFSET = 68;
        private const int MIN_LIST_HEIGHT = 3 * UNIT_HEIGHT;

        private IQuickMainWindowHandler m_handler;
    }
}
