using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Froser.Quick
{
    public class QuickConfig
    {
        public Boolean FirstRun { get; set; }
        public Keys HotKey { get; set; }
        public Int32 HotKeyFlags { get; set; }
        public Boolean SearchToogle { get; set; }

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
                }
                return _config;
            }
        }

        private QuickConfig()
        {
            FirstRun = true;
            HotKey = Keys.Q;
            HotKeyFlags = (int)Hotkey.KeyFlags.MOD_CONTROL;
            SearchToogle = true;
        }

        public bool TrySave()
        {
            return TrySave(this);
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
                instance = new QuickConfig();
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
                //注意：以下操作线程不安全
                //将密码暂时加密
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
    }
}
