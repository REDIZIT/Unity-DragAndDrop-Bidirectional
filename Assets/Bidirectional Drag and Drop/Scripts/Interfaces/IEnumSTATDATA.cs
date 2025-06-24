using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [ComImport]
    [Guid("00000105-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumSTATDATA
    {
        // ИСПРАВЛЕНО: Добавлены все методы IEnumSTATDATA
        [PreserveSig]
        int Next(
            [In] uint celt,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] STATDATA[] rgelt,
            [Out] out uint pceltFetched);

        [PreserveSig]
        int Skip([In] uint celt);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone(
            [Out] out IEnumSTATDATA ppEnum);
    }
}