using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class RectTransformExtensions
{
    public static void WrapChildren(this RectTransform rectTransform)
    {
        for (int i = 0; i < rectTransform.transform.childCount; i++)
        {
            RectTransform transform = rectTransform.transform.GetChild(0).GetComponent<RectTransform>();

            RectTransform wrapper = new GameObject(transform.name + " Wrapper").AddComponent<RectTransform>();
            wrapper.sizeDelta = transform.sizeDelta;
            wrapper.SetParent(transform.parent, false);
            transform.SetParent(wrapper, false);
            transform.localPosition = Vector2.zero;
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.offsetMin = Vector2.zero;
            transform.offsetMax = Vector2.zero;
            transform.localScale = Vector2.one;
            transform.sizeDelta = Vector2.zero;
        }
    }

    public static void FitParent(this RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.one;
    }
}
