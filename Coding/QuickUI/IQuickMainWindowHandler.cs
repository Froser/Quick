using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Froser.Quick.UI
{
    public interface IQuickMainWindowHandler
    {
        void Init();
        void SetHostWindow(QuickMainWindow host);
        void OnClosing(object sender, CancelEventArgs e);
        void OnDeactivated(object sender, EventArgs e);
        void OnActivated(object sender, EventArgs e);
        void OnKeyDown(object sender, KeyEventArgs e);
        void OnMouseWheel(object sender, MouseWheelEventArgs e);
        void OnTextChanged(object sender, TextChangedEventArgs e);
        void OnListItemClicked(object sender, EventArgs e);
        void OnLockedClick(object sender, MouseButtonEventArgs e);
        void OnListPageUp();
        void OnListPageDown();
        void OnShowing();
        void OnShowed();
    }
}
