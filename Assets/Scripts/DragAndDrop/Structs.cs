using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

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

    /// <summary>
    /// Windows' Window coordinates (zero at the top-left corner) to Unity's Window coordinates (zero at bottom-left corner)
    /// </summary>
    public static Vector2Int WindowToUnityScreenSpace(POINT point)
    {
        return new Vector2Int(point.x, Screen.height - point.y);
    }

    /// <summary>
    /// Windows' monitor coordinates (zero at top-left corner of the monitor) to Unity's Window coordinates (zero at bottom-left conrner of the window)
    /// </summary>
    public static Vector2Int MonitorToUnityScreenSpace(IntPtr hwnd, POINT monitorPoint)
    {
        if (hwnd == IntPtr.Zero)
        {
            Debug.LogError("MonitorToUnityWindowSpace: Window handle is null.");
            return default;
        }

        POINT clientPoint = monitorPoint;

        if (!WinAPI.ScreenToClient(hwnd, ref clientPoint))
        {
            Debug.LogError($"MonitorToUnityWindowSpace: Failed to convert screen to client coordinates. Error: {Marshal.GetLastWin32Error()}");
            return default;
        }

        Vector2Int screenPos = new(clientPoint.x, Screen.height - clientPoint.y);
        Vector2Int magicOffset = new(0, -1);

        return screenPos + magicOffset;
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


[StructLayout(LayoutKind.Sequential)]
public struct FORMATETC
{
    public CFFORMAT cfFormat;
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
public struct STATDATA
{
    public FORMATETC formatetc;
    public ADVF advf;
    [MarshalAs(UnmanagedType.Interface)] public IAdviseSink pAdvSink; // IAdviseSink*
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