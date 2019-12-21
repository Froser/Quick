using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Froser.Quick.Plugins.Find
{
    internal class QuickEverything32
    {
        const String dllPath = "3rdparty/everything/everything32.dll";

        [DllImport(dllPath, CharSet = CharSet.Unicode)]
        public static extern int Everything_SetSearchW(string lpSearchString);
        [DllImport(dllPath)]
        public static extern void Everything_SetMatchPath(bool bEnable);
        [DllImport(dllPath)]
        public static extern void Everything_SetMatchCase(bool bEnable);
        [DllImport(dllPath)]
        public static extern void Everything_SetMatchWholeWord(bool bEnable);
        [DllImport(dllPath)]
        public static extern void Everything_SetRegex(bool bEnable);
        [DllImport(dllPath)]
        public static extern void Everything_SetMax(int dwMax);
        [DllImport(dllPath)]
        public static extern void Everything_SetOffset(int dwOffset);

        [DllImport(dllPath)]
        public static extern bool Everything_GetMatchPath();
        [DllImport(dllPath)]
        public static extern bool Everything_GetMatchCase();
        [DllImport(dllPath)]
        public static extern bool Everything_GetMatchWholeWord();
        [DllImport(dllPath)]
        public static extern bool Everything_GetRegex();
        [DllImport(dllPath)]
        public static extern UInt32 Everything_GetMax();
        [DllImport(dllPath)]
        public static extern UInt32 Everything_GetOffset();
        [DllImport(dllPath)]
        public static extern string Everything_GetSearchW();
        [DllImport(dllPath)]
        public static extern int Everything_GetLastError();

        [DllImport(dllPath)]
        public static extern bool Everything_QueryW(bool bWait);

        [DllImport(dllPath)]
        public static extern void Everything_SortResultsByPath();

        [DllImport(dllPath)]
        public static extern int Everything_GetNumFileResults();
        [DllImport(dllPath)]
        public static extern int Everything_GetNumFolderResults();
        [DllImport(dllPath)]
        public static extern int Everything_GetNumResults();
        [DllImport(dllPath)]
        public static extern int Everything_GetTotFileResults();
        [DllImport(dllPath)]
        public static extern int Everything_GetTotFolderResults();
        [DllImport(dllPath)]
        public static extern int Everything_GetTotResults();
        [DllImport(dllPath)]
        public static extern bool Everything_IsVolumeResult(int nIndex);
        [DllImport(dllPath)]
        public static extern bool Everything_IsFolderResult(int nIndex);
        [DllImport(dllPath)]
        public static extern bool Everything_IsFileResult(int nIndex);
        [DllImport(dllPath, CharSet = CharSet.Unicode)]
        public static extern void Everything_GetResultFullPathNameW(int nIndex, StringBuilder lpString, int nMaxCount);
        [DllImport(dllPath)]
        public static extern void Everything_Reset();
    }
}
