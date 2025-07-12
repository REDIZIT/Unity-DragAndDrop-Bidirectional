using System;
using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    public static class WinAPI
    {
        // Key state flags used in grfKeyState
        public const uint MK_LBUTTON = 0x0001;
        public const uint MK_RBUTTON = 0x0002;
        public const uint MK_SHIFT = 0x0004;
        public const uint MK_CONTROL = 0x0008;
        public const uint MK_MBUTTON = 0x0010;
        public const uint MK_ALT = 0x0020; // This is not a standard MK_* flag, but is often used

        // Flags for GlobalAlloc
        public const uint GMEM_MOVEABLE = 0x0002;
        public const uint GMEM_ZEROINIT = 0x0040;


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref MSG lParam);

        [DllImport("shell32.dll")]
        public static extern void DragAcceptFiles(IntPtr hwnd, bool fAccept);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, System.Text.StringBuilder lpszFile, uint cch);

        [DllImport("shell32.dll")]
        public static extern void DragFinish(IntPtr hDrop);

        [DllImport("shell32.dll")]
        public static extern void DragQueryPoint(IntPtr hDrop, out POINT pos);

        [DllImport("ole32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int DoDragDrop(
            [In, MarshalAs(UnmanagedType.IUnknown)]
            IDataObject pDataObject,
            [In, MarshalAs(UnmanagedType.IUnknown)]
            IDropSource pDropSource,
            [In] DROPEFFECT dwOKEffects,
            [Out] out DROPEFFECT pdwEffect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern ushort RegisterClipboardFormat(string lpszFormat);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalFree(IntPtr hMem);



        // Initialize COM (often not needed since Unity does it, but good practice)
        [DllImport("ole32.dll")]
        public static extern int OleInitialize(IntPtr pvReserved);

        [DllImport("ole32.dll")]
        public static extern void OleUninitialize(); // To terminate gracefully if OleInitialize was called

        [DllImport("ole32.dll", ExactSpelling = true)]
        public static extern int RegisterDragDrop(
            [In] IntPtr hwnd,
            [In, MarshalAs(UnmanagedType.Interface)]
            IDropTarget pDropTarget);

        [DllImport("ole32.dll", ExactSpelling = true)]
        public static extern int RevokeDragDrop(
            [In] IntPtr hwnd);

        // Add this function to properly release STGMEDIUM
        [DllImport("ole32.dll")]
        public static extern void ReleaseStgMedium(ref STGMEDIUM pStgMedium);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);
    }
}