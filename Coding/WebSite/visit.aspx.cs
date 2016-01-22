using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Froser.Automaton.Network;
using Froser.Automaton;

namespace Froser.Quick.WebSite
{
    public class VisitorInfo
    {
        [MySqlType("Text")]
        public string visitor_agent{ get; set; }
        public string visitor_date{ get; set; }
        public string visitor_ip{ get; set; }
        public string visitor_page{ get; set; }
        public string visitor_time{ get; set; }
    }
    public partial class visit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string page = base.Request.QueryString["page"];
            string userHostAddress = base.Request.UserHostAddress;
            if (!(userHostAddress == "::1") && (page != null))
            {
                this.AddIntoDatabase(userHostAddress, page);
            }
        }

        private void AddIntoDatabase(string IP, string Page)
        {
            string userAgent = base.Request.UserAgent;
            string str2 = DateTime.Now.ToString("yyyyMMdd");
            string str3 = DateTime.Now.ToString("HH:mm:ss");
            VisitorInfo item = new VisitorInfo();
            item.visitor_ip = IP;
            item.visitor_page = Page;
            item.visitor_date = str2;
            item.visitor_time = str3;
            item.visitor_agent = userAgent;
            DatabaseInformation dbi = DatabaseCenter.GetDatabaseTable("tb_visitors");
            Database.CreateTable(dbi, item);
            Database.Insert(dbi, item);
        }
    }
}