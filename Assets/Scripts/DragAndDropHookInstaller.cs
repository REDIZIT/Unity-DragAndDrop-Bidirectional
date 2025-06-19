using UnityEngine;

public class DragAndDropHookInstaller : MonoBehaviour
{
    private void OnEnable()
    {
        UnityDragAndDropHook.InstallHook();
    }

    private void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }
}