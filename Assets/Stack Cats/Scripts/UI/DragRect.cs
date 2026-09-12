using UnityEngine;
using UnityEngine.EventSystems;

namespace Tofuwu.StackCats.UI
{
    public delegate void BeginDrag(PointerEventData eventData);
    public delegate void Drag(PointerEventData eventData);
    public delegate void EndDrag(PointerEventData eventData);

    public class DragRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
    {
        public event BeginDrag onBeginDrag;
        public event Drag onDrag;
        public event EndDrag onEndDrag;

        public bool IsDraggableWithinRect = false;

        private bool _isDragging;
        private bool _isButtonDragging;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isButtonDragging)
            {
                if (onDrag != null)
                {
                    onDrag(eventData);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            if (_isButtonDragging)
            {
                _isButtonDragging = false;
                if (onEndDrag != null) onEndDrag(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDragging && !_isButtonDragging)
            {
                if (onBeginDrag != null) onBeginDrag(eventData);
                _isButtonDragging = true;
            }
        }
    }
}
