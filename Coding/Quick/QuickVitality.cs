using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Web;

namespace Froser.Quick
{
    internal static class QuickVitality
    {
        public static void UpdateVitality(String action, String target, String api)
        {
#if !DEBUG
            Action update = () =>
            {
                action = HttpUtility.UrlEncode(action);
                target = HttpUtility.UrlEncode(target);
                api = HttpUtility.UrlEncode(api);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(String.Format(QuickMultiVersion.s_vitalityRequest, action, target, api));
                try
                {
                    request.Method = "GET";
                    request.UserAgent = "Quick " + Application.ProductVersion;
                    request.BeginGetResponse(null, null);     //只需要异步，无需知道结果
                }
                catch { }
            };
            update.BeginInvoke(null, null);
#endif
        }
    }
}
