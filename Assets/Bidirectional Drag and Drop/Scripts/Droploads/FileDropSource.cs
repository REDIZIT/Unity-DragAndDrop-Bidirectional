using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class FileDropSource : IDropSource
    {
        public int QueryContinueDrag(bool fEscapePressed, int grfKeyState)
        {
            if (fEscapePressed) return HRESULT.DRAGDROP_S_CANCEL;

            const int MK_LBUTTON = 0x0001; // Left mouse button state flag

            // If the left mouse button is released (its flag is not set in grfKeyState)
            // OR if the right/middle mouse button is pressed (which often means canceling or changing the action)
            // Return DRAGDROP_S_DROP so DoDragDrop can complete the operation
            // If the target does not accept the drop, DoDragDrop itself will turn this into DRAGDROP_S_CANCEL
            if ((grfKeyState & MK_LBUTTON) == 0) // Левая кнопка мыши отпущена
            {
                // Note: Returning DRAGDROP_S_DROP here does not mean that the drop has occurred,
                // but merely signals DoDragDrop that the state allows the operation to complete.
                // DoDragDrop will determine the actual result.
                return HRESULT.DRAGDROP_S_DROP;
            }

            // If other mouse buttons are pressed, this can also mean cancel
            // (e.g. right button during left - often cancel in Drag/Drop)
            const int MK_RBUTTON = 0x0002;
            const int MK_MBUTTON = 0x0010;
            if ((grfKeyState & (MK_RBUTTON | MK_MBUTTON)) != 0)
            {
                return HRESULT.DRAGDROP_S_CANCEL; // Cancel if other buttons are pressed
            }


            return HRESULT.S_OK; // Continue dragging
        }

        public int GiveFeedback(DROPEFFECT dwEffect)
        {
            // Let OLE use default cursors
            return HRESULT.DRAGDROP_S_USEDEFAULTCURSORS;
        }
    }
}