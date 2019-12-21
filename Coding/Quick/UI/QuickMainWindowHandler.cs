using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Froser.Quick.UI
{
    internal class QuickMainWindowHandler : IQuickMainWindowHandler, IQuickWindow
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        public QuickMainWindowHandler(object comObject, QuickModel model, QuickContext context, IntPtr hostPtr)
        {
            m_comObject = comObject;
            m_model = model;
            m_hostPtr = hostPtr;
            m_context = context;
            m_utilities = new Utilities(comObject);
            m_defaultIcon = null;
            m_bitmapSourceCache = null;
            m_isGlobalModel = true;
            m_locked = false;
        }

        ~QuickMainWindowHandler()
        {
            ReleaseObject();
        }

        public QuickMainWindowHandler(object comObject, QuickModel model, QuickContext context, IntPtr hostPtr, bool isGlobalModel, Process currentProcess)
            : this(comObject, model, context, hostPtr)
        {
            m_isGlobalModel = isGlobalModel;
            if (!isGlobalModel)
            {
                try
                {
                    var icon = Icon.ExtractAssociatedIcon(currentProcess.MainModule.FileName);     //获得主线程图标
                    if (icon != null)
                        SetDefaultIcon(icon);
                }
                catch
                {
                }
            }
        }

        public void Init()
        {
            UpdateLockImage();
        }

        public void OnLockedClick(object sender, MouseButtonEventArgs e)
        {
            var config = QuickConfig.ThisConfig;
            config.LockWindow = !config.LockWindow;
            UpdateLockImage();
            config.TrySave();
        }

        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            try
            {
                m_hostWindow.Hide();

                BlockTextBoxChangedEvent();
                NotifyPluginOnClosed();
                m_hostWindow.GetQueryTextBox().Document.Blocks.Clear();
                m_context.ClearSubModel();

                FilterList("");
                ResetPage();
                RefreshList();
            }
            catch { }
            finally
            {
                UnblockTextBoxChangedEvent();
            }

        }

        public void OnShowing()
        {
            m_hostWindow.ResetOpacity();

            if (m_bFirstLoad)
                FirstLoad();
            else
                AdjustPosition();
            m_hostWindow.GetQueryTextBox().Focus();
        }

        public void OnShowed()
        {
            if (m_bFirstLoad)
            {
                AdjustPosition();
                m_bFirstLoad = false;
            }
        }

        public void OnDeactivated(object sender, EventArgs e)
        {
            bool globalLock = QuickConfig.ThisConfig.LockWindow;
            if (!m_locked && !globalLock)
                JobDone();
            else
                m_hostWindow.SetOpacity(0.3d);
        }

        public void OnActivated(object sender, EventArgs e)
        {
            m_hostWindow.ResetOpacity();
        }

        public void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int idx = GetSelectedIndex();
            if (idx >= 0)
            {
                var plugin = m_availableMethods[idx].GetPluginInterface();
                if (plugin != null)
                    plugin.KeyDown(this, e);
            }

            switch (e.Key)
            {
                case Key.Enter:
                    InvokeCommand();
                    break;
                case Key.Down:
                    m_hostWindow.SelectNext();
                    break;
                case Key.Up:
                    m_hostWindow.SelectPrevious();
                    break;
                case Key.PageDown:
                    PageDown();
                    e.Handled = true;
                    break;
                case Key.PageUp:
                    PageUp();
                    e.Handled = true;
                    break;
                case Key.End:
                    {
                        int itemCount = m_hostWindow.GetList().Items.Count;
                        if (itemCount > 0)
                            m_hostWindow.Select(itemCount - 1);
                    }
                    break;
                case Key.Home:
                    {
                        int itemCount = m_hostWindow.GetList().Items.Count;
                        if (itemCount > 0)
                            m_hostWindow.Select(0);
                    }
                    break;
                case Key.Escape:
                    JobDone();
                    break;
            }
        }

        public void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
        }

        public void OnTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!m_textboxSelectionChangedBlock)
            {
                FilterList(GetQueryText());
                ResetPage();
                RefreshList();

                // 全局Model下要筛选下，拿到正确的Method
                m_hostWindow.GetList().SelectedIndex = 0;
                var currentMethod = GetSelectedMethod();
                if (currentMethod != null)
                {
                    var plugin = currentMethod.GetPluginInterface();
                    if (plugin != null)
                    {
                        plugin.TextChanged(this, e);
                        ResetPage();
                        return;
                    }
                }
            }
        }

        public void OnListItemClicked(object sender, EventArgs e)
        {
            InvokeCommand();
        }

        public void OnListPageUp()
        {
            if (PageUp())
                m_hostWindow.Select(m_hostWindow.GetList().Items.Count - 1);
        }

        public void OnListPageDown()
        {
            if (PageDown())
                m_hostWindow.Select(0);
        }

        public void SetHostWindow(QuickMainWindow host)
        {
            m_hostWindow = host;
        }

        public void BlockTextBoxChangedEvent()
        {
            m_textboxSelectionChangedBlock = true;
        }

        public void UnblockTextBoxChangedEvent()
        {
            m_textboxSelectionChangedBlock = false;
        }

        public string GetQueryText()
        {
            var textbox = m_hostWindow.GetQueryTextBox();
            TextRange txRange = new TextRange(textbox.Document.ContentStart, textbox.Document.ContentEnd);
            return txRange.Text.Trim();
        }

        public void SetQueryText(string text)
        {
            var textbox = m_hostWindow.GetQueryTextBox();
            BlockTextBoxChangedEvent();
            textbox.Document.Blocks.Clear();
            UnblockTextBoxChangedEvent();
            var para = new Paragraph();
            Run r = new Run(text);
            para.Inlines.Add(r);
            textbox.Document.Blocks.Add(para);
        }

        public RichTextBox GetQueryTextBox()
        {
            return m_hostWindow.GetQueryTextBox();
        }

        public string GetArgument()
        {
            return GetArgumentString(GetQueryText());
        }

        public void ReplaceMethods(IQuickPluginMethod[] methods)
        {
            QuickModel model = new QuickModel(m_model);
            foreach (var m in methods)
            {
                QuickMethod qm = new QuickMethod();
                qm.Application = m.AvailableApplicationName();
                qm.MethodDefArgs = " ";
                qm.MethodParamRegex = ".";     //一定可以接受参数
                qm.MethodDescription = m.GetDescription(this);
                qm.MethodName = m.GetName();
                qm.SetAdditionMethod(m);
                model.MethodList.Add(qm);
            }
            m_context.ReplaceSubModel(model);
            FilterList(GetQueryText());
            ResetPage();
            RefreshList();
        }

        public void ResetMethods()
        {
            m_context.ClearSubModel();
            FilterList(GetQueryText());
            ResetPage();
            RefreshList();
        }

        public void SetDefaultIcon(Icon icon)
        {
            m_defaultIcon = icon;
            m_bitmapSourceCache = icon.ToBitmapSource();
        }

        public string GetPluginsPath()
        {
            return QuickPluginLoader.PLUGINS_PATH;
        }

        public int GetCurrentPage()
        {
            return m_page;
        }

        public void Refresh(int selectIndex)
        {
            FilterList(GetQueryText());
            RefreshList();
            var list = m_hostWindow.GetList();
            int count = list.Items.Count;
            if (count > 0)
            {
                if (selectIndex < 0)
                    selectIndex = 0;
                else if (selectIndex >= count)
                    m_hostWindow.Select(count - 1);
                list.SelectedIndex = selectIndex;
            }
        }

        public void LockWindow()
        {
            m_locked = true;
        }

        public void UnlockWindow()
        {
            m_locked = false;
        }

        public void AsyncInvoke(Action action)
        {
            m_hostWindow.Dispatcher.BeginInvoke(action);
        }

        private void AdjustPosition()
        {
            if (!m_isGlobalModel)
            {
                try
                {
                    m_hostWindow.Left = (int)(QuickSafeReflection.Invoke(m_model.Left, m_comObject).Plus(QuickSafeReflection.Invoke(m_model.Width, m_comObject)) - 60);
                    m_hostWindow.Top = (int)(QuickSafeReflection.Invoke(m_model.Top, m_comObject).Plus(0));
                }
                catch
                {
                    foreach (var name in m_model.ProgramName.Split(','))
                    {
                        try
                        {
                            m_comObject = Marshal.GetActiveObject(name);
                            m_hostWindow.Left = (int)(QuickSafeReflection.Invoke(m_model.Left, m_comObject).Plus(QuickSafeReflection.Invoke(m_model.Width, m_comObject)) - 60);
                            m_hostWindow.Top = (int)(QuickSafeReflection.Invoke(m_model.Top, m_comObject).Plus(0));
                            break;
                        }
                        catch (Exception ex)
                        {
                            QuickVitality.UpdateVitality("error", m_model.ProgramName, "GetActiveObject error. " + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                        }
                    }
                }
            }
            else
            {
                var commonObj = m_comObject as QuickCommonObject;
                if (m_comObject == null)
                {
                    throw new Exception("公共对象并非CommonObject");
                }
                else
                {
                    m_hostWindow.Left = commonObj.GetWindowRect().Left + (commonObj.GetWindowRect().Right - commonObj.GetWindowRect().Left - m_hostWindow.Width) / 2;
                    m_hostWindow.Top = commonObj.GetWindowRect().Top + (commonObj.GetWindowRect().Bottom - commonObj.GetWindowRect().Top - m_hostWindow.Height) / 2;
                }
            }
        }

        private void NotifyPluginOnClosed()
        {
            foreach (var method in m_model.MethodList)
            {
                var plugin = method.GetPluginInterface();
                if (plugin != null)
                    plugin.Closed(this);
            }
        }

        private QuickMethod GetSelectedMethod()
        {
            int idx = GetSelectedIndex();
            if (m_availableMethods.Count > 0 && idx >= 0)
                return m_availableMethods[idx];
            return null;
        }

        private int GetMaxPage()
        {
            // 获得从0开始算起的最大页码编号
            int totalCount = m_availableMethods.Count;
            int max = (totalCount - 1) / MAX_DISPLAY_ACTION;
            return max < 0 ? 0 : max;
        }

        private bool PageUp()
        {
            bool result = false;
            if (m_page > 0)
            {
                m_page--;
                RefreshList();
                result = true;
            }

            var plugin = GetSelectedMethod().GetPluginInterface();
            if (plugin != null)
                plugin.PageUp(this);
            return result;
        }

        private bool PageDown()
        {
            bool result = false;
            if (m_page < GetMaxPage())
            {
                m_page++;
                RefreshList();
                result = true;
            }

            var plugin = GetSelectedMethod().GetPluginInterface();
            if (plugin != null)
                plugin.PageDown(this);
            return result;
        }

        private void JobDone()
        {
            m_hostWindow.Close();
        }

        private void FirstLoad()
        {
            ResetPage();
            FilterList("");
            RefreshList();
        }

        private void ResetPage()
        {
            m_page = 0;
        }

        private void FilterList(String queryText)
        {
            // 将符合条件的选项筛选进available list
            m_availableMethods = new List<QuickMethod>();
            var subModel = m_context.GetSubModel();
            bool isSubModel = (subModel != null);
            QuickModel currentModel = subModel ?? m_model;

            foreach (QuickMethod methodItem in currentModel.MethodList)
            {
                string query = queryText;
                bool hasParams = false;
                Regex reg_params = GetArgumentRegex(methodItem.MethodParamRegex);
                Match m = reg_params.Match(query);

                if (m.Success)
                {
                    query = reg_params.Replace(query, "").Trim(); //表示取出参数后的字符串
                    hasParams = true;
                }
                else
                {
                    hasParams = false;
                }

                if (query.Trim() == "" || methodItem.MethodName.Replace(" ", "").HasString(query) || isSubModel)
                {
                    if (hasParams)
                    {
                        //滤过不含参数的指令。含有参数的方法，其正则表达式不为空
                        if (methodItem.MethodParamRegex == "")
                        {
                            continue;
                        }
                    }
                    m_availableMethods.Add(methodItem);
                }
            }
        }

        private void RefreshList()
        {
            var list = m_hostWindow.GetList();
            list.Items.Clear();
            for (int i = 0; i < MAX_DISPLAY_ACTION; i++)
            {
                int idx = m_page * MAX_DISPLAY_ACTION + i;
                if (idx == m_availableMethods.Count)
                    break;

                var currentMethod = m_availableMethods[idx];
                string title = Regex.Replace(currentMethod.MethodName, REG_TITLE_RULES, "");
                string description = null;
                ImageSource icon = null;
                IQuickPluginMethod plugin = currentMethod.GetPluginInterface();
                // 如果是一个加载项，可以调用其函数获取
                if (plugin != null)
                {
                    description = plugin.GetDescription(this);
                    ImageSource tmp = null;
                    bool needIcon = plugin.GetIcon(this, out tmp);
                    if (needIcon)
                        icon = tmp;
                    else
                        icon = QuickUIResource.GetDefaultPluginIcon();
                }
                else
                {
                    description = currentMethod.MethodDescription;
                    if (!m_isGlobalModel) //非全局的情况下，使用当前应用程序的图标
                        icon = m_bitmapSourceCache;
                }

                QuickListItem item = new QuickListItem(title, description, icon);
                item.CreateListBoxItemTo(m_hostWindow.GetList());
            }
            if (GetSelectedIndex() == -1 && m_hostWindow.GetList().HasItems)
                m_hostWindow.GetList().SelectedIndex = 0;

            AutoResize();
        }

        private void AutoResize()
        {
            m_hostWindow.AutoResize();
        }

        private String GetArgumentString(String queryText)
        {
            String methodParamRegex = GetSelectedIndex() == -1 ? "." : m_availableMethods[GetSelectedIndex()].MethodParamRegex;
            Regex reg_params = GetArgumentRegex(methodParamRegex);
            Match m = reg_params.Match(queryText);

            if (m.Success)
                return m.Result("${PARAMS}");
            return null;
        }

        private int GetSelectedIndex()
        {
            return m_hostWindow.GetList().SelectedIndex;
        }

        private void InvokeCommand()
        {
            int idx = GetSelectedIndex();
            var list = m_hostWindow.GetList();
            if (idx == -1)
            {
                if (!list.HasItems)
                    return;
                idx = 0;
            }

            QuickMethod method = m_availableMethods[idx];
            String arguments = GetArgumentString(GetQueryText());
            String realMethod = "invalid";
            try
            {
                String[] args = arguments == null ? QuickModel.GetArguments(method.MethodDefArgs) : QuickModel.GetArguments(arguments);
                if (method.GetPluginInterface() != null)
                {
                    method.GetPluginInterface().Invoke(m_comObject, this);
                }
                else if (method.MethodParamRegex == "" || method.MethodParamRegex == "." || args.Length > 0)
                {
                    //将参数塞进MethodScript中
                    if (method.MethodScript.Trim()[0] == '!')
                    {
                        //!表示对布尔值进行切换
                        realMethod = method.MethodScript.Trim().Substring(1);
                        object value = QuickSafeReflection.Invoke(realMethod, m_comObject);
                        if (value is bool)
                        {
                            QuickSafeReflection.Set(realMethod, !(bool)value, m_comObject);
                        }
                    }
                    else if (method.MethodScript.Trim()[0] == '@')
                    {
                        String utilityMethod = null;
                        //如果是@开头，表示调用Utilities中方法来赋值或将返回值传给方法中的{$1}
                        //可能存在!在引号内的风险
                        if (method.MethodScript.Contains("!"))
                        {
                            utilityMethod = method.MethodScript.Substring(1, method.MethodScript.IndexOf('!') - 1);
                            utilityMethod = ReplaceAruguments(utilityMethod, args);
                            realMethod = method.MethodScript.Substring(method.MethodScript.IndexOf('!') + 1);

                            if (method.MethodScript.Trim()[method.MethodScript.Length - 1] != ')')
                            {
                                QuickSafeReflection.Set(realMethod, QuickSafeReflection.Invoke(utilityMethod, m_utilities), m_comObject);
                            }
                            else
                            {
                                realMethod = ReplaceAruguments(realMethod, (String[])QuickSafeReflection.Invoke(utilityMethod, m_utilities));
                                QuickSafeReflection.Invoke(realMethod, m_comObject);
                            }
                        }
                        else
                        {
                            utilityMethod = method.MethodScript.Substring(1);
                            realMethod = ReplaceAruguments(utilityMethod, args);
                            QuickSafeReflection.Invoke(realMethod, m_utilities);
                        }
                    }
                    else if (method.MethodScript.Trim()[method.MethodScript.Length - 1] != ')')
                    {
                        //如果不以)结尾，表示是一个赋值方法
                        realMethod = method.MethodScript;
                        QuickSafeReflection.Set(realMethod, args[0], m_comObject);
                    }
                    else
                    {
                        realMethod = ReplaceAruguments(method.MethodScript, args);
                        QuickSafeReflection.Invoke(realMethod, m_comObject);
                    }
                }

                QuickVitality.UpdateVitality("invoke", m_model.ProgramName, realMethod);
            }
            catch (Exception ex)
            {
                //在此插入异常
                QuickVitality.UpdateVitality("error", m_model.ProgramName, "invoking: " + realMethod + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            JobDone();
        }

        private Regex GetArgumentRegex(string methodParamRegex)
        {
            // 获得参数匹配的正则表达式
            return new Regex(@"\s+?(?<PARAMS>(" + methodParamRegex + @"|,|\s)+)(\s*$)");
        }

        private string ReplaceAruguments(string script, string[] replacement)
        {
            //将{$1}、{$2}、{$n}以实际的参数代替
            //将{$_}替换为，以引号包围、逗号分隔的长串字符
            //将{$!}替换为，"replacement[0]","replacement[rest]"形式的字符串
            if (replacement == null) return script;
            StringBuilder stringArgs = new StringBuilder();
            if (script.Contains("{$_}"))
            {
                foreach (var arg in replacement)
                {
                    stringArgs.AppendFormat("\"{0}\",", arg);
                }
                stringArgs.Remove(stringArgs.Length - 1, 1);
                script = script.Replace("{$_}", stringArgs.ToString());
            }
            else if (script.Contains("{$!}"))
            {
                bool prefix = false;
                for (int i = 0; i < replacement.Length; i++)
                {
                    if (i == 0)
                    {
                        stringArgs.Append("\"" + replacement[i] + "\",\"");
                    }
                    else
                    {
                        stringArgs.Append(replacement[i] + " ");
                        if (!prefix) prefix = true;
                    }
                }
                if (prefix) stringArgs.Remove(stringArgs.Length - 1, 1);
                stringArgs.Append("\"");
                script = script.Replace("{$!}", stringArgs.ToString());
            }

            for (int i = 0; i < replacement.Length; i++)
            {
                string token = "{$" + (i + 1) + "}";
                if (script.Contains(token))
                {
                    script = script.Replace(token, replacement[i]);
                }
            }
            return script;
        }

        private ImageSource GetWindowLockImage(bool locked)
        {
            return locked
                ? QuickUIResource.GetLockedIcon()
                : QuickUIResource.GetUnlockedIcon();
        }

        private void UpdateLockImage()
        {
            var lockedButton = m_hostWindow.GetLockImageButton();
            bool windowLocked = QuickConfig.ThisConfig.LockWindow;
            lockedButton.Source = GetWindowLockImage(windowLocked);
        }

        /// <summary>
        /// 彻底释放COM对象，防止进程残留
        /// </summary>
        private void ReleaseObject()
        {
            if (!m_isGlobalModel)
            {
                try
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(m_comObject);
                    GC.Collect();
                }
                catch { }
            }
        }

        private const string REG_TITLE_RULES = @"\[.*?\]|\{|\}";     //标题规范 [] {}，[]中的内容不会显示但是会被检索，{}中的内容会被显示但是不会被检索
        private const int MAX_DISPLAY_ACTION = 10;

        private bool m_bFirstLoad = true;
        private QuickMainWindow m_hostWindow;
        private object m_comObject;
        private QuickContext m_context;
        private QuickModel m_model;
        private IntPtr m_hostPtr;
        private Utilities m_utilities;
        private List<QuickMethod> m_availableMethods;    //第一部分为QuickMethod，第二部分为参数
        private int m_page;
        private bool m_textboxSelectionChangedBlock = false;

        private Icon m_defaultIcon;
        private BitmapSource m_bitmapSourceCache;
        private bool m_isGlobalModel;
        private bool m_locked;
    }
}
