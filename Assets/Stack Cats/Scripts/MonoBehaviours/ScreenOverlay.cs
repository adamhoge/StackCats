using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    public class ScreenOverlay : MonoBehaviour
    {
        /// <summary>
        /// The color of the screen overlay.
        /// </summary>
        public Color OverlayColor { get { return _overlay ? _overlay.color : Color.clear; } set { if(_overlay) _overlay.color = value; } }

        /// <summary>
        /// For full overlay, the sorting order value should be higher than that of any other canvas.
        /// </summary>
        public int CanvasSortingOrder = 1000;

        private Canvas _canvas;
        private Image _overlay;

        void Awake()
        {
            _canvas = new GameObject("Screen Fader Canvas").AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = CanvasSortingOrder;
            _canvas.transform.SetParent(this.transform);

            GameObject overlayObject = new GameObject("Screen Fader");
            RectTransform rt = overlayObject.AddComponent<RectTransform>();
            rt.SetParent(_canvas.transform, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            _overlay = overlayObject.AddComponent<Image>();
            _overlay.color = Color.clear;
        }

        public void SetAlpha(float a)
        {
            Color orig = OverlayColor;
            OverlayColor = new Color(orig.r, orig.g, orig.b, a);
        }

        void OnDestroy()
        {
            Destroy(_canvas);
        }
    }
}
