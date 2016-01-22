using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;
namespace Froser.Quick
{
    public class QuickConfig
    {
        public class ContextMenuItem
        {
            public const string Replacement = "{$1}";
            public string Name { get; set; }
            public string Exec { get; set; }
            public string Argument { get; set; }
            public ContextMenuItem()
            {
                Name = "上下文名称";
                Exec = "打开的地址";
                Argument = "运行参数";
            }
            public ContextMenuItem(string name, string exec, string argument)
            {
                Name = name;
                Exec = exec;
                Argument = argument;
            }
        }

        private static String ConfigDir
        {
            get
            {
                String strConfigDirector = Application.StartupPath;
                if (!Directory.Exists(strConfigDirector))
                {
                    Directory.CreateDirectory(strConfigDirector);
                }
                return strConfigDirector;
            }
        }

        private static String ConfigPath
        {
            get
            {
                return Path.Combine(ConfigDir, "quick.config");
            }
        }

        private static QuickConfig _config;
        public static QuickConfig ThisConfig
        {
            get
            {
                if (_config == null)
                {
                    _config = Load();
                    TrySave(_config);
                }
                return _config;
            }
        }

        private QuickConfig(bool isTemplate)
        {
            Init();
            if (isTemplate)
            {
                ModelName = new List<string>();
                ModelName.Add("wps");
                ModelName.Add("et");
                ModelName.Add("wpp");
                ModelName.Add("winword");
                ModelName.Add("excel");
                ModelName.Add("powerpnt");

                ContextMenuList = new List<ContextMenuItem>();
                ContextMenuList.Add(new ContextMenuItem("用BugManager搜索", "http://wpswebsvr.wps.kingsoft.net/ksbm/ViewIssue.aspx?IssueID={$1}", ""));
                ContextMenuList.Add(new ContextMenuItem("用Google搜索", "https://www.google.com.hk/webhp?hl=zh-CN&sourceid=cnhp&gws_rd=ssl#newwindow=1&safe=strict&hl=zh-CN&q={$1}", ""));
                ContextMenuList.Add(new ContextMenuItem("用Baidu搜索", "http://www.baidu.com/baidu?wd={$1}", ""));
            }
        }

        private QuickConfig()
        {
            Init();
        }

        private void Init()
        {
            FirstRun = true;
            QuickHotKey = Key.Q;
            QuickHotKeyFlags = (int)Hotkey.KeyFlags.MOD_CONTROL;
            ContextMenuHotKey = Key.OemTilde;
            ContextMenuHotKeyFlags = (int)Hotkey.KeyFlags.MOD_CONTROL;
            ContextMenuToogle = true;
            LockWindow = false;
        }

        public void SetDefaultConfig()
        {
            QuickHotKey = Key.Q;
            QuickHotKeyFlags = (int)Hotkey.KeyFlags.MOD_CONTROL;
            ContextMenuHotKey = Key.OemTilde;
            ContextMenuHotKeyFlags = (int)Hotkey.KeyFlags.MOD_CONTROL;
            ContextMenuToogle = true;

            ContextMenuList = new List<ContextMenuItem>();
            ContextMenuList.Add(new ContextMenuItem("用BugManager搜索", "http://wpswebsvr.wps.kingsoft.net/ksbm/ViewIssue.aspx?IssueID={$1}", ""));
            ContextMenuList.Add(new ContextMenuItem("用Google搜索", "https://www.google.com.hk/webhp?hl=zh-CN&sourceid=cnhp&gws_rd=ssl#newwindow=1&safe=strict&hl=zh-CN&q={$1}", ""));
            ContextMenuList.Add(new ContextMenuItem("用Baidu搜索", "http://www.baidu.com/baidu?wd={$1}", ""));
        }

        public bool TrySave()
        {
            return TrySave(this);
        }

        public void Reload()
        {
            _config = Load();
        }

        /// <summary>
        /// 读取一个配置文件，并返回一个PluginConfig对象。若XML配置文件不存在，则会用初始化本类的方式（使用默认初始化值）来创建一个配置文件。
        /// </summary>
        private static QuickConfig Load()
        {
            QuickConfig instance;
            if (!File.Exists(ConfigPath))
            {
                //配置文件不存在，则创建一个新的实例
                instance = new QuickConfig(true);
                TrySave(instance);
            }
            else
            {
                //如果文件存在，则反序列化它
                XmlSerializer xmlsLoad = new XmlSerializer(typeof(QuickConfig));
                using (FileStream fs = new FileStream(ConfigPath, FileMode.Open, FileAccess.Read))
                {
                    instance = (QuickConfig)xmlsLoad.Deserialize(fs);
                }
            }
            return instance;
        }

        private static bool TrySave(QuickConfig instance)
        {
            try
            {
                XmlSerializer xmlsSave = new XmlSerializer(typeof(QuickConfig));
                using (FileStream fs = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write))
                {
                    xmlsSave.Serialize(fs, instance);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Boolean FirstRun { get; set; }
        public Key QuickHotKey { get; set; }
        public Int32 QuickHotKeyFlags { get; set; }
        public Key ContextMenuHotKey { get; set; }
        public Int32 ContextMenuHotKeyFlags { get; set; }
        public Boolean ContextMenuToogle { get; set; }
        public List<String> ModelName { get; set; }
        public List<ContextMenuItem> ContextMenuList { get; set; }
        public String CurrentVersion { get; set; }
        public List<String> Plugins { get; set; }
        public Boolean LockWindow { get; set; }
    }
}
