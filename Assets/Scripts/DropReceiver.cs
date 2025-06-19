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
        UnityDragAndDropHook.SubcribeOnDropHover(OnDropHover);
    }

    private void OnDisable()
    {
        UnityDragAndDropHook.onFilesDropped -= OnDropped;
        UnityDragAndDropHook.UnsubscribeOnDropHover(OnDropHover);
    }

    private void OnDropped(FilesDropEvent e)
    {
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(rect, e.screenPos);
        dropActiveGroup.SetActive(isInside);
    }

    private bool OnDropHover(DropHoverEvent e)
    {
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(rect, e.screenPos);
        Debug.Log(isInside + ", " + e.screenPos + ", input: " + Input.mousePosition + ", detla: " + (Input.mousePosition.To2D() - e.screenPos));
        return isInside;
    }
}