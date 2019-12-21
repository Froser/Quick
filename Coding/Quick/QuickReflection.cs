using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Froser.Quick
{
    /// <summary>
    /// 定义了一些反射方法的类
    /// </summary>
    internal static class QuickReflection
    {
        /// <summary>
        /// 通过调用一些符合C#语法的语句，给对象的成员赋值
        /// </summary>
        /// <param name="api">符合C#语法的语句</param>
        /// <param name="value">需要赋的值</param>
        /// <param name="targetObj">需要赋值的对象</param>
        public static void Set(String api, object value, object targetObj)
        {
            object last_obj;
            string last_member_name;
            object obj = Invoke(api, out last_obj, out last_member_name, targetObj);
            last_obj.GetType().InvokeMember(last_member_name, BindingFlags.SetProperty, null, last_obj, new object[] { value });
        }

        /// <summary>
        /// 调用某个对象的特定方法或返回特定属性成员
        /// </summary>
        /// <param name="api">符合C#语法的语句</param>
        /// <param name="targetObj">需要赋值的对象</param>
        /// <returns>返回对象或属性的值</returns>
        public static object Invoke(String api, object targetObj)
        {
            string strUseless = "";
            object objUseless = null;
            return Invoke(api, out objUseless, out strUseless, targetObj);
        }

        private static object Invoke(String api, out object last_obj, out string last_member_name, object targetObj)
        {
            last_member_name = "";
            last_obj = null;
            object _obj = targetObj;
            string member_name;
            object[] parameters;
            BindingFlags flags;
            bool loop = true;
            try
            {
                //通过反射运行一段API，并获取返回值
                //API返回值大致这样分类：
                // 1. 属性值 ( {Property.Property...} = {pi} )
                // 2. 方法返回值 ( {Property.Method (Param1, Param2 ...) )
                while (loop)
                {
                    loop = GetNextMember(api, _obj, out member_name, out flags, out parameters, out api, targetObj);
                    last_obj = _obj;
                    _obj = _obj.GetType().InvokeMember(member_name, flags, null, _obj, parameters);
                    last_member_name = member_name;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return _obj;
        }

        private static bool GetNextMember(string api_string, object COMObj, out string member_name, out BindingFlags flags, out object[] parameters, out string api_remaining_string, object top_obj)
        {
            Regex regIndexer = new Regex(@"\[(?<index>(.*))\]");
            string s = api_string;
            while (s[0] == '.')
            {
                s = s.Remove(0, 1);
            }
            String[] sections = s.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Match mIndexer = regIndexer.Match(sections[0]);
            while (mIndexer.Success)
            {
                string result = mIndexer.Result("${index}");
                sections[0] = sections[0].Replace("[" + result + "]", ".Item(" + result + ")");
                mIndexer = mIndexer.NextMatch();
            }
            s = "";
            foreach (string frag in sections)
            {
                s += frag + ".";
            }
            s = s.Remove(s.Length - 1);

            string recorder = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s[i])) continue;
                if (s[i] != '.' && s[i] != '(' && s[i] != '[')
                {
                    recorder += s[i];
                }
                else if (s[i] == '.')
                {
                    flags = BindingFlags.GetProperty;
                    member_name = recorder;
                    parameters = null;
                    api_remaining_string = s.Substring(i);
                    if (api_remaining_string == "") return false;
                    return true;
                }
                else if (s[i] == '(')
                {
                    i++;
                    string pstring = "";
                    try
                    {
                        while (s[i] != ')' || s.Substring(0, i).CountOf('"') % 2 == 1)
                        {
                            pstring += s[i];
                            i++;
                        }
                        i++;
                        List<object> plist = new List<object>();
                        string tmp = "";
                        Action actAddToList = new Action(() =>
                        {
                            if (tmp != "")
                            {
                                double ifDouble;
                                int ifInt;
                                if (int.TryParse(tmp, out ifInt))
                                {
                                    plist.Add(ifInt);
                                }
                                else if (double.TryParse(tmp, out ifDouble))
                                {
                                    plist.Add(ifDouble);
                                }
                                else if (tmp.ToLower() == "true")
                                {
                                    plist.Add(true);
                                }
                                else if (tmp.ToLower() == "false")
                                {
                                    plist.Add(false);
                                }
                                else if (tmp[0] == '"' && tmp[tmp.Length - 1] == '"')
                                {
                                    plist.Add(tmp);
                                }
                                else
                                {
                                    plist.Add(Invoke(tmp, top_obj));
                                }
                            }
                            tmp = "";
                        });

                        for (int j = 0; j < pstring.Length; j++)
                        {
                            tmp += pstring[j];
                            if (pstring[j] == '"')
                            {
                                j++;
                                while (pstring[j] != '"')
                                {
                                    tmp += pstring[j];
                                    j++;
                                }
                                tmp += pstring[j];
                                plist.Add(tmp.Replace("\"", ""));
                                tmp = "";
                            }
                            else
                            {
                                if (char.IsWhiteSpace(pstring[j]))
                                {
                                    //2014.4.24
                                    tmp = tmp.Remove(0);
                                    //---------
                                    continue;
                                }
                                if (pstring[j] == ',')
                                {
                                    tmp = tmp.Remove(tmp.Length - 1);
                                    actAddToList.Invoke();
                                }
                            }
                        }

                        if (tmp != "")
                        {
                            actAddToList.Invoke();
                        }

                        parameters = plist.ToArray();
                        flags = BindingFlags.InvokeMethod;
                        member_name = recorder;
                        api_remaining_string = s.Substring(i);
                        if (api_remaining_string == "") return false;
                        return true;

                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new IndexOutOfRangeException("输入的语句不合法");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            member_name = recorder;
            flags = BindingFlags.GetProperty;
            parameters = null;
            api_remaining_string = "";
            return false;
        }

        /// <summary>
        /// 判断某个属性是否为字符串、布尔型、数字型或枚举型
        /// </summary>
        /// <param name="p">需要进行判断属性</param>
        /// <returns>返回判断结果</returns>
        private static bool IsValueType(PropertyInfo p)
        {
            Type t = p.PropertyType;
            if (t == typeof(String) ||
                t == typeof(Int32) ||
                t == typeof(Int64) ||
                t == typeof(Single) ||
                t == typeof(Double) ||
                t == typeof(Boolean) ||
                t.IsEnum
                )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 计算在字符串内某个字符出现的次数
        /// </summary>
        /// <param name="str">需要进行判断的字符串</param>
        /// <param name="ch">需要计算数量的字符</param>
        /// <returns>返回值: 字符个数</returns>
        private static int CountOf(this string str, char ch)
        {
            int count = 0;
            foreach (char c in str)
            {
                if (c == ch) count++;
            }
            return count;
        }
    }
}
