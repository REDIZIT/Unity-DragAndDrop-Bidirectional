using System;

namespace REDIZIT.DragAndDrop
{
    [Flags]
    public enum ADVF : int
    {
        ADVF_NODATA = 1,
        ADVF_PRIMEFIRST = 2,
        ADVF_ONLYONCE = 4,
        ADVF_DATAONSTOP = 8,
        ADVFCACHE_NOHANDLER = 16,
        ADVFCACHE_FORCEBUILTIN = 32,
        ADVFCACHE_ONSAVE = 64
    }
}