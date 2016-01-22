using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Quick.QRCode
{
    public partial class QRAdvForm : Form
    {
//         public static string uploadAPI = "http://chuantu.biz/upload.php";

        public QRSettings QrSettings { get; set; }

        public QRAdvForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            QrSettings = new QRSettings();
            bg.BackColor = ColorTranslator.FromHtml("#" + QrSettings.bg);
            fg.BackColor = ColorTranslator.FromHtml("#" + QrSettings.fg);
            gc.BackColor = ColorTranslator.FromHtml("#" + QrSettings.gc);
        }

        private void ChangeColor(object sender, EventArgs e)
        {
            var me = sender as PictureBox;
            color.Color = me.BackColor;
            if (color.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                me.BackColor = color.Color;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string fghex = Convert.ToString(fg.BackColor.ToArgb(), 16).Substring(2);
            string bghex = Convert.ToString(bg.BackColor.ToArgb(), 16).Substring(2);
            string gchex = Convert.ToString(gc.BackColor.ToArgb(), 16).Substring(2);
            string logo = null;
            if (rLink.Checked)
            {
                logo = textLogoURL.Text;
            }
            QrSettings = new QRSettings(fghex, bghex, gchex, logo);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void textLogoURL_TextChanged(object sender, EventArgs e)
        {
            rLink.Checked = true;
        }
// 
//         public static string Post(String imgPath)
//         {
//             int timeout = 1500000;
//             string _bound = "-----multipartformboundary";
//             String boundary = "--" + _bound;
// 
//             var content = new StringBuilder();
//             Image img = Image.FromFile (imgPath);
//             var tmpfilename = Path.GetTempFileName();
//             img.Save(tmpfilename, ImageFormat.Png);
// 
//             content
//                 .AppendLine(boundary)
//                 .AppendLine("Content-Disposition: form-data; name=\"MAX_FILE_SIZE\"")
//                 .AppendLine()
//                 .AppendLine("200000000")
//                 .AppendLine(boundary)
//                 .AppendLine("Content-Disposition: form-data; name=\"uploadimg\"; filename=\"upload.png\"")
//                 .AppendLine("Content-Type: image/png")
//                 .AppendLine();
// 
//             try
//             {
//                 Uri uri = new Uri(uploadAPI);
//                 HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
// 
//                 byte[] byteData = new UTF8Encoding(false).GetBytes(content.ToString());
//                 request.Timeout = timeout;
//                 request.UseDefaultCredentials = true;
//                 request.Method = "POST";
//                 request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0";
//                 request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
//                 request.ContentType = "multipart/form-data; boundary=" + _bound;
//                 request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
//                 request.Headers.Add("Accept-Encoding", "gzip, deflate");
//                 request.KeepAlive = true;
// 
//                 var end = new UTF8Encoding(false).GetBytes(Environment.NewLine + boundary + "--" + Environment.NewLine);
// 
//                 using (var imgStream = new FileStream(tmpfilename, FileMode.Open, FileAccess.Read))
//                 {
//                     request.ContentLength = byteData.Length + imgStream.Length + end.Length;
//                     using (Stream streamWriter = request.GetRequestStream())
//                     {
//                         streamWriter.Write(byteData, 0, byteData.Length);
//                         byte[] imgBytes = new byte[imgStream.Length];
//                         imgStream.Read(imgBytes, 0, imgBytes.Length);
//                         streamWriter.Write(imgBytes, 0, imgBytes.Length);
//                         streamWriter.Write(end, 0, end.Length);
//                     }
//                 }
// 
//                 using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
//                 {
//                     StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding ("ISO-8859-1"));
//                     return reader.ReadToEnd();
//                 }
//             }
//             catch (WebException)
//             {
//                 MessageBox.Show("对不起，获取信息超时，请稍后重试 %>_<%", "Quick QRCode", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                 throw;
//             }
//             catch (Exception)
//             {
//                 throw;
//             }
//             finally
//             {
//                 try { File.Delete(tmpfilename); }
//                 catch { }
//                 GC.Collect();
//             }
//         }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://www.mftp.info/");
            }
            catch { }
        }
// 
//         private void textLocalImg_TextChanged(object sender, EventArgs e)
//         {
//             rUpload.Checked = true;
//         }
    }
}
