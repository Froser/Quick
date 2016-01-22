using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Froser.Quick
{
    internal static class QuickMultiVersion
    {
#if KSO
        public static string s_vitalityRequest = "http://10.20.133.13/vitality.aspx?action={0}&target={1}&api={2}";
        public static string s_homePage = "http://10.20.133.13";
        public static string s_commentPage = "http://10.20.133.13/comment.html";
#else
        public static string s_vitalityRequest = "http://quick.kd.net/vitality.aspx?action={0}&target={1}&api={2}";
        public static string s_homePage = "http://quick.kd.net";
        public static string s_commentPage = "http://quick.kd.net/comment.html";
#endif
    }
}
