using System;
namespace Froser.Quick.UI
{
    public interface IQuickContextWindowHandler
    {
        void SetHost(QuickContextWindow host);
        void Init();
        void AfterShow();
        void BeforeShow(string context);
        void OnDeactivate(object sender, EventArgs e);
    }
}
