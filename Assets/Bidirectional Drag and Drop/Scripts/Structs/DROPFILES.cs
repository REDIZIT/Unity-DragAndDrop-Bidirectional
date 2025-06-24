using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DROPFILES
    {
        public uint pFiles; // Offset to the file list from the beginning of this structure
        public POINT pt;    // Drop point (not relevant for source)
        [MarshalAs(UnmanagedType.Bool)]
        public bool fNC;    // Non-client area (not relevant for source)
        [MarshalAs(UnmanagedType.Bool)]
        public bool fWide;  // TRUE if file names are Unicode
    }
}