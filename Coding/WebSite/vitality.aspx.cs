using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Froser.Automaton.Network;
using Froser.Automaton;
using Froser.Quick.WebSite;

namespace Froser.Quick.WebSite
{
    public class VitalityInfo
    {
        [MySqlType("Text")]
        public string vitality_agent{ get; set; }
        public string vitality_ip{ get; set; }
        public string vitality_action { get; set; }
        public string vitality_target { get; set; }
        [MySqlType("Text")]
        public string vitality_api { get; set; }
        public string vitality_time { get; set; }
        public string vitality_date { get; set; }        
    }
    public partial class vitality : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = base.Request.QueryString["action"];
            string target = base.Request.QueryString["target"];
            string api = base.Request.QueryString["api"];
            string userHostAddress = base.Request.UserHostAddress;
            if (!(userHostAddress == "::1") && (action != null))
            {
                this.AddIntoDatabase(userHostAddress, action, target, api);
            }
        }

        private void AddIntoDatabase(string IP, string action, string target, string api)
        {
            string userAgent = base.Request.UserAgent;
            string str2 = DateTime.Now.ToString("yyyyMMdd");
            string str3 = DateTime.Now.ToString("HH:mm:ss");
            VitalityInfo item = new VitalityInfo();
            item.vitality_ip = IP;
            item.vitality_action = action;
            item.vitality_date = str2;
            item.vitality_time = str3;
            item.vitality_agent = userAgent;
            item.vitality_target = target;
            item.vitality_api = api;
            DatabaseInformation dbi = DatabaseCenter.GetDatabaseTable("tb_vitality");
            Database.CreateTable(dbi, item);
            Database.Insert(dbi, item);
        }
    }
}