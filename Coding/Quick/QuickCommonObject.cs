using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Froser.Quick
{
    [Serializable]
    internal class QuickCommonObject
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
            byte bVk,    //虚拟键值
            byte bScan,// 一般为0
            int dwFlags,  //这里是整数类型  0 为按下，2为释放
            int dwExtraInfo  //这里是整数类型 一般情况下设成为 0
        );

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rect lpRect);
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        private IntPtr currentWindowPtr;
        private bool isGlobal;

        public QuickCommonObject(IntPtr windowPtr, bool isGlobal)
        {
            currentWindowPtr = windowPtr;
            this.isGlobal = isGlobal;
        }


        public Rect GetWindowRect()
        {
            Rect lpRect;
            if (isGlobal)
            {
                var rect = Screen.GetWorkingArea(new Point(0, 0));
                lpRect.Left = rect.Left;
                lpRect.Right = rect.Right;
                lpRect.Top = rect.Top;
                lpRect.Bottom = rect.Bottom;
            }
            else
            {
                GetWindowRect(currentWindowPtr, out lpRect);
            }
            return lpRect;
        }

        public String GetSelection
        {
            get
            {
                keybd_event((byte)Keys.LControlKey, 0, 0, 0);
                keybd_event((byte)Keys.C, 0, 0, 0);
                keybd_event((byte)Keys.LControlKey, 0, 2, 0);
                keybd_event((byte)Keys.C, 0, 2, 0);
                Thread.Sleep(10);
                return Clipboard.GetText();
            }
        }

        public void Call(String cmd, String arg)
        {
            Process.Start(cmd, arg);
        }
    }
}
