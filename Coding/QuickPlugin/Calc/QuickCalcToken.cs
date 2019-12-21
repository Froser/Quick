using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Froser.Quick.Plugins.Calc
{
    internal enum QuickCalcToken 
    {
        Reserved,
        Numeric,
        Symbol,
        Error,
    }
}
