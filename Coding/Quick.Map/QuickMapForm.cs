using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;

namespace Quick.Map
{
    public partial class QuickMapForm : Form
    {
        private QuickMapForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            var cancelButton = new Button();
            this.CancelButton = cancelButton;
            cancelButton.Click += (sender, e) =>
                {
                    Close();
                };
        }

        private void ShowMap(Tuple<String, String> position, String ak)
        {
            var content =
                Template.htmlTemplate
                .Replace("${lng}", position.Item1)
                .Replace("${lat}", position.Item2)
                .Replace("${ak}", ak)
                .Replace("${width}", mapBrowser.Width.ToString ())
                .Replace("${height}", mapBrowser.Height.ToString ());

            mapBrowser.DocumentText = content;
        }

        private void ShowContent(String content)
        {
            mapBrowser.DocumentText = content;
        }

        public static void Show(Tuple<String, String> position, String ak)
        {
            var MapForm = new QuickMapForm();
            MapForm.ShowMap(position, ak);
            MapForm.ShowDialog();
            MapForm = null;
        }

        public static void Show(String content)
        {
            var MapForm = new QuickMapForm();
            MapForm.ShowContent(content);
            MapForm.ShowDialog();
        }

        private void QuickMapForm_ResizeEnd(object sender, EventArgs e)
        {
            Regex regCssWidth = new Regex(@"(width:\d+?px)");
            Regex regCssHeight = new Regex(@"(height:\d+?px)");
            var docText = regCssWidth.Replace(mapBrowser.DocumentText, "width:" + mapBrowser.Width + "px");
            docText = regCssHeight.Replace(docText, "height:" + mapBrowser.Height + "px");
            mapBrowser.DocumentText = docText;
        }

        private void QuickMapForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                QuickMapForm_ResizeEnd(sender, e);
        }

        private void QuickMapForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                mapBrowser.Dispose();
                this.Dispose();
            }
            catch { }
            GC.Collect();
        }

    }
}
