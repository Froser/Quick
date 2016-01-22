using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Froser.Quick.UI
{
    public class QuickPreferenceWindowHandler : IQuickPreferenceWindowHandler
    {
        public void LoadConfig()
        {
            LoadGeneral();
            LoadTemplate();
            LoadVersion();
        }

        public void SetHost(QuickPreferenceWindow host)
        {
            m_host = host;
        }

        public void OnSave()
        {
            SaveGeneral();
            QuickConfig.ThisConfig.TrySave();
            QuickListener.Listener.Reload();
        }

        public void OnGeneralDefault()
        {
            QuickConfig.ThisConfig.SetDefaultConfig();
            LoadGeneral();
        }

        public void OnSaveMethod()
        {
            var list = m_host.GetMethodsListInTemplate();
            var item = (QuickMethod)list.SelectedItem;
            if (item != null)
            {
                var methodName = m_host.GetMethodNameTextBoxInTemplate();
                var methodDescription = m_host.GetMethodDescriptionTextBoxInTemplate();
                var methodScript = m_host.GetMethodScriptTextBoxInTemplate();
                var methodDefArg = m_host.GetMethodDefaultArgumentTextBoxInTemplate();
                var methodRegex = m_host.GetMethodRegexTextBoxInTemplate();
                var filename = ((FrameworkElement)m_host.GetTemplateListInTemplate().SelectedItem).Tag.ToString();

                item.MethodName = methodName.Text;
                item.MethodDescription = methodDescription.Text;
                item.MethodScript = methodScript.Text;
                item.MethodDefArgs = methodDefArg.Text;
                item.MethodParamRegex = methodRegex.Text;
                item.MethodPriority = 0;

                SaveModel();
            }
        }

        private void SaveModel()
        {
            var filename = ((FrameworkElement)m_host.GetTemplateListInTemplate().SelectedItem).Tag.ToString();
            try
            {
                m_currentModel.Save(filename);
                QuickListener.Listener.Reload();
            }
            catch (Exception)
            {
                // TODO: 在这里处理异常
            }
        }

        private void ClearTextBoxInTemplate()
        {
            var methodName = m_host.GetMethodNameTextBoxInTemplate();
            var methodDescription = m_host.GetMethodDescriptionTextBoxInTemplate();
            var methodScript = m_host.GetMethodScriptTextBoxInTemplate();
            var methodDefArg = m_host.GetMethodDefaultArgumentTextBoxInTemplate();
            var methodRegex = m_host.GetMethodRegexTextBoxInTemplate();
            methodName.Clear();
            methodDescription.Clear();
            methodScript.Clear();
            methodDefArg.Clear();
            methodRegex.Clear();
        }

        public void OnCreateNewContext()
        {
            var list = m_host.GetContextMenuInGeneral();
            QuickConfig.ContextMenuItem newItem = new QuickConfig.ContextMenuItem();
            QuickConfig.ThisConfig.ContextMenuList.Add(newItem);
            LoadContextMenu();
        }

        public void OnRemoveContext()
        {
            var list = m_host.GetContextMenuInGeneral();
            object selectedItem = list.SelectedItem;
            if (selectedItem != null)
            {
                list.Items.Remove(selectedItem);
                QuickConfig.ThisConfig.ContextMenuList.Remove((QuickConfig.ContextMenuItem)selectedItem);
                LoadContextMenu();
            }
        }

        public void OnModifyContext()
        {
            var list = m_host.GetContextMenuInGeneral();
            var item = (QuickConfig.ContextMenuItem)list.SelectedItem;
            if (item == null)
                return;

            QuickPerferenceContextEdit editor = new QuickPerferenceContextEdit();
            editor.Init += (() => {
                editor.GetName().Text = item.Name;
                editor.GetBehavior().Text = item.Exec;
                editor.GetArgument().Text = item.Argument;
            });
            editor.Done += (() => {
                item.Name = editor.GetName().Text;
                item.Exec = editor.GetBehavior().Text;
                item.Argument = editor.GetArgument().Text;
                QuickConfig.ThisConfig.TrySave();
                LoadContextMenu();
            });
            editor.Show();
        }

        public void OnRestoreMethod()
        {
            var list = m_host.GetMethodsListInTemplate();
            var item = (QuickMethod)list.SelectedItem;
            if (item != null)
            {
                var methodName = m_host.GetMethodNameTextBoxInTemplate();
                var methodDescription = m_host.GetMethodDescriptionTextBoxInTemplate();
                var methodScript = m_host.GetMethodScriptTextBoxInTemplate();
                var methodDefArg = m_host.GetMethodDefaultArgumentTextBoxInTemplate();
                var methodRegex = m_host.GetMethodRegexTextBoxInTemplate();
                methodName.Text = item.MethodName;
                methodDescription.Text = item.MethodDescription;
                methodScript.Text = item.MethodScript;
                methodDefArg.Text = item.MethodDefArgs;
                methodRegex.Text = item.MethodParamRegex;
            }
        }

        public void OnMoveUpMethod()
        {
            var list = m_host.GetMethodsListInTemplate();
            var item = list.SelectedItem;
            if (item != null)
            {
                int idx = list.SelectedIndex;
                if (idx > 0)
                {
                    m_currentModel.MethodList.Swap(list.Items[idx], list.Items[idx - 1]);
                    list.Items.Swap(list.Items[idx], list.Items[idx - 1]);
                    list.SelectedIndex = idx - 1;
                    SaveModel();
                }
            }
        }

        public void OnMoveDownMethod()
        {
            var list = m_host.GetMethodsListInTemplate();
            var item = list.SelectedItem;
            if (item != null)
            {
                int idx = list.SelectedIndex;
                if (idx < list.Items.Count - 1)
                {
                    m_currentModel.MethodList.Swap(list.Items[idx], list.Items[idx + 1]);
                    list.Items.Swap(list.Items[idx], list.Items[idx + 1]);
                    list.SelectedIndex = idx + 1;
                    SaveModel();
                }
            }
        }

        public void OnCreateNewMethod()
        {
            var list = m_host.GetMethodsListInTemplate();
            var item = list.SelectedItem;
            var method = new QuickMethod();
            if (item != null)
            {
                m_currentModel.MethodList.Insert(list.SelectedIndex, method);
                list.Items.Insert(list.SelectedIndex, method);
            }
            else
            {
                m_currentModel.MethodList.Add(method);
                list.Items.Insert(list.Items.Count, new QuickMethod());
            }
            SaveModel();
        }

        public void OnDeleteMethod()
        {
            var list = m_host.GetMethodsListInTemplate();
            var item = list.SelectedItem;
            if (item != null)
            {
                var method = new QuickMethod();
                m_currentModel.MethodList.Remove((QuickMethod)item);
                list.Items.Remove(item);
                SaveModel();
            }
        }

        public void OnTemplateSelected(int index)
        {
            if (index < 0)
                return;

            var item = m_host.GetTemplateListInTemplate().Items[index];
            string filename = ((FrameworkElement)item).Tag.ToString();
            var methodsList = m_host.GetMethodsListInTemplate();
            m_currentModel = QuickModel.GetModel(filename);
            methodsList.Items.Clear();
            foreach (var i in m_currentModel.MethodList)
            {
                methodsList.Items.Add(i);
            }
        }

        private void LoadGeneral()
        {
            TextBox quickKey = m_host.GetQuickTextBoxInGeneral();
            CheckBox quickShift = m_host.GetQuickShiftInGeneral();
            CheckBox quickCtrl = m_host.GetQuickCtrlInGeneral();
            CheckBox quickAlt = m_host.GetQuickAltInGeneral();
            CheckBox quickWin = m_host.GetQuickWinInGeneral();

            quickKey.Tag = QuickConfig.ThisConfig.QuickHotKey;
            quickKey.Text = QuickConfig.ThisConfig.QuickHotKey.KeyToString();
            if ((QuickConfig.ThisConfig.QuickHotKeyFlags & (int)Hotkey.KeyFlags.MOD_ALT) != 0)
                quickAlt.IsChecked = true;
            if ((QuickConfig.ThisConfig.QuickHotKeyFlags & (int)Hotkey.KeyFlags.MOD_CONTROL) != 0)
                quickCtrl.IsChecked = true;
            if ((QuickConfig.ThisConfig.QuickHotKeyFlags & (int)Hotkey.KeyFlags.MOD_SHIFT) != 0)
                quickShift.IsChecked = true;
            if ((QuickConfig.ThisConfig.QuickHotKeyFlags & (int)Hotkey.KeyFlags.MOD_WIN) != 0)
                quickWin.IsChecked = true;

            // ----
            TextBox contextKey = m_host.GetContextTextBoxInGeneral();
            CheckBox contextShift = m_host.GetContextShiftInGeneral();
            CheckBox contextCtrl = m_host.GetContextCtrlInGeneral();
            CheckBox contextAlt = m_host.GetContextAltInGeneral();
            CheckBox contextWin = m_host.GetContextWinInGeneral();
            CheckBox useContext = m_host.GetUseContext();

            contextKey.Tag = QuickConfig.ThisConfig.ContextMenuHotKey;
            contextKey.Text = QuickConfig.ThisConfig.ContextMenuHotKey.KeyToString();
            if ((QuickConfig.ThisConfig.ContextMenuHotKeyFlags & (int)Hotkey.KeyFlags.MOD_ALT) != 0)
                contextAlt.IsChecked = true;
            if ((QuickConfig.ThisConfig.ContextMenuHotKeyFlags & (int)Hotkey.KeyFlags.MOD_CONTROL) != 0)
                contextCtrl.IsChecked = true;
            if ((QuickConfig.ThisConfig.ContextMenuHotKeyFlags & (int)Hotkey.KeyFlags.MOD_SHIFT) != 0)
                contextShift.IsChecked = true;
            if ((QuickConfig.ThisConfig.ContextMenuHotKeyFlags & (int)Hotkey.KeyFlags.MOD_WIN) != 0)
                contextWin.IsChecked = true;

            useContext.IsChecked = QuickConfig.ThisConfig.ContextMenuToogle;

            // ---
            LoadContextMenu();
        }

        private void LoadContextMenu()
        {
            var contextMenu = m_host.GetContextMenuInGeneral();
            contextMenu.Items.Clear();
            var coreList = QuickConfig.ThisConfig.ContextMenuList;
            foreach (var i in coreList)
                contextMenu.Items.Add(i);
        }

        private void SaveGeneral()
        {
            TextBox quickKey = m_host.GetQuickTextBoxInGeneral();
            CheckBox quickShift = m_host.GetQuickShiftInGeneral();
            CheckBox quickCtrl = m_host.GetQuickCtrlInGeneral();
            CheckBox quickAlt = m_host.GetQuickAltInGeneral();
            CheckBox quickWin = m_host.GetQuickWinInGeneral();
            Key qkey = (Key)quickKey.Tag;
            Hotkey.KeyFlags quickFlags = Hotkey.KeyFlags.MOD_NONE;
            if (quickAlt.IsChecked.HasValue && quickAlt.IsChecked.Value) quickFlags |= Hotkey.KeyFlags.MOD_ALT;
            if (quickCtrl.IsChecked.HasValue && quickCtrl.IsChecked.Value) quickFlags |= Hotkey.KeyFlags.MOD_CONTROL;
            if (quickShift.IsChecked.HasValue && quickShift.IsChecked.Value) quickFlags |= Hotkey.KeyFlags.MOD_SHIFT;
            if (quickWin.IsChecked.HasValue && quickWin.IsChecked.Value) quickFlags |= Hotkey.KeyFlags.MOD_WIN;

            // ----
            TextBox contextKey = m_host.GetContextTextBoxInGeneral();
            CheckBox contextShift = m_host.GetContextShiftInGeneral();
            CheckBox contextCtrl = m_host.GetContextCtrlInGeneral();
            CheckBox contextAlt = m_host.GetContextAltInGeneral();
            CheckBox contextWin = m_host.GetContextWinInGeneral();
            CheckBox useContext = m_host.GetUseContext();

            Key ckey = (Key)contextKey.Tag;
            Hotkey.KeyFlags searchFlags = Hotkey.KeyFlags.MOD_NONE;
            if (contextAlt.IsChecked.HasValue && contextAlt.IsChecked.Value) searchFlags |= Hotkey.KeyFlags.MOD_ALT;
            if (contextCtrl.IsChecked.HasValue && contextCtrl.IsChecked.Value) searchFlags |= Hotkey.KeyFlags.MOD_CONTROL;
            if (contextShift.IsChecked.HasValue && contextShift.IsChecked.Value) searchFlags |= Hotkey.KeyFlags.MOD_SHIFT;
            if (contextWin.IsChecked.HasValue && contextWin.IsChecked.Value) searchFlags |= Hotkey.KeyFlags.MOD_WIN;

            QuickConfig.ThisConfig.QuickHotKey = qkey;
            QuickConfig.ThisConfig.QuickHotKeyFlags = (int)quickFlags;
            QuickConfig.ThisConfig.ContextMenuHotKey = ckey;
            QuickConfig.ThisConfig.ContextMenuHotKeyFlags = (int)searchFlags;
            QuickConfig.ThisConfig.ContextMenuToogle = useContext.IsChecked.HasValue && useContext.IsChecked.Value;
        }

        private void LoadTemplate()
        {
            ClearTextBoxInTemplate();
            var list = m_host.GetTemplateListInTemplate();
            list.Items.Clear();

            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = "全局模板";
                item.Tag = QuickModel.GlobalModelName + ".xml";
                list.Items.Add(item);
            }

            foreach (var i in QuickConfig.ThisConfig.ModelName)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i + ".xml";
                item.Tag = i + ".xml";
                list.Items.Add(item);
            }

            if (list.Items.Count > 0)
                list.SelectedIndex = 0;
        }

        private void LoadVersion()
        {
            var version = m_host.GetVersionTextBlockInAbout();
            StringBuilder str = new StringBuilder();
            str.AppendLine("Quick");
            str.AppendLine(System.Windows.Forms.Application.ProductVersion);
            str.AppendLine();
            str.AppendLine("Froser, 2015年8月21日15:35:35");
            version.Text = str.ToString();
        }

        public void ShortcutTextBoxOnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                e.Key == Key.LWin || e.Key == Key.RWin)
                return;

            TextBox tb = sender as TextBox;
            tb.Tag = e.Key;
            tb.Text = e.Key.KeyToString();
            e.Handled = true;
        }

        public void OnTemplateMethodSelect(int index)
        {
            var methodsList = m_host.GetMethodsListInTemplate();
            var item = (QuickMethod)(methodsList).SelectedItem;
            var methodName = m_host.GetMethodNameTextBoxInTemplate();
            var methodDescription = m_host.GetMethodDescriptionTextBoxInTemplate();
            var methodScript = m_host.GetMethodScriptTextBoxInTemplate();
            var methodDefArg = m_host.GetMethodDefaultArgumentTextBoxInTemplate();
            var methodRegex = m_host.GetMethodRegexTextBoxInTemplate();
            if (item != null)
            {
                methodName.Text = item.MethodName;
                methodDescription.Text = item.MethodDescription;
                methodScript.Text = item.MethodScript;
                methodDefArg.Text = item.MethodDefArgs;
                methodRegex.Text = item.MethodParamRegex;
            }
        }

        private QuickModel m_currentModel;
        private QuickPreferenceWindow m_host;
    }
}
