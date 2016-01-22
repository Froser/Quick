using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Froser.Quick.Plugins.Calc
{
    internal class QuickCalcCache
    {
        public double GetValue()
        {
            return m_value;
        }

        public void SetValue(double value)
        {
            m_value = value;
            m_cached = true;
        }

        public bool HasCache()
        {
            return m_cached;
        }

        public void ClearCache()
        {
            m_cached = false;
        }

        private bool m_cached = false;
        private double m_value;
    }
}
