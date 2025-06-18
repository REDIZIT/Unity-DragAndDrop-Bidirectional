using System;
using System.Runtime.InteropServices;
using System.Text;

public static class RustPlugin
{
    private const string DllName = "unity_drag_drop_plugin"; // Имя Rust DLL

    // ---------------------------------------------------
    // Common Plugin Functions
    // ---------------------------------------------------
    [DllImport(DllName)]
    public static extern void initialize_plugin();

    // ---------------------------------------------------
    // RECEIVER (Drag IN to Unity) Functions
    // ---------------------------------------------------

    // Делегат для колбэка, который Rust будет вызывать для передачи файлов
    // [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)]
    // ^ это не сработает напрямую, т.к. Rust не использует CoTaskMemAlloc для строк
    // Вместо этого Rust передает массив указателей на GlobalAlloc-ные строки.
    public delegate void DroppedFilesCallback(
        IntPtr /* *const *const u16 */ filePathsPtrArray,
        int count);

    [DllImport(DllName)]
    public static extern void set_drop_callback(DroppedFilesCallback callback);

    [DllImport(DllName)]
    public static extern void free_dropped_files_memory(
        IntPtr /* *mut *mut u16 */ ptrArrayPtr,
        int count);

    [DllImport(DllName)]
    public static extern int register_unity_drop_target(IntPtr hwnd);

    [DllImport(DllName)]
    public static extern int revoke_unity_drop_target(IntPtr hwnd);

    // ---------------------------------------------------
    // SENDER (Drag OUT from Unity) Functions
    // ---------------------------------------------------

    [DllImport(DllName, CharSet = CharSet.Unicode)]
    public static extern uint start_drag_operation(
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] filePaths,
        int numFiles);

    // Constants for DragDropEffects (optional, can use Rust's direct return values)
    public enum DragDropEffects : uint
    {
        None = 0,
        Copy = 1,
        Move = 2,
        Link = 4,
        Scroll = 0x80000000 // -2147483648
    }

    // ---------------------------------------------------
    // WinAPI helper functions (if needed in C#, otherwise in Rust)
    // ---------------------------------------------------
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, StringBuilder lpszFile, uint cch);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll")]
    public static extern bool GlobalUnlock(IntPtr hMem);
}