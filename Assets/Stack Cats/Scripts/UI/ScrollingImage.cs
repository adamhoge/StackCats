using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(Mask))]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    [ExecuteAlways]
    public class ScrollingImage : MonoBehaviour
    {
        public Image TargetImage;
        public float ScrollX;
        public float ScrollY;
        public bool Preview = true;

        [SerializeField]
        [HideInInspector]
        private Sprite _targetSprite;

        [SerializeField]
        [HideInInspector]
        private Vector2 _targetSpriteSize;

        private RectTransform _rectTransform;
        private Vector2 _rectSize;

        protected void Start()
        {
            UpdateSprite();
            ResizeImage();
        }

        protected void Update()
        {
            bool resizeImage = false;

            if (!Preview || !TargetImage || !TargetImage.sprite || (ScrollX == 0 && ScrollY == 0)) return;

            if (TargetImage.sprite != _targetSprite)
            {
                UpdateSprite();
                resizeImage = true;
            }

            if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();

            Vector2 rectSize = _rectTransform.rect.size;

            if (rectSize == Vector2.zero) return;

            if (_rectSize != rectSize)
            {
                _rectSize = rectSize;
                resizeImage = true;
            }

            if (resizeImage) ResizeImage();

            float offsetX, offsetY;
            if (ScrollX != 0)
            {
                int sign = ScrollX > 0 ? 1 : -1;
                offsetX = _targetSpriteSize.x / 2 * -sign + Mathf.Abs(ScrollX * Time.time) % _targetSpriteSize.x * sign;
            }
            else
            {
                offsetX = 0.0f;
            }

            if (ScrollY != 0)
            {
                int sign = ScrollY > 0 ? 1 : -1;
                offsetY = _targetSpriteSize.y / 2 * -sign + Mathf.Abs(ScrollY * Time.time) % _targetSpriteSize.y * sign;
            }
            else
            {
                offsetY = 0.0f;
            }

            TargetImage.transform.localPosition = new Vector3(offsetX, offsetY, 0.0f);
        }

        private void ResizeImage()
        {
            TargetImage.rectTransform.sizeDelta = _rectSize + _targetSpriteSize;
        }

        private void UpdateSprite()
        {
            _targetSprite = TargetImage.sprite;
            _targetSpriteSize = _targetSprite.rect.size;
            CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
            if (canvasScaler) _targetSpriteSize *= (canvasScaler.referencePixelsPerUnit / _targetSprite.pixelsPerUnit);
            TargetImage.type = Image.Type.Tiled;
            //Debug.Log("");
            //Debug.Log("Sprite Changed");
            //Debug.Log("--------------");
            //Debug.Log(".rect.size: " + _targetSprite.rect.size);
            //Debug.Log(".textureRect.size: " + _targetSprite.textureRect.size);
            //Debug.Log(".pixelsPerUnit: " + _targetSprite.pixelsPerUnit);
            //Debug.Log(".bounds: " + _targetSprite.bounds);
        }
    }
}