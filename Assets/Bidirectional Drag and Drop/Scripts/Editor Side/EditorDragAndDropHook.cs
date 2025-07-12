using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

namespace REDIZIT.DragAndDrop
{
    public delegate IntPtr HookProc(int code, IntPtr wParam, ref MSG lParam);
    public delegate bool EnumThreadDelegate(IntPtr Hwnd, IntPtr lParam);

    public static class EditorDragAndDropHook
    {
        public static Action<FilesDropEvent> onFilesDropped;

        private static List<Func<DropHoverEvent, bool>> onDropHoverHandler = new();

        private static uint threadId;
        private static IntPtr mainWindow = IntPtr.Zero;
        private static IntPtr m_Hook;
        private static string m_ClassName = "UnityWndClass";
        private static bool s_IsDraggingOut;

        private static DropTarget s_DropTarget;
        private static bool s_OleInitializedByHook;

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

            //
            // OLE/COM initialization
            //
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

            //
            // IDropTarget registration
            //
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
            // WM.DROPFILES will no longer come up since we don't use DragAcceptFiles
            // If you see WM.DROPFILES, it may be because DragAcceptFiles was called
            // somewhere else or the hook was installed before it was removed.
            if (code == 0 && lParam.message == WM.DROPFILES)
            {
                Debug.LogWarning("WM_DROPFILES received, but IDropTarget is preferred. This should not happen if DragAcceptFiles is removed.");
                WinAPI.DragFinish(lParam.wParam); // We still free up resources if it comes
                // Event will be handled by IDropTarget.Drop
            }

            return WinAPI.CallNextHookEx(m_Hook, code, wParam, ref lParam);
        }



        public static DragSendResult StartDragFiles(List<string> filePaths, out DROPEFFECT resultEffect)
        {
            resultEffect = DROPEFFECT.DROPEFFECT_NONE;
            if (Application.isEditor)
            {
                Debug.LogError("Drag and Drop is avaiable only in Build, not in Editor");
                return DragSendResult.Error;
            }

            if (filePaths == null || filePaths.Count == 0)
            {
                Debug.LogError("No files provided for drag operation.");
                return DragSendResult.Error;
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
                return DragSendResult.Error;
            }


            IDataObject dataObject = new FileDataObject(filePaths);
            IDropSource dropSource = new FileDropSource();

            s_IsDraggingOut = true;
            try
            {
                int hr = WinAPI.DoDragDrop(dataObject, dropSource,
                    DROPEFFECT.DROPEFFECT_COPY | DROPEFFECT.DROPEFFECT_MOVE | DROPEFFECT.DROPEFFECT_LINK,
                    out resultEffect);

                if (hr == HRESULT.DRAGDROP_S_DROP)
                {
                    Debug.Log($"Drag operation completed with effect: {resultEffect}");
                    return DragSendResult.Dropped;
                }
                else if (hr == HRESULT.DRAGDROP_S_CANCEL)
                {
                    Debug.Log("Drag operation cancelled.");
                    return DragSendResult.Cancelled;
                }
                else
                {
                    Debug.LogError($"DoDragDrop failed with HRESULT: {hr}");
                    return DragSendResult.Error;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during DoDragDrop: {ex.Message}");
                return DragSendResult.Error;
            }
            finally
            {
                s_IsDraggingOut = false;
                if (oleInitializedByUs)
                {
                    WinAPI.OleUninitialize();
                    Debug.Log("OLE Uninitialized by Drag.");
                }

                // Explicitly tell the garbage collector that these objects are used before the method returns,
                // so that they are not collected during a native call.
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
}