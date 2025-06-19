using System;
using TMPro;
using UnityEngine;

public class DropReceiver : MonoBehaviour
{
    [SerializeField] private GameObject hoveredGroup;
    [SerializeField] private TextMeshProUGUI droppedFileText;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        UnityDragAndDropHook.onFilesDropped += OnDropped;
        UnityDragAndDropHook.SubcribeOnDropHover(OnDropHover);
    }

    private void OnDisable()
    {
        UnityDragAndDropHook.onFilesDropped -= OnDropped;
        UnityDragAndDropHook.UnsubscribeOnDropHover(OnDropHover);
    }

    private void OnDropped(FilesDropEvent e)
    {
        if (IsInside(e.screenPos))
        {
            droppedFileText.text = $"Dropped files ({e.pathes.Count}):\n{string.Join(",\n", e.pathes)}";
        }
        
        hoveredGroup.SetActive(false);
    }

    private bool OnDropHover(DropHoverEvent e)
    {
        bool isInside = IsInside(e.screenPos);
        hoveredGroup.SetActive(isInside);
        return isInside;
    }

    private bool IsInside(Vector2Int screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos);
    }
}