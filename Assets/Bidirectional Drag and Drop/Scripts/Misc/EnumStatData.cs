using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class EnumStatData : IEnumSTATDATA
    {
        public int Next(uint celt, STATDATA[] rgelt, out uint pceltFetched)
        {
            pceltFetched = 0;
            // Return S_FALSE to indicate that there is no data to enumerate.
            return HRESULT.S_FALSE;
        }

        public int Skip(uint celt)
        {
            return HRESULT.S_OK; // Successfully "skipped" (since nothing happened)
        }

        public int Reset()
        {
            return HRESULT.S_OK; // Successfully "dropped"
        }

        public int Clone(out IEnumSTATDATA ppEnum)
        {
            // Return a new stub enumerator
            ppEnum = new EnumStatData();
            return HRESULT.S_OK;
        }
    }
}