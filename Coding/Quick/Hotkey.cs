using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Froser.Quick
{
    internal delegate void HotkeyEventHandler(int HotKeyID);

    internal class Hotkey : global::System.Windows.Forms.IMessageFilter
    {
        global::System.Collections.Hashtable keyIDs = new global::System.Collections.Hashtable();
        IntPtr hWnd;

        public event HotkeyEventHandler OnHotkey;

        public enum KeyFlags
        {
            MOD_NONE = 0x0,
            MOD_ALT = 0x1,
            MOD_CONTROL = 0x2,
            MOD_SHIFT = 0x4,
            MOD_WIN = 0x8,
            MOD_NOREPEAT = 0x4000
        }
        [DllImport("user32.dll")]
        private static extern UInt32 RegisterHotKey(IntPtr hWnd, UInt32 id, UInt32 fsModifiers, UInt32 vk);

        [DllImport("user32.dll")]
        private static extern UInt32 UnregisterHotKey(IntPtr hWnd, UInt32 id);

        [DllImport("kernel32.dll")]
        private static extern UInt32 GlobalAddAtom(String lpString);

        [DllImport("kernel32.dll")]
        private static extern UInt32 GlobalDeleteAtom(UInt32 nAtom);

        public Hotkey(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            global::System.Windows.Forms.Application.AddMessageFilter(this);
        }

        public int RegisterHotkey(Key Key, KeyFlags keyflags)
        {
            try
            {
                UInt32 hotkeyid = GlobalAddAtom(global::System.Guid.NewGuid().ToString());
                int keycode = KeyInterop.VirtualKeyFromKey(Key);
                RegisterHotKey((IntPtr)hWnd, hotkeyid, (UInt32)keyflags, (UInt32)keycode);
                keyIDs.Add(hotkeyid, hotkeyid);
                return (int)hotkeyid;
            }
            catch { return 0; };
        }

        public void UnregisterHotkeys()
        {
            global::System.Windows.Forms.Application.RemoveMessageFilter(this);
            foreach (UInt32 key in keyIDs.Values)
            {
                UnregisterHotKey(hWnd, key);
                GlobalDeleteAtom(key);
            }
        }

        public bool PreFilterMessage(ref global::System.Windows.Forms.Message m)
        {
            if (m.Msg == 0x312)
            {
                if (OnHotkey != null)
                {
                    foreach (UInt32 key in keyIDs.Values)
                    {
                        if ((UInt32)m.WParam == key)
                        {
                            OnHotkey((int)m.WParam);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}