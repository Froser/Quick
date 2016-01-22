using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.QRCode
{
    public class QRSettings
    {
        public string fg;
        public string bg;
        public string gc;
        public string logo = null;
        public QRSettings()
        {
            fg = "000000";
            bg = "ffffff";
            gc = "000000";
            logo = null;
        }

        public QRSettings(string _fg, string _bg, string _gc, string _logo)
        {
            fg = _fg;
            bg = _bg;
            gc = _gc;
            logo = _logo;
        }
    }
}
