using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Froser.Quick
{
    internal static class QuickPluginLoader
    {
        public static void AddAdditionQuickMethodTo(Dictionary<String, QuickModel> models)
        {
            // 从配置文件中读取出加载项，放入model中
            var plugins = QuickConfig.ThisConfig.Plugins;
            if (!plugins.Contains(s_defaultPlugin))
                plugins.Insert(0, s_defaultPlugin);
            List<string> addedList = new List<string>();
            foreach (var pluginPath in plugins)
            {
                if (addedList.Contains(pluginPath))
                    continue;
                addedList.Add(pluginPath);
                string additionDir = QuickUtilities.DirectoryFromDomain(PLUGINS_PATH);
                string addFullPath = Path.Combine(additionDir, pluginPath);
                try
                {
                    var methods = GetMethodsFromAssembly(addFullPath);
                    if (methods == null)
                        continue;
                    foreach (var method in methods)
                        AddToCorrectModel(models, method);
                }
                catch { }
            }
        }

        private static IQuickPluginMethod[] GetMethodsFromAssembly(string path)
        {
            // 加载到当前程序集
            Assembly asm = Assembly.LoadFrom(path);
            object instance = asm.CreateInstance("Froser.Quick.QuickPlugin");
            IQuickPlugin addition = new QuickSafePluginRef((IQuickPlugin)instance);
            return addition.GetMethods();
        }

        private static void AddToCorrectModel(Dictionary<String, QuickModel> models, IQuickPluginMethod method)
        {
            QuickMethod coreMethod = new QuickMethod();
            coreMethod.MethodDefArgs = " ";
            coreMethod.MethodName = method.GetName();
            coreMethod.MethodDescription = method.GetDescription(null);
            bool acceptArgs = method.AcceptArgs();
            coreMethod.MethodParamRegex = acceptArgs ? "." : "";
            coreMethod.SetAdditionMethod(method);

            bool bAddedToSpecificModel = false;
            foreach (var modelName in QuickConfig.ThisConfig.ModelName)
            {
                var availStr = method.AvailableApplicationName();
                string[] avails = availStr != null ? availStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null;
                if (avails != null && avails.Contains(modelName, new ModelNameComparer()))
                {
                    models[modelName].MethodList.Insert(0, coreMethod);
                    bAddedToSpecificModel = true;
                }
            }
            if (!bAddedToSpecificModel)
            {
                // 如果没有加到特定的模块，则加到global中
                models[QuickModel.GlobalModelName].MethodList.Insert(0, coreMethod);
            }
        }

        private class ModelNameComparer : IEqualityComparer<String>
        {
            public bool Equals(string x, string y)
            {
                return x.Trim().ToLower() == y.Trim().ToLower();
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }

        public const string PLUGINS_PATH = "plugins";
        private const string s_defaultPlugin = "quickplugin.dll";
    }
}
