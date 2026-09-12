using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public delegate void BeginDragPlaceableObjectItem(PointerEventData eventData, PlaceableObjectItem PlaceableObjectItem);
    public delegate void DragPlaceableObjectItem(PointerEventData eventData, PlaceableObjectItem PlaceableObjectItem);
    public delegate void EndDragPlaceableObjectItem(PointerEventData eventData, PlaceableObjectItem PlaceableObjectItem);
    public delegate void ClickPlaceableObjectItem(PlaceableObjectItem PlaceableObjectItem);

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(DragRect))]
    [RequireComponent(typeof(Button))]
    public class PlaceableObjectItemDragButton : MonoBehaviour
    {
        public BeginDragPlaceableObjectItem onBeginDragPlaceableObjectItem;
        public DragPlaceableObjectItem onDragPlaceableObjectItem;
        public EndDragPlaceableObjectItem onEndDragPlaceableObjectItem;
        public ClickPlaceableObjectItem onClickPlaceableObjectItem;

        public Image ButtonImage;
        
        public PlaceableObjectItem PlaceableObjectItem { get { return _PlaceableObjectItem; } set { SetPlaceableObjectItem(value); } }

        private PlaceableObjectItem _PlaceableObjectItem;
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

        protected void Start()
        {
            ButtonImage.sprite = PlaceableObjectItem.Icon;
            ButtonImage.SetMaterialForItem(PlaceableObjectItem.ColorShift);
        }

        private void SetPlaceableObjectItem(PlaceableObjectItem PlaceableObjectItem)
        {
            if (_PlaceableObjectItem == PlaceableObjectItem) return;

            _PlaceableObjectItem = PlaceableObjectItem;
            ButtonImage.sprite = PlaceableObjectItem.Icon;
            ButtonImage.SetMaterialForItem(PlaceableObjectItem.ColorShift);
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            if (onBeginDragPlaceableObjectItem != null) onBeginDragPlaceableObjectItem(eventData, PlaceableObjectItem);
        }

        private void OnDrag(PointerEventData eventData)
        {
            if (onDragPlaceableObjectItem != null) onDragPlaceableObjectItem(eventData, PlaceableObjectItem);
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            if (onEndDragPlaceableObjectItem != null) onEndDragPlaceableObjectItem(eventData, PlaceableObjectItem);
        }

        private void OnClick()
        {
            if (onClickPlaceableObjectItem != null) onClickPlaceableObjectItem(PlaceableObjectItem);
        }
    }
}
