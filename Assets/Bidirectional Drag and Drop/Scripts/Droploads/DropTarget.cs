using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace REDIZIT.DragAndDrop
{
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

            //
            // Check supports or not format CF_HDROP
            //
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
                pdwEffect = DROPEFFECT.DROPEFFECT_COPY | DROPEFFECT.DROPEFFECT_MOVE;
            }
            else
            {
                pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
            }


            // Return S_OK to make Windows to begin sending DragOver invokes
            // If return S_FALSE or whatever else, then DragOver will not be invoked while you moving cursor over the Unity window
            return HRESULT.S_OK;
        }

        public int DragOver(uint grfKeyState, POINT pt, ref DROPEFFECT pdwEffect)
        {
            Debug.Log($"DragOver: pt=({pt.x},{pt.y}), grfKeyState={grfKeyState}"); // Частое логирование, может быть шумным

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
                pdwEffect = DROPEFFECT.DROPEFFECT_COPY;
            }
            else
            {
                pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
            }

            bool canHandle = EditorDragAndDropHook.CanHandleDropHover(new()
            {
                screenPos = POINT.MonitorToGameScreenSpace(hwnd, pt),
                monitorPoint = pt
            });
            return canHandle ? HRESULT.S_OK : HRESULT.S_FALSE;
        }

        public int DragLeave()
        {
            Debug.Log("DragLeave");
            currentDataObject = null;
            return HRESULT.S_OK;
        }

        public int Drop(IDataObject pDataObj, uint grfKeyState, POINT pt, ref DROPEFFECT pdwEffect)
        {
            Debug.Log($"Drop: pt=({pt.x},{pt.y}), grfKeyState={grfKeyState}, pdwEffect={pdwEffect}");

            currentDataObject = null;

            pdwEffect = DROPEFFECT.DROPEFFECT_COPY;

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

                    // IMPORTANT: WinAPI.ReleaseStgMedium will release memory, if pUnkForRelease is not null.
                    // In this case, pUnkForRelease == IntPtr.Zero, so GlobalFree will be invoked.
                    WinAPI.ReleaseStgMedium(ref stgMedium);


                    FilesDropEvent e = new()
                    {
                        pathes = result,
                        screenPos = POINT.MonitorToGameScreenSpace(hwnd, pt),
                        windowPoint = pt
                    };
                    EditorDragAndDropHook.onFilesDropped?.Invoke(e);

                    return HRESULT.S_OK;
                }
            }
            else
            {
                Debug.LogError($"IDataObject.GetData failed with HRESULT: {hr}");
            }

            pdwEffect = DROPEFFECT.DROPEFFECT_NONE;
            return HRESULT.DV_E_FORMATETC;
        }
    }
}