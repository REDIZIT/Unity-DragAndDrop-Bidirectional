using System;
using System.Runtime.InteropServices;

public static class WinAPI
{
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
        [In, MarshalAs(UnmanagedType.IUnknown)] IDataObject pDataObject,
        [In, MarshalAs(UnmanagedType.IUnknown)] IDropSource pDropSource,
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

    // Флаги для GlobalAlloc
    public const uint GMEM_MOVEABLE = 0x0002;
    public const uint GMEM_ZEROINIT = 0x0040;

    // Инициализация COM (часто не требуется, т.к. Unity это делает, но хорошая практика)
    [DllImport("ole32.dll")]
    public static extern int OleInitialize(IntPtr pvReserved);
    [DllImport("ole32.dll")]
    public static extern void OleUninitialize(); // Для корректного завершения, если OleInitialize был вызван
}