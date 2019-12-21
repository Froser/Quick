using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Froser.Quick.UI
{
    public static class QuickUIResource
    {
        public static BitmapImage GetDefaultIcon()
        {
            if (s_defaultIcon == null)
            {
                string uri = "resources/icons/quick.png";
                s_defaultIcon = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
            }
            return s_defaultIcon;
        }

        public static BitmapImage GetDefaultPluginIcon()
        {
            if (s_defaultPluginIcon == null)
            {
                string uri = "resources/icons/plugin.png";
                s_defaultPluginIcon = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
            }
            return s_defaultPluginIcon;
        }

        public static BitmapImage GetLockedIcon()
        {
            if (m_iconLocked == null)
            {
                string uri = "resources/icons/locked.png";
                m_iconLocked = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
            }
            return m_iconLocked;
        }

        public static BitmapImage GetUnlockedIcon()
        {
            if (m_iconUnlocked == null)
            {
                string uri = "resources/icons/unlocked.png";
                m_iconUnlocked = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
            }
            return m_iconUnlocked;
        }

        private static BitmapImage s_defaultIcon;
        private static BitmapImage s_defaultPluginIcon;
        private static BitmapImage m_iconLocked;
        private static BitmapImage m_iconUnlocked;
    }
}
