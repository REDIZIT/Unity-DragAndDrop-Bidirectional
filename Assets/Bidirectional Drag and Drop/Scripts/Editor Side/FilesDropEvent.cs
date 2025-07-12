using System.Collections.Generic;
using UnityEngine;

namespace REDIZIT.DragAndDrop
{
    public class FilesDropEvent
    {
        public List<string> pathes;
        public Vector2Int screenPos;
        public POINT windowPoint;
    }
}