using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

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

    private static List<Func<DropHoverEvent, bool>> onDropHoverHandler = new();


    private static uint threadId;
    private static IntPtr mainWindow = IntPtr.Zero;
    private static IntPtr m_Hook;
    private static string m_ClassName = "UnityWndClass";
    private static bool s_IsDraggingOut = false;

    private static DropTarget s_DropTarget; // Новое поле для IDropTarget
    private static bool s_OleInitializedByHook = false; // Флаг для отслеживания инициализации OLE

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

        // Инициализация OLE/COM
        int hrOle = WinAPI.OleInitialize(IntPtr.Zero);
        if (hrOle == HRESULT.S_OK || hrOle == HRESULT.S_FALSE)
        {
            s_OleInitializedByHook = (hrOle == HRESULT.S_OK);
            Debug.Log($"OLE Initialized by Hook: {s_OleInitializedByHook}, HRESULT: {hrOle}");
        }
        else
        {
            Debug.LogError($"OleInitialize failed in InstallHook with HRESULT: {hrOle}");
            return;
        }

        threadId = WinAPI.GetCurrentThreadId();
        if (threadId > 0)
            Window.EnumThreadWindows(threadId, EnumCallback, IntPtr.Zero);

        if (mainWindow == IntPtr.Zero)
        {
            Debug.LogError("Main window handle not found. Cannot install drag and drop hook.");
            return;
        }

        // Удаляем DragAcceptFiles, так как теперь используем IDropTarget
        // WinAPI.DragAcceptFiles(mainWindow, true); // УДАЛЕНО

        // Регистрируем наш IDropTarget
        s_DropTarget = new DropTarget(mainWindow);
        int hrRegister = WinAPI.RegisterDragDrop(mainWindow, s_DropTarget);
        if (hrRegister == HRESULT.S_OK)
        {
            Debug.Log("IDropTarget registered successfully.");
        }
        else
        {
            Debug.LogError($"Failed to register IDropTarget, HRESULT: {hrRegister}");
            s_DropTarget = null;
        }

        // Hook WH_GETMESSAGE теперь менее критичен для Drag&Drop, но может быть полезен для других сообщений.
        // Если единственная цель хука - DROPFILES, то он может быть убран.
        // Оставим его пока для примера, но WM_DROPFILES обрабатываться не будет
        // так как DragAcceptFiles убрали.
        var hModule = WinAPI.GetModuleHandle(null);
        m_Hook = WinAPI.SetWindowsHookEx(HookType.WH_GETMESSAGE, Callback, hModule, threadId);
        if (m_Hook == IntPtr.Zero)
        {
            Debug.LogError($"Failed to set WH_GETMESSAGE hook, error: {Marshal.GetLastWin32Error()}");
        }
    }

    public static void UninstallHook()
    {
        if (Application.isEditor) return;

        if (m_Hook != IntPtr.Zero)
        {
            WinAPI.UnhookWindowsHookEx(m_Hook);
            m_Hook = IntPtr.Zero;
        }

        if (s_DropTarget != null)
        {
            int hrRevoke = WinAPI.RevokeDragDrop(mainWindow);
            if (hrRevoke == HRESULT.S_OK)
            {
                Debug.Log("IDropTarget revoked successfully.");
            }
            else
            {
                Debug.LogError($"Failed to revoke IDropTarget, HRESULT: {hrRevoke}");
            }
            s_DropTarget = null;
        }

        // WinAPI.DragAcceptFiles(mainWindow, false); // УДАЛЕНО

        if (s_OleInitializedByHook)
        {
            WinAPI.OleUninitialize();
            Debug.Log("OLE Uninitialized by Hook.");
            s_OleInitializedByHook = false;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(HookProc))]
    private static IntPtr Callback(int code, IntPtr wParam, ref MSG lParam)
    {
        // Debug.Log("Callback message: " + lParam.message);

        // WM.DROPFILES больше не будет приходить, так как мы не используем DragAcceptFiles
        // Если вы видите WM.DROPFILES, это может быть связано с тем, что DragAcceptFiles был вызван
        // где-то еще или хук был установлен до его удаления.
        if (code == 0 && lParam.message == WM.DROPFILES)
        {
            Debug.LogWarning("WM_DROPFILES received, but IDropTarget is preferred. This should not happen if DragAcceptFiles is removed.");
            WinAPI.DragFinish(lParam.wParam); // Все равно освобождаем ресурсы, если пришло
            // Event will be handled by IDropTarget.Drop
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
            Debug.Log($"OLE Initialized by Drag: {oleInitializedByUs}, HRESULT: {hrOle}");
        }
        else
        {
            Debug.LogError($"OleInitialize failed in StartDragFiles with HRESULT: {hrOle}");
            return;
        }

        // Создаем экземпляры объектов IDataObject и IDropSource
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
                Debug.Log("OLE Uninitialized by Drag.");
            }

            // Явно указываем сборщику мусора, что эти объекты используются
            // до завершения метода, чтобы они не были собраны во время нативного вызова.
            GC.KeepAlive(dataObject);
            GC.KeepAlive(dropSource);
        }
    }

    public static void SubcribeOnDropHover(Func<DropHoverEvent, bool> handler)
    {
        onDropHoverHandler.Add(handler);
    }

    public static void UnsubscribeOnDropHover(Func<DropHoverEvent, bool> handler)
    {
        onDropHoverHandler.Remove(handler);
    }

    public static bool CanHandleDropHover(DropHoverEvent e)
    {
        Assert.IsTrue(onDropHoverHandler.Count > 0, "There is no any drop hover handler.");

        foreach (Func<DropHoverEvent, bool> handler in onDropHoverHandler)
        {
            if (handler(e)) return true;
        }

        return false;
    }
}