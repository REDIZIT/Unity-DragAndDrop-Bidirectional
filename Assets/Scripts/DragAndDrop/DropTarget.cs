using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public class DropTarget : IDropTarget
{
    private IntPtr hwnd;
    private IDataObject currentDataObject;

    public DropTarget(IntPtr hwnd)
    {
        this.hwnd = hwnd;
    }

    public int DragEnter(IDataObject pDataObj, uint grfKeyState, POINT pt, ref DROPEFFECT pdwEffect)
    {
        Debug.Log($"DragEnter: pt=({pt.x},{pt.y}), grfKeyState={grfKeyState}, pdwEffect={pdwEffect}");

        currentDataObject = pDataObj;

        // Проверяем, поддерживается ли формат CF_HDROP
        FORMATETC formatEtc = new FORMATETC
        {
            cfFormat = CFFORMAT.CF_HDROP,
            ptd = IntPtr.Zero,
            dwAspect = DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = TYMED.TYMED_HGLOBAL
        };

        if (pDataObj.QueryGetData(ref formatEtc) == HRESULT.S_OK)
        {
            // Если формат поддерживается, устанавливаем разрешенные эффекты (например, копирование)
            pdwEffect = DROPEFFECT.DROPEFFECT_COPY | DROPEFFECT.DROPEFFECT_MOVE; // Разрешаем копирование или перемещение
        }
        else
        {
            // Если формат не поддерживается, запрещаем дроп
            pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
        }


        // Return S_OK to make Windows to begin sending DragOver invokes
        // If return S_FALSE or whatever else, then DragOver will not be invoked while you moving cursor over the Unity window
        return HRESULT.S_OK;
    }

    public int DragOver(uint grfKeyState, POINT pt, ref DROPEFFECT pdwEffect)
    {
        Debug.Log($"DragOver: pt=({pt.x},{pt.y}), grfKeyState={grfKeyState}"); // Частое логирование, может быть шумным

        // Здесь можно изменить pdwEffect в зависимости от grfKeyState (например, Shift для Link)
        // В данном случае, просто сохраняем разрешенные эффекты из DragEnter
        FORMATETC formatEtc = new FORMATETC
        {
            cfFormat = CFFORMAT.CF_HDROP,
            ptd = IntPtr.Zero,
            dwAspect = DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = TYMED.TYMED_HGLOBAL
        };

        if (currentDataObject != null && currentDataObject.QueryGetData(ref formatEtc) == HRESULT.S_OK)
        {
            // Проверяем состояние клавиш, чтобы дать обратную связь пользователю
            bool isShiftPressed = (grfKeyState & (uint)WinAPI.MK_SHIFT) != 0;
            bool isCtrlPressed = (grfKeyState & (uint)WinAPI.MK_CONTROL) != 0;

            if (isCtrlPressed && !isShiftPressed) // Ctrl для копирования
            {
                pdwEffect = DROPEFFECT.DROPEFFECT_COPY;
            }
            else if (isShiftPressed && !isCtrlPressed) // Shift для перемещения (или Link, если актуально)
            {
                pdwEffect = DROPEFFECT.DROPEFFECT_MOVE; // или DROPEFFECT.DROPEFFECT_LINK
            }
            else if (isCtrlPressed && isShiftPressed) // Ctrl+Shift для Link
            {
                pdwEffect = DROPEFFECT.DROPEFFECT_LINK;
            }
            else // По умолчанию копирование
            {
                pdwEffect = DROPEFFECT.DROPEFFECT_COPY;
            }
        }
        else
        {
            pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
        }

        bool canHandle = UnityDragAndDropHook.CanHandleDropHover(new()
        {
            screenPos = POINT.MonitorToUnityScreenSpace(hwnd, pt),
            monitorPoint = pt
        });
        return canHandle ? HRESULT.S_OK : HRESULT.S_FALSE;
    }

    public int DragLeave()
    {
        Debug.Log("DragLeave");
        currentDataObject = null; // Очищаем ссылку
        // Здесь можно вызвать событие, если нужно оповестить о выходе курсора
        return HRESULT.S_OK;
    }

    public int Drop(IDataObject pDataObj, uint grfKeyState, POINT pt, ref DROPEFFECT pdwEffect)
    {
        Debug.Log($"Drop: pt=({pt.x},{pt.y}), grfKeyState={grfKeyState}, pdwEffect={pdwEffect}");

        currentDataObject = null; // Очищаем ссылку после дропа

        // Определяем желаемый эффект дропа
        bool isShiftPressed = (grfKeyState & (uint)WinAPI.MK_SHIFT) != 0;
        bool isCtrlPressed = (grfKeyState & (uint)WinAPI.MK_CONTROL) != 0;

        if (isCtrlPressed && !isShiftPressed)
        {
            pdwEffect = DROPEFFECT.DROPEFFECT_COPY;
        }
        else if (isShiftPressed && !isCtrlPressed)
        {
            pdwEffect = DROPEFFECT.DROPEFFECT_MOVE;
        }
        else if (isCtrlPressed && isShiftPressed)
        {
            pdwEffect = DROPEFFECT.DROPEFFECT_LINK;
        }
        else
        {
            pdwEffect = DROPEFFECT.DROPEFFECT_COPY; // По умолчанию копирование
        }

        FORMATETC formatEtc = new FORMATETC
        {
            cfFormat = CFFORMAT.CF_HDROP,
            ptd = IntPtr.Zero,
            dwAspect = DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = TYMED.TYMED_HGLOBAL
        };

        STGMEDIUM stgMedium;
        int hr = pDataObj.GetData(ref formatEtc, out stgMedium);

        if (hr == HRESULT.S_OK)
        {
            if (stgMedium.tymed == TYMED.TYMED_HGLOBAL && stgMedium.hGlobal != IntPtr.Zero)
            {
                List<string> result = new List<string>();
                IntPtr hDrop = stgMedium.hGlobal;

                uint n = WinAPI.DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);
                var sb = new System.Text.StringBuilder(1024);

                for (uint i = 0; i < n; i++)
                {
                    int len = (int)WinAPI.DragQueryFile(hDrop, i, sb, 1024);
                    result.Add(sb.ToString(0, len));
                    sb.Length = 0;
                }

                // ВАЖНО: WinAPI.ReleaseStgMedium освобождает память, если pUnkForRelease не null.
                // В нашем случае pUnkForRelease == IntPtr.Zero, так что GlobalFree будет вызван.
                WinAPI.ReleaseStgMedium(ref stgMedium);


                FilesDropEvent e = new()
                {
                    pathes = result,
                    screenPos = POINT.MonitorToUnityScreenSpace(hwnd, pt),
                    windowPoint = pt
                };
                UnityDragAndDropHook.onFilesDropped?.Invoke(e);

                return HRESULT.S_OK;
            }
        }
        else
        {
            Debug.LogError($"IDataObject.GetData failed with HRESULT: {hr}");
        }

        // Если что-то пошло не так, устанавливаем эффект NONE
        pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
        return HRESULT.DV_E_FORMATETC; // Или другой соответствующий HRESULT
    }
}