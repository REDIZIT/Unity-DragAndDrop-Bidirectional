using System;
using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FORMATETC
    {
        public CFFORMAT cfFormat;
        public IntPtr ptd; // Pointer to DVTARGETDEVICE, often null for simple data
        public DVASPECT dwAspect;
        public TYMED tymed;
        public int lindex;
    }
}