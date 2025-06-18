using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class RustDragDrop : MonoBehaviour
{
    // Определение колбэка, который будет вызываться из Rust
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FileDropCallback(IntPtr filePath);

    // Импорт функции инициализации из Rust DLL
    [DllImport("RustBridge.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void init_dragdrop(FileDropCallback callback);

    // Колбэк, который Rust будет вызывать при переносе файла
    private static void OnFileDropped(IntPtr filePathPtr)
    {
        // string filePath = Marshal.PtrToStringAnsi(filePathPtr);
        // Debug.Log("📁 Файл перетащен в окно: " + filePath);

        // Здесь можно вызвать загрузку файла, отправить его в систему и т.д.
    }

    void Start()
    {
        Debug.Log("🔄 Инициализация Drag & Drop через Rust DLL...");

        try
        {
            // Регистрируем наш C# метод как callback для Rust
            init_dragdrop(OnFileDropped);
        }
        catch (Exception err)
        {
            Debug.LogError(err);
        }


        Debug.Log("✅ Drag & Drop активирован.");
    }
}