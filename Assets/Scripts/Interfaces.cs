using System.Runtime.InteropServices;

[ComImport]
[Guid("00000121-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IDropSource
{
    [PreserveSig]
    int QueryContinueDrag(
        [In, MarshalAs(UnmanagedType.Bool)] bool fEscapePressed, // ИСПРАВЛЕНО (с int на bool, добавлен MarshalAs)
        [In] int grfKeyState);

    [PreserveSig]
    int GiveFeedback(
        [In] DROPEFFECT dwEffect);
}

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
        [In, MarshalAs(UnmanagedType.Bool)] bool fRelease); // ИСПРАВЛЕНО

    [PreserveSig]
    int EnumFormatEtc(
        [In] DATADIR dwDirection,
        [Out, MarshalAs(UnmanagedType.Interface)]
        out IEnumFORMATETC ppEnumFormatEtc); // ИСПРАВЛЕНО

    [PreserveSig]
    int DAdvise(
        [In] ref FORMATETC pFormatEtc,
        [In] ADVF grfAdvf,
        [In, MarshalAs(UnmanagedType.Interface)]
        IAdviseSink pAdvSink, // ИСПРАВЛЕНО
        [Out] out uint pdwConnection);

    [PreserveSig]
    int DUnadvise(
        [In] uint dwConnection);

    [PreserveSig]
    int EnumDAdvise(
        [Out, MarshalAs(UnmanagedType.Interface)]
        out IEnumSTATDATA ppEnumAdvise); // ИСПРАВЛЕНО
}

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

[ComImport]
[Guid("00000103-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IEnumFORMATETC
{
    [PreserveSig]
    int Next(
        [In] uint celt,
        [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] FORMATETC[] rgelt,
        [Out] out uint pceltFetched);

    [PreserveSig]
    int Skip([In] uint celt);

    [PreserveSig]
    int Reset();

    [PreserveSig]
    int Clone(
        [Out] out IEnumFORMATETC ppEnum);
}

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