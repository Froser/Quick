using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Froser.Quick
{
    public class QuickMethod
    {
        public String MethodName = "打开资源管理器 [dakaiziyuanguanliqi]";
        public String MethodScript = "Call(\"explorer.exe\",\"\")";
        public String MethodDescription = "打开资源管理器";
        public String MethodParamRegex = @""; // 可接参数的正则表达式匹配，如\d表示任意数字，.表示任意字符，+表示一个或多个，*表示0个或多个
        public String MethodDefArgs = ""; //动作执行的默认参数，用逗号分隔开
        public Int32 MethodPriority = 0;
        [XmlIgnore]
        public String Application = "";  //针对特定程序的方法，如wps、et、wpp，用于插件中

        public override String ToString()
        {
            return Regex.Replace(MethodName, @"\[.*?\]|\{|\}", "");
        }

        public void SetAdditionMethod(IQuickPluginMethod method)
        {
            m_method = method;
        }

        public IQuickPluginMethod GetPluginInterface()
        {
            if (m_method == null)
                return null;
            return new QuickSafePluginMethodRef(m_method);
        }

        private IQuickPluginMethod m_method;
    }

}
