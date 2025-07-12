namespace REDIZIT.DragAndDrop
{
    public static class HRESULT
    {
        public const int S_OK = 0;
        public const int S_FALSE = 1;
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        public const int E_INVALIDARG = unchecked((int)0x80070057);
        public const int E_OUTOFMEMORY = unchecked((int)0x8007000E);
        public const int E_POINTER = unchecked((int)0x80004003);
        public const int DV_E_FORMATETC = unchecked((int)0x80040064);

        public const int DRAGDROP_S_CANCEL = 0x00040101;
        public const int DRAGDROP_S_DROP = 0x00040100;
        public const int DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102; // Special code for IDropSource.GiveFeedback
    }
}