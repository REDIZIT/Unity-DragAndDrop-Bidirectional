using System;
using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STGMEDIUM
    {
        public TYMED tymed;
        public IntPtr hGlobal; // Or hBitmap, hMetaFilePict, etc.
        public IntPtr pUnkForRelease; // IUnknown*
    }
}