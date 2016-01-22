using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Froser.Quick
{
    public interface IQuickWindow
    {
        string GetPluginsPath();
        void SetQueryText(string text);
        string GetQueryText();
        string GetArgument();
        RichTextBox GetQueryTextBox();
        void ReplaceMethods(IQuickPluginMethod[] methods);
        void ResetMethods();
        void Refresh(int selectIndex);
        int GetCurrentPage();
        void LockWindow();
        void UnlockWindow();
        void AsyncInvoke(Action action);
    }
}
