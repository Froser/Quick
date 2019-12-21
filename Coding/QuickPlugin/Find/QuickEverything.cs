using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Froser.Quick.Plugins.Find
{
    public enum EVERYTHING_RESULT
    {
        EVERYTHING_OK = 0,
        EVERYTHING_ERROR_MEMORY = 1,
        EVERYTHING_ERROR_IPC = 2,
        EVERYTHING_ERROR_REGISTERCLASSEX = 3,
        EVERYTHING_ERROR_CREATEWINDOW = 4,
        EVERYTHING_ERROR_CREATETHREAD = 5,
        EVERYTHING_ERROR_INVALIDINDEX = 6,
        EVERYTHING_ERROR_INVALIDCALL = 7,
    }

    internal class QuickEverything
    {
        private static bool s_is64Bit = false;
        static QuickEverything()
        {
            s_is64Bit = Environment.Is64BitOperatingSystem;
        }
        public static int Everything_SetSearchW(string lpSearchString)
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_SetSearchW(lpSearchString);
            else
                return QuickEverything32.Everything_SetSearchW(lpSearchString);
        }

        public static void Everything_SetMatchPath(bool bEnable)
        {
            if (s_is64Bit)
                QuickEverything64.Everything_SetMatchPath(bEnable);
            else
                QuickEverything32.Everything_SetMatchPath(bEnable);
        }

        public static void Everything_SetMatchCase(bool bEnable)
        {
            if (s_is64Bit)
                QuickEverything64.Everything_SetMatchCase(bEnable);
            else
                QuickEverything32.Everything_SetMatchCase(bEnable);
        }

        public static void Everything_SetMatchWholeWord(bool bEnable)
        {
            if (s_is64Bit)
                QuickEverything64.Everything_SetMatchWholeWord(bEnable);
            else
                QuickEverything32.Everything_SetMatchWholeWord(bEnable);
        }

        public static void Everything_SetRegex(bool bEnable)
        {
            if (s_is64Bit)
                QuickEverything64.Everything_SetRegex(bEnable);
            else
                QuickEverything32.Everything_SetRegex(bEnable);
        }

        public static void Everything_SetMax(int dwMax)
        {
            if (s_is64Bit)
                QuickEverything64.Everything_SetMax(dwMax);
            else
                QuickEverything32.Everything_SetMax(dwMax);
        }

        public static void Everything_SetOffset(int dwOffset)
        {
            if (s_is64Bit)
                QuickEverything64.Everything_SetOffset(dwOffset);
            else
                QuickEverything32.Everything_SetOffset(dwOffset);
        }

        public static bool Everything_GetMatchPath()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetMatchPath();
            else
                return QuickEverything32.Everything_GetMatchPath();
        }

        public static bool Everything_GetMatchCase()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetMatchCase();
            else
                return QuickEverything32.Everything_GetMatchCase();
        }

        public static bool Everything_GetMatchWholeWord()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetMatchWholeWord();
            else
                return QuickEverything32.Everything_GetMatchWholeWord();
        }

        public static bool Everything_GetRegex()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetRegex();
            else
                return QuickEverything32.Everything_GetRegex();
        }

        public static UInt32 Everything_GetMax()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetMax();
            else
                return QuickEverything32.Everything_GetMax();
        }

        public static UInt32 Everything_GetOffset()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetOffset();
            else
                return QuickEverything32.Everything_GetOffset();
        }

        public static string Everything_GetSearchW()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetSearchW();
            else
                return QuickEverything32.Everything_GetSearchW();
        }

        public static int Everything_GetLastError()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetLastError();
            else
                return QuickEverything32.Everything_GetLastError();
        }

        public static bool Everything_QueryW(bool bWait)
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_QueryW(bWait);
            else
                return QuickEverything32.Everything_QueryW(bWait);
        }

        public static void Everything_SortResultsByPath()
        {
            if (s_is64Bit)
                QuickEverything64.Everything_SortResultsByPath();
            else
                QuickEverything32.Everything_SortResultsByPath();
        }

        public static int Everything_GetNumFileResults()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetNumFileResults();
            else
                return QuickEverything32.Everything_GetNumFileResults();
        }

        public static int Everything_GetNumFolderResults()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetNumFolderResults();
            else
                return QuickEverything32.Everything_GetNumFolderResults();
        }

        public static int Everything_GetNumResults()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetNumResults();
            else
                return QuickEverything32.Everything_GetNumResults();
        }

        public static int Everything_GetTotFileResults()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetTotFileResults();
            else
                return QuickEverything32.Everything_GetTotFileResults();
        }

        public static int Everything_GetTotFolderResults()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetTotFolderResults();
            else
                return QuickEverything32.Everything_GetTotFolderResults();
        }

        public static int Everything_GetTotResults()
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_GetTotResults();
            else
                return QuickEverything32.Everything_GetTotResults();
        }

        public static bool Everything_IsVolumeResult(int nIndex)
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_IsVolumeResult(nIndex);
            else
                return QuickEverything32.Everything_IsVolumeResult(nIndex);
        }

        public static bool Everything_IsFolderResult(int nIndex)
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_IsFolderResult(nIndex);
            else
                return QuickEverything32.Everything_IsFolderResult(nIndex);
        }

        public static bool Everything_IsFileResult(int nIndex)
        {
            if (s_is64Bit)
                return QuickEverything64.Everything_IsFileResult(nIndex);
            else
                return QuickEverything32.Everything_IsFileResult(nIndex);
        }

        public static void Everything_GetResultFullPathNameW(int nIndex, StringBuilder lpString, int nMaxCount)
        {
            if (s_is64Bit)
                QuickEverything64.Everything_GetResultFullPathNameW(nIndex, lpString, nMaxCount);
            else
                QuickEverything32.Everything_GetResultFullPathNameW(nIndex, lpString, nMaxCount);
        }

        public static void Everything_Reset()
        {
            if (s_is64Bit)
                QuickEverything64.Everything_Reset();
            else
                QuickEverything32.Everything_Reset();
        }
    }
}
