using System.Runtime.InteropServices;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)] // ДОБАВЛЕНО
public class EnumFormatEtc : IEnumFORMATETC
{
    private FORMATETC[] _formats;
    private int _currentIndex;

    public EnumFormatEtc(FORMATETC[] formats)
    {
        _formats = formats;
        _currentIndex = 0;
    }

    public int Next(uint celt, FORMATETC[] rgelt, out uint pceltFetched)
    {
        pceltFetched = 0;
        if (rgelt == null) return HRESULT.E_POINTER;

        for (int i = 0; i < celt && _currentIndex < _formats.Length; i++)
        {
            rgelt[i] = _formats[_currentIndex];
            _currentIndex++;
            pceltFetched++;
        }

        return (pceltFetched == celt) ? HRESULT.S_OK : HRESULT.S_FALSE;
    }

    public int Skip(uint celt)
    {
        _currentIndex += (int)celt;
        return (_currentIndex <= _formats.Length) ? HRESULT.S_OK : HRESULT.S_FALSE;
    }

    public int Reset()
    {
        _currentIndex = 0;
        return HRESULT.S_OK;
    }

    public int Clone(out IEnumFORMATETC ppEnum)
    {
        ppEnum = new EnumFormatEtc(_formats);
        ((EnumFormatEtc)ppEnum)._currentIndex = _currentIndex;
        return HRESULT.S_OK;
    }
}