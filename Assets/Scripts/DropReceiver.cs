using TMPro;
using UnityEngine;

public class DropReceiver : MonoBehaviour
{
    [SerializeField] private GameObject dropActiveGroup;
    [SerializeField] private TextMeshProUGUI text;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        UnityDragAndDropHook.onFilesDropped += OnDropped;
    }

    private void OnDisable()
    {
        UnityDragAndDropHook.onFilesDropped -= OnDropped;
    }

    private void OnDropped(FilesDropEvent e)
    {
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(rect, e.screenPos);
        dropActiveGroup.SetActive(isInside);
    }
}