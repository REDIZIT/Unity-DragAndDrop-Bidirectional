using System.Runtime.InteropServices;

namespace REDIZIT.DragAndDrop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        public override string ToString()
        {
            return "(" + Left + ", " + Top + ", " + Right + ", " + Bottom + ")";
        }
    }
}