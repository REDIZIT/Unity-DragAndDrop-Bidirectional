using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [ComImport]
    [Guid("00000121-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDropSource
    {
        [PreserveSig]
        int QueryContinueDrag(
            [In, MarshalAs(UnmanagedType.Bool)] bool fEscapePressed,
            [In] int grfKeyState);

        [PreserveSig]
        int GiveFeedback(
            [In] DROPEFFECT dwEffect);
    }
}