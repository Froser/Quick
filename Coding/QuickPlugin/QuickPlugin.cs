using Froser.Quick.Plugins;

namespace Froser.Quick
{
    internal class QuickPlugin : IQuickPlugin
    {
        public QuickPlugin()
        {
            m_additionMethod = new IQuickPluginMethod[]
                {
                    new QuickCalc(),
                    new QuickRun(),
                    new QuickFind(),
                };
        }

        public IQuickPluginMethod[] GetMethods()
        {
            return m_additionMethod;
        }

        private IQuickPluginMethod[] m_additionMethod;
    }

}
