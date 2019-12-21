using System.Windows.Forms;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;

namespace Froser.Quick
{
    public static class Extension
    {
        //判断某字符串是否按顺序含有子字符串，如abcd含有ac但是不含有ca
        public static bool HasString(this String str, String substr)
        {
            //包含在{}中的字符表示，不作为查找依据。例如，{x, y}中，忽略x, y这几个字符，当然{}本身也不包含在内
            Regex brace = new Regex(@"\{(.*?)\}");
            //先统一大小写
            str = str.ToLower();
            substr = substr.ToLower();

            bool result = false;
            String rec_str = brace.Replace(str, ""); 
            for (int i = 0; i < substr.Length; i++)
            {
                int index = rec_str.IndexOf(substr[i]);
                if (index < 0)
                {
                    return false;
                }
                else
                {
                    if (rec_str.Length == 1) return ( i == substr.Length - 1);
                    rec_str = rec_str.Substring(index + 1);
                    result = true;
                }
            }

            return result;
        }

        public static int Plus(this object lhs, object addend)
        {
            if (lhs is int)
            {
                if (addend is int)
                {
                    return (int)lhs + (int)addend;
                }
                else if (addend is float || addend is double)
                {
                    return (int)((int)lhs + double.Parse(addend.ToString()));
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else if (lhs is float || lhs is double)
            {
                if (addend is int)
                {
                    return (int)(double.Parse (lhs.ToString ()) + (int)addend);
                }
                else if (addend is float || addend is double)
                {
                    return (int)(double.Parse(lhs.ToString()) + double.Parse(addend.ToString()));
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static void ShowBalloonTip(this NotifyIcon notify, String text)
        {
            notify.ShowBalloonTip(500, System.Windows.Forms.Application.ProductName, text, ToolTipIcon.Info);
        }

        public static void Swap(this IList list, object a, object b)
        {
            int indexOfB = list.IndexOf (b);
            list.Insert(list.IndexOf (a), b);
            list.Remove(a);
            list.Remove(b);
            list.Insert(indexOfB, a);
        }

        public static string KeyToString(this Key key)
        {
            switch (key.ToString().ToLower())
            {
                case "oem3":
                    return "`";
                default:
                    return key.ToString();
            }
        }

        public static bool ContainsKey<T>(this Dictionary<String, T> dic, String key, bool ignoreCase, out string keyInModel)
        {
            keyInModel = "";
            foreach (var item in dic.Keys)
            {
                if (String.Compare(item, key, ignoreCase) == 0) {
                    keyInModel = item;
                    return true;
                }
            }
            return false;
        }

        public static BitmapSource ToBitmapSource(this Icon icon)
        {
            var bitmap = icon.ToBitmap();
            var hbitmap = bitmap.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
