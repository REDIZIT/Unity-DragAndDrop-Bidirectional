using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [ComImport]
    [Guid("0000010E-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDataObject
    {
        [PreserveSig]
        int GetData(
            [In] ref FORMATETC pFormatEtc,
            [Out] out STGMEDIUM pStgMedium);

        [PreserveSig]
        int GetDataHere(
            [In] ref FORMATETC pFormatEtc,
            [In, Out] ref STGMEDIUM pStgMedium);

        [PreserveSig]
        int QueryGetData(
            [In] ref FORMATETC pFormatEtc);

        [PreserveSig]
        int GetCanonicalFormatEtc(
            [In] ref FORMATETC pFormatEtcIn,
            [Out] out FORMATETC pFormatEtcOut);

        [PreserveSig]
        int SetData(
            [In] ref FORMATETC pFormatEtc,
            [In] ref STGMEDIUM pStgMedium,
            [In, MarshalAs(UnmanagedType.Bool)] bool fRelease);

        [PreserveSig]
        int EnumFormatEtc(
            [In] DATADIR dwDirection,
            [Out, MarshalAs(UnmanagedType.Interface)]
            out IEnumFORMATETC ppEnumFormatEtc);

        [PreserveSig]
        int DAdvise(
            [In] ref FORMATETC pFormatEtc,
            [In] ADVF grfAdvf,
            [In, MarshalAs(UnmanagedType.Interface)]
            IAdviseSink pAdvSink,
            [Out] out uint pdwConnection);

        [PreserveSig]
        int DUnadvise(
            [In] uint dwConnection);

        [PreserveSig]
        int EnumDAdvise(
            [Out, MarshalAs(UnmanagedType.Interface)]
            out IEnumSTATDATA ppEnumAdvise);
    }
}