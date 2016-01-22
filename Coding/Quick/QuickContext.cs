using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Froser.Quick
{
    internal struct QuickContext
    {
        public void ClearSubModel()
        {
            m_subModel = null;
        }

        public QuickModel GetSubModel()
        {
            return m_subModel;
        }

        public void ReplaceSubModel(QuickModel m)
        {
            m_subModel = m;
        }

        // 有时候进入某些条目，需要替换掉当前的model
        private QuickModel m_subModel;
    }
}
