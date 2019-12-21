using Froser.Quick.Plugins.Calc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Froser.Quick.Plugins
{
    internal class QuickCalc : IQuickPluginMethod
    {
        // 接口部分
        public virtual string GetName()
        {
            return "计算(calc)[jisuan]";
        }

        public virtual string GetDescription(IQuickWindow quickWindow)
        {
            if (!GetCache().HasCache())
                return "输入算式进行计算，回车粘贴到剪贴板";
            return GetCache().GetValue().ToString();
        }

        public virtual void Invoke(object sender, IQuickWindow quickWindow)
        {
            Process.Start("calc.exe");
        }

        public bool AcceptArgs()
        {
            return true;
        }

        public string AvailableApplicationName()
        {
            return null;
        }

        public virtual bool GetIcon(IQuickWindow quickWindow, out ImageSource icon)
        {
            icon = null;
            return false;
        }

        public void KeyDown(IQuickWindow quickWindow, KeyEventArgs e)
        {
        }

        public void TextChanged(IQuickWindow quickWindow, TextChangedEventArgs e)
        {
            var arg = quickWindow.GetArgument ();
            if (arg == null)
            {
                GetCache().ClearCache();
                quickWindow.ResetMethods();
                return;
            }

            CalcResult(quickWindow);

            QuickCalcItemType[] types = { QuickCalcItemType.ToDecimal, 
                                            QuickCalcItemType.ToHex, 
                                            QuickCalcItemType.ToBinary,
                                        };
            List<QuickCalcItem> calcItemList = new List<QuickCalcItem>();
            foreach (var type in types)
            {
                QuickCalcItem item = new QuickCalcItem(GetRoot(), type);
                calcItemList.Add(item);
            }
            if (calcItemList.Count > 0)
                quickWindow.ReplaceMethods(calcItemList.ToArray());
        }

        public virtual QuickCalc GetRoot()
        {
            return this;
        }

        public virtual QuickCalcCache GetCache()
        {
            return m_cache;
        }

        private void CalcResult(IQuickWindow quickWindow)
        {
            QuickCalcScanner calc = new QuickCalcScanner();
            if (quickWindow != null)
            {
                var arg = quickWindow.GetArgument();
                if (arg != null)
                {
                    try
                    {
                        var result = calc.Eval(arg);
                        GetCache().SetValue (result);
                    }
                    catch { }
                    return;
                }
            }
            GetCache().ClearCache();
        }

        public void Closed(IQuickWindow quickWindow)
        {
            GetCache().ClearCache();
            quickWindow.Refresh(0);
        }

        public void PageDown(IQuickWindow quickWindow)
        {
        }

        public void PageUp(IQuickWindow quickWindow)
        {
        }

        private QuickCalcCache m_cache = new QuickCalcCache();
    }

}
