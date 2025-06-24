using System;
using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public WM message;
        public IntPtr wParam;
        public IntPtr lParam;
        public ushort time;
        public POINT pt;
    }
}