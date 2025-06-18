using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)] // ДОБАВЛЕНО: Отключает автоматический Class Interface
public class FileDataObject : IDataObject
{
    private List<string> _filePaths;
    private STGMEDIUM stg;

    public FileDataObject(List<string> filePaths)
    {
        _filePaths = filePaths;

        stg = new();
        stg.tymed = TYMED.TYMED_FILE;
    }

    public int GetData(ref FORMATETC pFormatEtc, out STGMEDIUM pStgMedium)
    {
        // Debug.Log("GetData: " + pFormatEtc.cfFormat); // GetData: -16094
        //
        //
        // // pFormatEtc.cfFormat = (short)WinAPI.RegisterClipboardFormat("someformat");
        // pFormatEtc.cfFormat = 15;
        // // pFormatEtc.dwAspect = DVASPECT.DVASPECT_CONTENT;
        // pFormatEtc.dwAspect = DVASPECT.DVASPECT_CONTENT;
        // pFormatEtc.tymed = TYMED.TYMED_HGLOBAL;
        // // Debug.Log("- cfFormat: " + pFormatEtc.cfFormat);
        //
        //
        //
        // pStgMedium = stg;
        // return HRESULT.S_OK;
        pStgMedium = new STGMEDIUM();

        Debug.Log(pFormatEtc.cfFormat);

        if (pFormatEtc.cfFormat == DragDropHelper.CF_HDROP_VALUE &&
            (pFormatEtc.tymed & TYMED.TYMED_HGLOBAL) != 0)
        {
            // Calculate total size needed for all paths including null terminators
            // Each path needs a null terminator, plus one final null terminator for the list
            int totalPathsByteSize = 0;
            List<byte[]> pathByteArrays = new List<byte[]>(); // Сначала сохраним байтовые представления путей
            foreach (string path in _filePaths)
            {
                // Получаем байты строки в кодировке Unicode (UTF-16)
                byte[] pathBytes = System.Text.Encoding.Unicode.GetBytes(path);
                pathByteArrays.Add(pathBytes);
                totalPathsByteSize += pathBytes.Length + 2; // +2 для Unicode нуль-терминатора (0x0000)
            }
            totalPathsByteSize += 2; // Для финального двойного нуль-терминатора (0x0000) для всего списка

            int dropFilesHeaderSize = Marshal.SizeOf(typeof(DROPFILES));
            uint totalAllocSize = (uint)(dropFilesHeaderSize + totalPathsByteSize);

            // Allocate global memory
            IntPtr hGlobal = WinAPI.GlobalAlloc(WinAPI.GMEM_MOVEABLE | WinAPI.GMEM_ZEROINIT, (UIntPtr)totalAllocSize);
            if (hGlobal == IntPtr.Zero)
            {
                Debug.LogError("1");
                return HRESULT.E_OUTOFMEMORY;
            }

            IntPtr pGlobal = WinAPI.GlobalLock(hGlobal);
            if (pGlobal == IntPtr.Zero)
            {
                WinAPI.GlobalFree(hGlobal);
                Debug.LogError("2");
                return HRESULT.E_OUTOFMEMORY;
            }

            try
            {
                // Write DROPFILES structure
                DROPFILES df = new DROPFILES();
                df.pFiles = (uint)dropFilesHeaderSize; // Offset to the file list from start of structure
                df.fWide = true; // Use Unicode strings
                Marshal.StructureToPtr(df, pGlobal, false);

                // Write file paths
                IntPtr currentDataPtr = (IntPtr)((long)pGlobal + dropFilesHeaderSize);
                foreach (byte[] pathBytes in pathByteArrays)
                {
                    Marshal.Copy(pathBytes, 0, currentDataPtr, pathBytes.Length);
                    currentDataPtr = (IntPtr)((long)currentDataPtr + pathBytes.Length);
                    Marshal.WriteInt16(currentDataPtr, 0); // Write Unicode null terminator (0x0000)
                    currentDataPtr = (IntPtr)((long)currentDataPtr + 2); // Advance past null terminator
                }
                Marshal.WriteInt16(currentDataPtr, 0); // Final double Unicode null terminator (0x0000)

                pStgMedium.tymed = TYMED.TYMED_HGLOBAL;
                pStgMedium.hGlobal = hGlobal;
                pStgMedium.pUnkForRelease = IntPtr.Zero; // System will free this memory via GlobalFree

                Debug.LogError("4");
                return HRESULT.S_OK;
            }
            finally
            {
                WinAPI.GlobalUnlock(hGlobal); // Decrements lock count, does not free memory
            }
        }

        Debug.LogError("3");
        return HRESULT.DV_E_FORMATETC;
    }

    // Остальные методы IDataObject (большей частью заглушки)
    public int GetDataHere(ref FORMATETC pFormatEtc, ref STGMEDIUM pStgMedium)
    {
        Debug.Log("GetDataHere");
        return HRESULT.E_NOTIMPL;
    }
    public int QueryGetData(ref FORMATETC pFormatEtc)
    {
        Debug.Log("QueryGetData: " + pFormatEtc.cfFormat);
        if (pFormatEtc.cfFormat == DragDropHelper.CF_HDROP_VALUE &&
            (pFormatEtc.tymed & TYMED.TYMED_HGLOBAL) != 0)
        {
            return HRESULT.S_OK; // Формат поддерживается
        }
        return HRESULT.S_FALSE; // Формат не поддерживается
    }
    public int GetCanonicalFormatEtc(ref FORMATETC pFormatEtcIn, out FORMATETC pFormatEtcOut)
    {
        Debug.Log("GetCanonicalFormatEtc");
        pFormatEtcOut = new FORMATETC();
        return HRESULT.E_NOTIMPL;
    }

    public int SetData(ref FORMATETC pFormatEtc, ref STGMEDIUM pStgMedium, bool fRelease)
    {
        Debug.Log("SetData");
        return HRESULT.E_NOTIMPL;
    }
    public int EnumFormatEtc(DATADIR dwDirection, out IEnumFORMATETC ppEnumFormatEtc)
    {
        Debug.Log("EnumFormatEtc: " + dwDirection);
        if (dwDirection == DATADIR.DATADIR_GET)
        {
            FORMATETC[] formats = new FORMATETC[]
            {
                new FORMATETC
                {
                    cfFormat = DragDropHelper.CF_HDROP_VALUE,
                    tymed = TYMED.TYMED_HGLOBAL,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    ptd = IntPtr.Zero
                }
            };
            ppEnumFormatEtc = new EnumFormatEtc(formats);
            return HRESULT.S_OK;
        }
        ppEnumFormatEtc = null;
        return HRESULT.E_INVALIDARG;
    }

    public int DAdvise(ref FORMATETC pFormatEtc, ADVF grfAdvf, IAdviseSink pAdvSink, out uint pdwConnection)
    {
        Debug.Log("DAdvise");
        pdwConnection = 0;
        return HRESULT.E_NOTIMPL;
    }

    public int DUnadvise(uint dwConnection)
    {
        Debug.Log("DUnadvise");
        return HRESULT.E_NOTIMPL;
    }
    public int EnumDAdvise(out IEnumSTATDATA ppEnumAdvise)
    {
        Debug.Log("EnumDAdvise");
        ppEnumAdvise = new EnumStatData();
        return HRESULT.S_OK;
    }
}