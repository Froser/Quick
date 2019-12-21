using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Froser.Quick
{
    public interface IQuickPlugin
    {
        IQuickPluginMethod[] GetMethods();
    }

    public interface IQuickPluginMethod
    {
        string GetName();
        string GetDescription(IQuickWindow quickWindow);
        string AvailableApplicationName();
        void Invoke(object sender, IQuickWindow quickWindow);
        bool AcceptArgs();
        bool GetIcon(IQuickWindow quickWindow, out ImageSource icon);

        // 事件回调
        // 插件在激活状态下按下一个键时
        void KeyDown(IQuickWindow quickWindow, KeyEventArgs e);
        // 插件在激活状态下搜索文本有变化时
        void TextChanged(IQuickWindow quickWindow, TextChangedEventArgs e);
        // 主窗体关闭时
        void Closed(IQuickWindow quickWindow);
        void PageDown(IQuickWindow quickWindow);
        void PageUp(IQuickWindow quickWindow);
    }
}
