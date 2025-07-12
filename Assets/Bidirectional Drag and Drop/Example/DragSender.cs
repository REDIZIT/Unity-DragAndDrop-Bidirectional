using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace REDIZIT.DragAndDrop
{
    public class DragSender : MonoBehaviour
    {
        [SerializeField] private TMP_InputField filePathField;
        [SerializeField] private TextMeshProUGUI statusText;

        private RectTransform rect;
        private Vector2 downScreenPos;
        private bool isDragged, isDragTracking;

        private const float DRAG_BEGIN_DISTANCE = 10;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (isDragged == false)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    isDragTracking = false;
                    statusText.text = "Idle";
                }

                if (Input.GetMouseButtonDown(0) && IsInside(Input.mousePosition))
                {
                    downScreenPos = Input.mousePosition;
                    isDragTracking = true;

                    statusText.text = "Tracking...";
                }
                else if (isDragTracking && Input.GetMouseButton(0))
                {
                    if (Vector2.Distance(Input.mousePosition, downScreenPos) >= DRAG_BEGIN_DISTANCE)
                    {
                        isDragged = true;
                        isDragTracking = false;

                        statusText.text = "Dragging...";
                        DragSendResult result = EditorDragAndDropHook.StartDragFiles(new List<string>()
                        {
                            filePathField.text
                        }, out DROPEFFECT effect);

                        statusText.text = "Result: " + result + "\nEffect: " + effect;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragged = false;
            }
        }

        private bool IsInside(Vector2 screenPos)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos);
        }
    }
}