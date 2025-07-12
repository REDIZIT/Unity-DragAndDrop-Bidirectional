using UnityEngine;

namespace REDIZIT.DragAndDrop
{
    public class DragAndDropHookInstaller : MonoBehaviour
    {
        private void OnEnable()
        {
            EditorDragAndDropHook.InstallHook();
        }

        private void OnDisable()
        {
            EditorDragAndDropHook.UninstallHook();
        }
    }
}