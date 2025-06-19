using System;
using System.Collections.Generic;
using UnityEngine;

public class FilesDropEvent
{
    public List<string> pathes;
    public Vector2Int screenPos;
    public POINT windowPoint;
}

public class DropHoverEvent
{
    public Vector2Int screenPos;
}

public static class UnityDragAndDropHook
{
    public static Action<FilesDropEvent> onFilesDropped;
    public static Action<DropHoverEvent> onDropHover;


    private static uint threadId;
    private static IntPtr mainWindow = IntPtr.Zero;
    private static IntPtr m_Hook;
    private static string m_ClassName = "UnityWndClass";
    private static bool s_IsDraggingOut = false; // ДОБАВЛЕНО: Флаг для отслеживания активного Drag-Out

    // attribute required for IL2CPP, also has to be a static method
    [AOT.MonoPInvokeCallback(typeof(EnumThreadDelegate))]
    private static bool EnumCallback(IntPtr W, IntPtr _)
    {
        if (Window.IsWindowVisible(W) && (mainWindow == IntPtr.Zero || (m_ClassName != null && Window.GetClassName(W) == m_ClassName)))
        {
            mainWindow = W;
        }

        return true;
    }

    public static void InstallHook()
    {
        if (Application.isEditor) return;


        threadId = WinAPI.GetCurrentThreadId();
        if (threadId > 0)
            Window.EnumThreadWindows(threadId, EnumCallback, IntPtr.Zero);

        var hModule = WinAPI.GetModuleHandle(null);
        m_Hook = WinAPI.SetWindowsHookEx(HookType.WH_GETMESSAGE, Callback, hModule, threadId);
        // Allow dragging of files onto the main window. generates the WM_DROPFILES message
        WinAPI.DragAcceptFiles(mainWindow, true);
    }

    public static void UninstallHook()
    {
        if (Application.isEditor) return;


        WinAPI.UnhookWindowsHookEx(m_Hook);
        WinAPI.DragAcceptFiles(mainWindow, false);
        m_Hook = IntPtr.Zero;
    }

    [AOT.MonoPInvokeCallback(typeof(HookProc))]
    private static IntPtr Callback(int code, IntPtr wParam, ref MSG lParam)
    {
        // Debug.Log("Callback message: " + lParam.message);

        if (code == 0 && lParam.message == WM.DROPFILES)
        {
            POINT pos;
            WinAPI.DragQueryPoint(lParam.wParam, out pos);

            uint n = WinAPI.DragQueryFile(lParam.wParam, 0xFFFFFFFF, null, 0);
            var sb = new System.Text.StringBuilder(1024);

            List<string> result = new List<string>();
            for (uint i = 0; i < n; i++)
            {
                int len = (int) WinAPI.DragQueryFile(lParam.wParam, i, sb, 1024);
                result.Add(sb.ToString(0, len));
                sb.Length = 0;
            }

            // // ИЗМЕНЕНО: Вызываем DragFinish только если это не наш собственный Drag-Out
            // if (!s_IsDraggingOut)
            // {
            //     WinAPI.DragFinish(lParam.wParam);
            //     Debug.Log("Called DragFinish for external drop.");
            // }
            // else
            // {
            //     Debug.Log("Skipping DragFinish for self-drop during active drag-out operation.");
            // }

            FilesDropEvent e = new()
            {
                pathes = result,
                screenPos = new(pos.x, Screen.height - pos.y),
                windowPoint = pos
            };
            onFilesDropped?.Invoke(e);
        }

        return WinAPI.CallNextHookEx(m_Hook, code, wParam, ref lParam);
    }



    public static void StartDragFiles(List<string> filePaths)
    {
        if (Application.isEditor) return;


        if (filePaths == null || filePaths.Count == 0)
        {
            Debug.LogWarning("No files provided for drag operation.");
            return;
        }

        bool oleInitializedByUs = false;
        int hrOle = WinAPI.OleInitialize(IntPtr.Zero);
        if (hrOle == HRESULT.S_OK || hrOle == HRESULT.S_FALSE)
        {
            oleInitializedByUs = (hrOle == HRESULT.S_OK);
        }
        else
        {
            Debug.LogError($"OleInitialize failed with HRESULT: {hrOle}");
            return;
        }

        // Создаем экземпляры объектов IDataObject и IDropSource
        // IDataObject dataObject = new FileDataObject(filePaths);
        IDataObject dataObject = new FileDataObject(filePaths);
        IDropSource dropSource = new FileDropSource();

        DROPEFFECT resultEffect;
        s_IsDraggingOut = true;
        try
        {
            // Вызываем нативную функцию DoDragDrop
            int hr = WinAPI.DoDragDrop(dataObject, dropSource,
                DROPEFFECT.DROPEFFECT_COPY | DROPEFFECT.DROPEFFECT_MOVE | DROPEFFECT.DROPEFFECT_LINK,
                out resultEffect);

            if (hr == HRESULT.DRAGDROP_S_DROP)
            {
                Debug.Log($"Drag operation completed with effect: {resultEffect}");
                // Здесь можно добавить логику, если, например, это было перемещение (DROPEFFECT_MOVE),
                // то удалить оригинальные файлы.
            }
            else if (hr == HRESULT.DRAGDROP_S_CANCEL)
            {
                Debug.Log("Drag operation cancelled.");
            }
            else
            {
                Debug.LogError($"DoDragDrop failed with HRESULT: {hr}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception during DoDragDrop: {ex.Message}");
        }
        finally
        {
            s_IsDraggingOut = false;
            if (oleInitializedByUs)
            {
                WinAPI.OleUninitialize();
            }

            // ДОБАВЛЕНО: Явно указываем сборщику мусора, что эти объекты используются
            // до завершения метода, чтобы они не были собраны во время нативного вызова.
            GC.KeepAlive(dataObject);
            GC.KeepAlive(dropSource);
        }
    }
}