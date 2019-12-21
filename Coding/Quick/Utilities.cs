using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Froser.Quick
{
    public static class QuickUtilities 
    {
        /// <summary>
        /// 表示本程序目录下的一个目录的完整路径
        /// </summary>
        /// <param name="dirname">目录名</param>
        /// <param name="create">如果不存在，是否要创建一个新的目录</param>
        /// <returns></returns>
        public static String DirectoryFromDomain(String dirname, bool create)
        {
            String dir = AppDomain.CurrentDomain.BaseDirectory;
            String result = Path.Combine(dir, dirname);
            if (create)
            {
                if (!Directory.Exists(result))
                    Directory.CreateDirectory(result);
            }
            return result;
        }

        public static String DirectoryFromDomain(String dirname)
        {
            return DirectoryFromDomain(dirname, false);
        }

        public static bool IsRoot(String path)
        {
            try
            {
                if (char.IsLetter(path[0]) && path[1] == ':')
                {
                    if (path.Length == 3 && path[2] == '\\')
                    {
                        return true;
                    }
                    else if (path.Length == 2)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsLatestVersion(String current, String latest)
        {
            Version vc = new Version(current);
            Version vl = new Version(latest);

            var result = vc.CompareTo(vl);
            return result >= 0;
        }

    }

    internal class Utilities
    {
        private object comObject;
        public Utilities(object comObject)
        {
            this.comObject = comObject;
        }

        //获得一个Win32颜色的整型数值
        public int Win32Color(string api)
        {
            int def = (int) double.Parse (QuickReflection.Invoke(api, comObject).ToString ());
            try
            {
                ColorDialog cd = new ColorDialog();
                cd.Color = ColorTranslator.FromWin32(def);
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    return ColorTranslator.ToWin32(cd.Color);
                }
                return def;
            }
            catch { return def; }
        }

        // Utilities的方法返回值一定要为String[]
        public String[] AuthorAndContent(string api, String str)
        {
            return new String[] { QuickReflection.Invoke(api, comObject) + ":" + Environment.NewLine + str };
        }

        public object GetObject(string api)
        {
            return QuickReflection.Invoke(api, comObject);
        }

        public String EmptyString()
        {
            return "";
        }

        public void RunFile(String cmd, String arg)
        {
            try
            {
                Process.Start(cmd, arg);
            }
            catch (Win32Exception)
            {
                Process.Start(Path.Combine(QuickUtilities.DirectoryFromDomain(""), cmd), arg);
            }
        }
    }
}
