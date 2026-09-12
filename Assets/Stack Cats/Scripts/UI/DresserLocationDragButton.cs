using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public delegate void BeginDragDresserLocation(PointerEventData eventData, int dresserPlacementIndex);
    public delegate void DragDresserLocation(PointerEventData eventData, int dresserPlacementIndex);
    public delegate void EndDragDresserLocation(PointerEventData eventData, int dresserPlacementIndex);
    public delegate void ClickDresserLocation(int dresserPlacementIndex);

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(DragRect))]
    [RequireComponent(typeof(Button))]
    public class DresserLocationDragButton : MonoBehaviour
    {
        public int DresserLocationIndex;

        public BeginDragDresserLocation onBeginDragDresserLocation;
        public DragDresserLocation onDragDresserLocation;
        public EndDragDresserLocation onEndDragDresserLocation;
        public ClickDresserLocation onClickDresserLocation;

        private RectTransform _rectTransform;
        private DragRect _dragRect;
        private Button _button;

        public bool IsPositionWithinBounds(Vector2 position)
        {
            return _rectTransform.IsPositionWithinBounds(position);
        }

        protected void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _dragRect = GetComponent<DragRect>();
            _button = GetComponent<Button>();
        }

        protected void OnEnable()
        {
            _dragRect.onBeginDrag += OnBeginDrag;
            _dragRect.onDrag += OnDrag;
            _dragRect.onEndDrag += OnEndDrag;

            _button.onClick.AddListener(OnClick);
        }

        protected void OnDisable()
        {
            _dragRect.onBeginDrag -= OnBeginDrag;
            _dragRect.onDrag -= OnDrag;
            _dragRect.onEndDrag -= OnEndDrag;

            _button.onClick.RemoveListener(OnClick);
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            if (onBeginDragDresserLocation != null) onBeginDragDresserLocation(eventData, DresserLocationIndex);
        }

        private void OnDrag(PointerEventData eventData)
        {
            if (onDragDresserLocation != null) onDragDresserLocation(eventData, DresserLocationIndex);
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            if (onEndDragDresserLocation != null) onEndDragDresserLocation(eventData, DresserLocationIndex);
        }

        private void OnClick()
        {
            if (onClickDresserLocation != null) onClickDresserLocation(DresserLocationIndex);
        }
    }
}