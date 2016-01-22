using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Froser.Quick.UI
{
    public interface IQuickPreferenceWindowHandler
    {
        void SetHost(QuickPreferenceWindow host);
        void LoadConfig();
        void ShortcutTextBoxOnPreviewKeyDown(object sender, KeyEventArgs e);
        void OnSave();
        void OnTemplateSelected(int index);
        void OnTemplateMethodSelect(int index);
        void OnGeneralDefault();
        void OnSaveMethod();
        void OnRestoreMethod();
        void OnMoveUpMethod();
        void OnMoveDownMethod();
        void OnCreateNewMethod();
        void OnDeleteMethod();
        void OnCreateNewContext();
        void OnRemoveContext();
        void OnModifyContext();
    }
}
