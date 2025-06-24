using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [ComImport]
    [Guid("0000010F-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAdviseSink
    {
        [PreserveSig]
        void OnDataChange([In] ref FORMATETC pFormatEtc, [In] ref STGMEDIUM pStgMedium);
        [PreserveSig]
        void OnViewChange([In] uint dwAspect, [In] int lindex);
        [PreserveSig]
        void OnRename([In, MarshalAs(UnmanagedType.IUnknown)] object pmk); // IMoniker - ИСПРАВЛЕНО
        [PreserveSig]
        void OnSave();
        [PreserveSig]
        void OnClose();
    }
}