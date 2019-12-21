using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace Froser.Quick.Plugins.Calc
{
    enum QuickCalcItemType
    {
        ToDecimal,
        ToHex,
        ToBinary,
    }

    internal class QuickCalcItem : QuickCalc
    {
        public QuickCalcItem(QuickCalc parent, QuickCalcItemType type)
        {
            m_parent = parent;
            m_type = type;
        }

        public override string GetName()
        {
            StringBuilder name = new StringBuilder("结果 ");
            switch (m_type)
            {
                case QuickCalcItemType.ToDecimal:
                    break;
                case QuickCalcItemType.ToHex:
                    name.Append("十六进制 ");
                    break;
                case QuickCalcItemType.ToBinary:
                    name.Append("二进制 ");
                    break;
                default:
                    Debug.Assert(false, "无有效类型");
                    break;
            }
            return name.ToString ();
        }

        public override string GetDescription(IQuickWindow quickWindow)
        {
            return Transform(GetCache().GetValue ());
        }

        public override QuickCalc GetRoot()
        {
            return m_parent;
        }

        public override void Invoke(object sender, IQuickWindow quickWindow)
        {
            Clipboard.SetText(Transform(GetCache().GetValue()));
        }

        public override QuickCalcCache GetCache()
        {
            return GetRoot().GetCache();
        }

        private string Transform(double n)
        {
            switch (m_type)
            {
                case QuickCalcItemType.ToDecimal:
                    return n.ToString ();
                case QuickCalcItemType.ToHex:
                    return "0x" + Convert.ToString ((int)n, 16).ToUpper();
                case QuickCalcItemType.ToBinary:
                    return Convert.ToString((int)n, 2);
                default:
                    Debug.Assert(false, "无有效类型");
                    break;
            }
            return n.ToString ();
        }

        private QuickCalc m_parent;
        private QuickCalcItemType m_type;
    }
}
