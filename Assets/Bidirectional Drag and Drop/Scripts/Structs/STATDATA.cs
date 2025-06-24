using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STATDATA
    {
        public FORMATETC formatetc;
        public ADVF advf;
        [MarshalAs(UnmanagedType.Interface)] public IAdviseSink pAdvSink; // IAdviseSink*
        public uint dwConnection;
    }
}