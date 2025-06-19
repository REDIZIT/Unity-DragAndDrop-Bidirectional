using System;
using System.Runtime.InteropServices;

public static class Window
{
    [DllImport("user32.dll")]
    public static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);
    public static string GetClassName(IntPtr hWnd)
    {
        var sb = new System.Text.StringBuilder(256);
        int count = GetClassName(hWnd, sb, 256);
        return sb.ToString(0, count);
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowTextLength(IntPtr hWnd);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);
    public static string GetWindowText(IntPtr hWnd)
    {
        int length = GetWindowTextLength(hWnd) + 2;
        var sb = new System.Text.StringBuilder(length);
        int count = GetWindowText(hWnd, sb, length);
        return sb.ToString(0, count);
    }
}