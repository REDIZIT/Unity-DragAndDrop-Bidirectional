// WH_CALLWNDPROC

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct CWPSTRUCT
{
    public IntPtr lParam;
    public IntPtr wParam;
    public WM message;
    public IntPtr hwnd;
}

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int x, y;
    public POINT(int aX, int aY)
    {
        x = aX;
        y = aY;
    }
    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}

//WH_GETMESSAGE
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

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left, Top, Right, Bottom;

    public RECT(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    public override string ToString()
    {
        return "(" + Left + ", " + Top + ", " + Right + ", " + Bottom + ")";
    }
}


// Добавленные Structs для Drag & Drop
[StructLayout(LayoutKind.Sequential)]
public struct FORMATETC
{
    public short cfFormat;
    public IntPtr ptd; // Pointer to DVTARGETDEVICE, often null for simple data
    public DVASPECT dwAspect;
    public TYMED tymed;
    public int lindex;
}

[StructLayout(LayoutKind.Sequential)]
public struct STGMEDIUM
{
    public TYMED tymed;
    public IntPtr hGlobal; // Or hBitmap, hMetaFilePict, etc.
    public IntPtr pUnkForRelease; // IUnknown*
}

[StructLayout(LayoutKind.Sequential)]
public struct STATDATA // НОВАЯ СТРУКТУРА
{
    public FORMATETC formatetc;
    public ADVF advf;
    [MarshalAs(UnmanagedType.Interface)] // Это IAdviseSink*
    public IAdviseSink pAdvSink;
    public uint dwConnection;
}

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