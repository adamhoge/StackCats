using UnityEngine;

namespace Tofuwu.StackCats
{
    public static class RectTransformHelpers
    {
        public static void SetPivotTop(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(0.5f, 1.0f);
        }

        public static void SetPivotTopRight(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(1.0f, 1.0f);
        }

        public static void SetPivotRight(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(1.0f, 0.5f);
        }

        public static void SetPivotBottomRight(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(1.0f, 0.0f);
        }

        public static void SetPivotBottom(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(0.5f, 0.0f);
        }

        public static void SetPivotBottomLeft(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(0.0f, 0.0f);
        }

        public static void SetPivotLeft(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(0.0f, 0.5f);
        }

        public static void SetPivotTopLeft(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(0.0f, 1.0f);
        }

        public static bool IsPositionWithinBounds(this RectTransform rectTransform, Vector2 position)
        {
            Vector2 localPosition = rectTransform.InverseTransformPoint(Input.mousePosition);
            return rectTransform.rect.Contains(localPosition);
        }
    }
}
