using System;
using Froser.Quick;
using System.Net;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Drawing;
using System.Web;
using System.IO;

namespace Quick.QRCode
{
    //这是Quick插件开发的一个标准模板
    public class QuickPlugin : IQuickPlugin
    {
        bool isCallback = false;

        static string api = "http://qr.liantu.com/api.php?text={0}&bg={1}&fg={2}&gc={3}";

        public string GetAPI(String text, QRSettings settings)
        {
            string ret = String.Format(api, text, settings.bg, settings.fg, settings.gc);
            if (settings.logo != null)
                ret += "&logo=" + settings.logo;
            return ret;
        }

        QRSettings qrsettings = new QRSettings();

        //插件的状态
        enum PluginState
        {
            Help,
            QR,
            QRADV
        }
        PluginState pluginState;

        const int pluginTimeout = 15000;

        public String Name { get { return "二维码(qrcode) {Quick.QRCode} "; } }

        public String Description {
            get {
                return
                    "生成选定文字的二维码图形，输入help查看帮助及版本"
                ;
            }
        }

        public Boolean AcceptArguments { get { return true; } }

        public String ApplicationName { get { return "wps, WINWORD, et, EXCEL, wpp, POWERPNT"; } }

        //表示一个异步方法
        public void BeginInvoke(object arg)
        {
            //从元组中提取数据
            var sender = ((Tuple<object, object[]>)arg).Item1;
            var arguments = ((Tuple<object, object[]>)arg).Item2;

            //当参数为help时
            if (pluginState == PluginState.Help)
            {
                //显示帮助，然后退出此方法
                MessageBox.Show(Resource.Help + Environment.NewLine + "插件版本：" + PluginVersion, "Quick QRCode 帮助", MessageBoxButtons.OK);
                return;
            }

            var senderName = Reflection.Invoke("Name", sender).ToString();


            var selapi = "";
            string preinvoke = null;
            var insert = "";

            if (senderName == "Microsoft PowerPoint")
            {
                selapi = "ActiveWindow.Selection.TextRange.Text";
                insert = "ActiveWindow.View.Paste()";
            }
            else if (senderName == "Microsoft Word")
            {
                selapi = "Selection.Text";
                preinvoke = "Selection.SetRange (Selection.Range.End, Selection.Range.End)";
                insert = "Selection.InlineShapes.AddPicture ($)";
            }
            else
            {
                selapi = "ActiveCell.Value";
                insert = "ActiveSheet.Shapes.AddPicture ($, 0, -1, -1, -1, -1, -1)";
            }

            try
            {
                switch (pluginState)
                {
                    case PluginState.QR:
                        //插件的主要行为
                        var img = GetImage ( Reflection.Invoke (selapi, sender).ToString (), qrsettings);
                        
                        var tmpFilename = Path.GetTempFileName ();
                        img.Save(tmpFilename);
                        Clipboard.SetImage(img);

                        InsertPicture(tmpFilename, preinvoke, insert, sender);
                        break;
                    case PluginState.QRADV:
                        var form = new QRAdvForm();
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            qrsettings = form.QrSettings;
                            isCallback = true;
                            pluginState = PluginState.QR;
                            Invoke(sender, new object[]{""});
                        }
                        break;
                }
            }
            catch (WebException)
            {
                //如果是一个网络API插件，这里则最有可能是超时抛出的异常
            }
            catch (Exception)
            {
            }
        }

        public void Invoke(object sender, object[] arguments)
        {
            if (!isCallback)
            {
                //分析出插件的状态
                switch (arguments[0].ToString().ToLower())
                {
                    case "help":
                        pluginState = PluginState.Help;
                        break;
                    case "a":
                    case "advance":
                        pluginState = PluginState.QRADV;
                        break;
                    default:
                        pluginState = PluginState.QR;
                        break;
                }
            }

            Thread t = new Thread(new ParameterizedThreadStart(BeginInvoke));
            t.SetApartmentState(ApartmentState.STA);
            
            //将参数放入元组
            t.Start(new Tuple<object, object[]>(sender, arguments));

            //根据实际需要，将某些行为设定为有超时机制，有些则没有（如对话框）
            //将所有的行为都放入一个新的线程，当然也可以将那些没有超时机制的行为放入此主线程中
            if (pluginState != PluginState.Help && pluginState != PluginState.QRADV)
            {
                if (!t.Join(pluginTimeout))
                {
                    try { t.Abort(); }
                    catch { }
                    MessageBox.Show("对不起，等待超时，请稍后重试。", "Quick QRCode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                t.Join();
            }
        }

        public static String PluginVersion
        {
            get
            {
                Version version = Assembly.GetCallingAssembly().GetName().Version;
                return version.ToString();
            }
        }

        public Image GetImage(string text, QRSettings settings)
        {
            Image qrImg = null;
            string _api = GetAPI(text, settings);
            WebClient wc = new WebClient();
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(_api);
            webRequest.Method = "GET";
            using (var stream = webRequest.GetResponse().GetResponseStream())
            {
                qrImg = Image.FromStream(stream);
            }
            return qrImg;
        }

        private void InsertPicture(String imageFileName, String preinvoke, String insert, Object sender)
        {
            if (preinvoke != null) Reflection.Invoke(preinvoke, sender);
            Reflection.Invoke(insert.Replace("$", "\"" + imageFileName + "\""), sender);
        }
    }
}
