using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Froser.Quick
{
    public class QuickModel
    {
        [XmlIgnore]
        public static String GlobalModelName
        {
            get
            {
                return "__global";
            }
        }

        public QuickModel()
        {
            MethodList = new List<QuickMethod>();
        }

        public QuickModel(QuickModel cloneProperty)
        {
            MethodList = new List<QuickMethod>();
            this.BorderColorB = cloneProperty.BorderColorB;
            this.BorderColorG = cloneProperty.BorderColorG;
            this.BorderColorR = cloneProperty.BorderColorR;
            this.Left = cloneProperty.Left;
            this.Search = cloneProperty.Search;
            this.Top = cloneProperty.Top;
            this.Width = cloneProperty.Width;
        }
        
        public static void CreateTemplate()
        {
            var template = new QuickModel();

            template.ProgramName = "kwps.application";
            template.MethodList.Add(new QuickMethod());

            template.BorderColorR = 77;
            template.BorderColorG = 130;
            template.BorderColorB = 228;
            using (var fs = File.Create(QuickUtilities.DirectoryFromDomain(@"config\template.xml")))
            {
                new XmlSerializer(template.GetType()).Serialize(fs, template);
            }
        }

        public static QuickModel GetModel(String filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(QuickModel));
            QuickModel ret = null;
            using (FileStream fs = new FileStream(Path.Combine(QuickUtilities.DirectoryFromDomain(@"config\"), filename), FileMode.Open, FileAccess.Read))
            {
                ret = (QuickModel)ser.Deserialize(fs);
                return ret;
            }
        }

        public void Save(String filename)
        {
            Save(this, filename);
        }

        private static void Save(QuickModel instance, String filename)
        {
            XmlSerializer xmlsSave = new XmlSerializer(typeof(QuickModel));
            using (FileStream fs = new FileStream(Path.Combine(QuickUtilities.DirectoryFromDomain(@"config\"), filename), FileMode.Create, FileAccess.Write))
            {
                xmlsSave.Serialize(fs, instance);
            }
        }

        public static String[] GetArguments(string input)
        {
            char quote = '"';
            char[] separator = new char [] { ' ', ',' };
            //state:
            //-1: parameter with no quote
            //0: start
            //1: in quote
            int state = 0;
            List<String> result = new List<string>();
            string value = "";
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                switch (state)
                {
                    case -1:
                        if (separator.Contains(c))
                        {
                            state = 0;
                            result.Add(value);
                            value = "";
                        }
                        else
                        {
                            value += c;
                        }
                        break;
                    case 0:
                        if (c == quote)
                        {
                            state = 1;
                        }
                        else
                        {
                            state = -1;
                            i--;
                        }
                        break;
                    case 1:
                        if (c == quote)
                        {
                            state = 0;
                            result.Add(value);
                            value = "";
                            if (i < input.Length - 1 && separator.Contains(input[i + 1]))
                                i++;
                        }
                        else
                        {
                            value += c;
                        }
                        break;
                }
            }

            if (value != "")
                result.Add(value);
            return result.ToArray();
        }

        public QuickModel GetFilteredModel(Func<QuickMethod, bool> condition)
        {
            //condition为筛选条件，达到条件者被保留
            QuickModel model = new QuickModel();
            Action<QuickMethod> copyAction = (method) => {
                if (condition(method)) {
                    model.MethodList.Add (method);
                }
            };
            MethodList.ForEach(copyAction);
            return model;
        }

        // meta，不要修改变量名
        public String ProgramName { get; set; }
        public int BorderColorR { get; set; }
        public int BorderColorG { get; set; }
        public int BorderColorB { get; set; }
        public String Left { get; set; }
        public String Width { get; set; }
        public String Top { get; set; }
        public String Search { get; set; }
        public List<QuickMethod> MethodList { get; set; }
    }
}
