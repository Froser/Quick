using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Froser.Quick
{
    public static class QuickSafeReflection
    {
        public static void Set(String api, object value, object targetObj)
        {
            try
            {
                QuickReflection.Set(api, value, targetObj);
            }
            catch { }
        }

        public static object Invoke(String api, object targetObj)
        {
            try
            {
                return QuickReflection.Invoke(api, targetObj);
            }
            catch 
            {
                return null;
            }
        }

    }
}
