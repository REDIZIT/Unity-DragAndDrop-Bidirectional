using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [ComImport]
    [Guid("00000122-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDropTarget
    {
        [PreserveSig]
        int DragEnter(
            [In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObj,
            [In] uint grfKeyState,
            [In] POINT pt,
            [In, Out] ref DROPEFFECT pdwEffect);

        [PreserveSig]
        int DragOver(
            [In] uint grfKeyState,
            [In] POINT pt,
            [In, Out] ref DROPEFFECT pdwEffect);

        [PreserveSig]
        int DragLeave();

        [PreserveSig]
        int Drop(
            [In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObj,
            [In] uint grfKeyState,
            [In] POINT pt,
            [In, Out] ref DROPEFFECT pdwEffect);
    }
}