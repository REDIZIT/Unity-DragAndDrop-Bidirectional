using UnityEngine;

namespace REDIZIT.DragAndDrop
{
    public class FPSLimiter : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}