using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Froser.Quick
{
    // 会吞掉异常的引用代理
    internal class QuickSafePluginRef : IQuickPlugin
    {
        public QuickSafePluginRef(IQuickPlugin plugin)
        {
            m_plugin = plugin;
        }

        public IQuickPluginMethod[] GetMethods()
        {
            try
            {
                return m_plugin.GetMethods();
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin", "plugin load error", "invoking: GetMethods() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        private IQuickPlugin m_plugin;
    }

    internal class QuickSafePluginMethodRef : IQuickPluginMethod
    {
        public QuickSafePluginMethodRef(IQuickPluginMethod method)
        {
            m_method = method;
        }

        public string GetName()
        {
            try
            {
                return m_method.GetName();
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", "plugin GetName() error", "invoking: GetName() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        public string GetDescription(IQuickWindow quickWindow)
        {
            try
            {
                return m_method.GetDescription(quickWindow);
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: GetName() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        public string AvailableApplicationName()
        {
            try
            {
                return m_method.AvailableApplicationName();
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: AvailableApplicationName() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        public void Invoke(object sender, IQuickWindow quickWindow)
        {
            try
            {
                m_method.Invoke(sender, quickWindow);
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: Invoke() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public bool AcceptArgs()
        {
            try
            {
                return m_method.AcceptArgs();
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: AcceptArgs() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
            }
        }

        public bool GetIcon(IQuickWindow quickWindow, out System.Windows.Media.ImageSource icon)
        {
            try
            {
                return m_method.GetIcon(quickWindow, out icon);
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: GetIcon() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
                icon = null;
                return false;
            }
        }

        public void KeyDown(IQuickWindow quickWindow, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                m_method.KeyDown(quickWindow, e);
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: KeyDown() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void TextChanged(IQuickWindow quickWindow, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                m_method.TextChanged(quickWindow, e);
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: TextChanged() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void Closed(IQuickWindow quickWindow)
        {
            try
            {
                m_method.Closed(quickWindow);
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: Closed() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void PageDown(IQuickWindow quickWindow)
        {
            try
            {
                m_method.PageDown(quickWindow);
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: PageDown() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void PageUp(IQuickWindow quickWindow)
        {
            try
            {
                m_method.PageUp(quickWindow);
            }
            catch (Exception ex)
            {
                QuickVitality.UpdateVitality("error in plugin method", m_method.GetName(), "invoking: PageUp() and caused " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private IQuickPluginMethod m_method;
    }
}
