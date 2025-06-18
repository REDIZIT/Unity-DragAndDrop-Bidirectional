using System.Runtime.InteropServices;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)] // ДОБАВЛЕНО
public class EnumStatData : IEnumSTATDATA
{
    public int Next(uint celt, STATDATA[] rgelt, out uint pceltFetched)
    {
        pceltFetched = 0;
        // Возвращаем S_FALSE, чтобы указать, что нет данных для перечисления
        return HRESULT.S_FALSE;
    }

    public int Skip(uint celt)
    {
        return HRESULT.S_OK; // Успешно "пропущено" (т.к. ничего и не было)
    }

    public int Reset()
    {
        return HRESULT.S_OK; // Успешно "сброшено"
    }

    public int Clone(out IEnumSTATDATA ppEnum)
    {
        // Возвращаем новую заглушку-перечислитель
        ppEnum = new EnumStatData();
        return HRESULT.S_OK;
    }
}