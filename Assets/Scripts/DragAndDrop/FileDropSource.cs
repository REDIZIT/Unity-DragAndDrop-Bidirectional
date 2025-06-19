using System.Runtime.InteropServices;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
public class FileDropSource : IDropSource
{
    public int QueryContinueDrag(bool fEscapePressed, int grfKeyState)
    {
        // Если нажата Escape, отменяем перетаскивание
        if (fEscapePressed)
            return HRESULT.DRAGDROP_S_CANCEL;

        const int MK_LBUTTON = 0x0001; // Флаг состояния левой кнопки мыши

        // Если левая кнопка мыши отпущена (ее флаг не установлен в grfKeyState)
        // ИЛИ если правая/средняя кнопка мыши нажата (что часто означает отмену или изменение действия)
        // Возвращаем DRAGDROP_S_DROP, чтобы DoDragDrop мог завершить операцию
        // Если цель не примет дроп, DoDragDrop сам превратит это в DRAGDROP_S_CANCEL
        if ((grfKeyState & MK_LBUTTON) == 0) // Левая кнопка мыши отпущена
        {
            // Примечание: Возврат DRAGDROP_S_DROP здесь не означает, что дроп произошел,
            // а лишь сигнализирует DoDragDrop, что состояние позволяет завершить операцию.
            // DoDragDrop сам определит фактический результат.
            return HRESULT.DRAGDROP_S_DROP; // ИЗМЕНЕНО: Более явный сигнал о завершении
        }

        // Если другие кнопки мыши нажаты, это также может означать отмену
        // (например, правая кнопка во время левой - часто отмена в Drag/Drop)
        const int MK_RBUTTON = 0x0002;
        const int MK_MBUTTON = 0x0010;
        if ((grfKeyState & (MK_RBUTTON | MK_MBUTTON)) != 0) // ДОБАВЛЕНО: Проверка других кнопок
        {
            return HRESULT.DRAGDROP_S_CANCEL; // Отмена, если нажаты другие кнопки
        }


        return HRESULT.S_OK; // Продолжаем перетаскивание
    }

    public int GiveFeedback(DROPEFFECT dwEffect)
    {
        // Позволяем OLE использовать курсоры по умолчанию
        return HRESULT.DRAGDROP_S_USEDEFAULTCURSORS;
    }
}