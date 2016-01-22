using Froser.Automaton;
using Froser.Automaton.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Froser.Quick.WebSite
{
    public partial class api : System.Web.UI.Page
    {
        private DatabaseInformation dbcomment;
        private string query;
        public api()
        {
            this.dbcomment =DatabaseCenter.GetDatabaseTable("tb_comments");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.query = base.Request.QueryString["action"];
            if (this.query != null)
            {
                try
                {
                    Reflection.Invoke(this.query, this);
                }
                catch
                {
                }
            }
        }

        public void download()
        {
            string url = XDocument.Load(base.Server.MapPath("~/download/version.xml")).Element("info").Element("version").Element("url").Value;
            base.Response.Redirect(url);
        }

        public void getlatestdescription(string xmlfilename)
        {
            try
            {
                string s = XDocument.Load(base.Server.MapPath("~/download/" + xmlfilename)).Element("info").Element("version").Element("description").Value;
                base.Response.Write(s);
                base.Response.End();
            }
            catch
            {
            }
        }

        public void getlatestversion(string xmlfilename)
        {
            try
            {
                string s = XDocument.Load(base.Server.MapPath("~/download/" + xmlfilename)).Element("info").Element("version").Attribute("value").Value;
                base.Response.Write(s);
                base.Response.End();
            }
            catch
            {
            }
        }

        public void getcommentscount()
        {
            string s = Database.QueryScalar(this.dbcomment, string.Format("SELECT COUNT(*) FROM {0}", dbcomment.DB_TABLE_NAME)).ToString();
            base.Response.Write(s);
            base.Response.End();
        }

        public void getcomment(string limit)
        {
            Queue<object[]> retQueue = new Queue<object[]>();
            Database.Query(this.dbcomment, "SELECT com_name, com_content, com_date FROM " + dbcomment.DB_TABLE_NAME + " ORDER BY com_date DESC LIMIT " + limit, 3, retQueue);
            StringBuilder builder = new StringBuilder();
            builder.Append("{ 'comments': [");
            while (retQueue.Count > 0)
            {
                object[] objArray = retQueue.Dequeue();
                builder.AppendFormat("{{'name': '{0}', 'content':'{1}', 'date':'{2}'}}", objArray[0].ToString().Replace("'", @"\'"), objArray[1].ToString().Replace("'", @"\'"), objArray[2]);
                builder.Append(',');
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append("] }");
            base.Response.Write(builder.ToString());
            base.Response.End();
        }

        public void addcomment(string name, string content)
        {
            string userHostAddress = base.Request.UserHostAddress;
            if (userHostAddress == "::1")
            {
                userHostAddress = AutomatonUtil.IP;
            }
            string str2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = string.Format("INSERT INTO {4} (com_ip, com_name, com_content, com_date) VALUES ('{0}', '{1}', '{2}', '{3}')", new object[] { userHostAddress, name, content, str2, dbcomment.DB_TABLE_NAME });
            Database.QueryScalar(this.dbcomment, sql);
            base.Response.Write("您的评论已经提交，谢谢您的评论！");
            base.Response.End();
        }
    }
}