using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace REDIZIT.DragAndDrop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x, y;

        public POINT(int aX, int aY)
        {
            x = aX;
            y = aY;
        }
        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        /// <summary>
        /// Windows' Window coordinates (zero at the top-left corner) to Unity's Window coordinates (zero at bottom-left corner)
        /// </summary>
        public static Vector2Int WindowToUnityScreenSpace(POINT point)
        {
            return new Vector2Int(point.x, Screen.height - point.y);
        }

        /// <summary>
        /// Windows' monitor coordinates (zero at top-left corner of the monitor) to Unity's Window coordinates (zero at bottom-left conrner of the window)
        /// </summary>
        public static Vector2Int MonitorToUnityScreenSpace(IntPtr hwnd, POINT monitorPoint)
        {
            if (hwnd == IntPtr.Zero)
            {
                Debug.LogError("MonitorToUnityWindowSpace: Window handle is null.");
                return default;
            }

            POINT clientPoint = monitorPoint;

            if (!WinAPI.ScreenToClient(hwnd, ref clientPoint))
            {
                Debug.LogError($"MonitorToUnityWindowSpace: Failed to convert screen to client coordinates. Error: {Marshal.GetLastWin32Error()}");
                return default;
            }

            Vector2Int screenPos = new(clientPoint.x, Screen.height - clientPoint.y);
            Vector2Int magicOffset = new(0, -1);

            return screenPos + magicOffset;
        }
    }
}