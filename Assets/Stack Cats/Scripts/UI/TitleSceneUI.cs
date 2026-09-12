using System;
using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class TitleSceneUI : MonoBehaviour
    {
        public CanvasGroup Title;
        public CanvasGroup BackgroundObjects;
        public CanvasGroup MidgroundObjects;
        public CanvasGroup ForegroundObjects;
        public TextMeshProUGUI CopyrightText;

        protected void Awake()
        {
            CopyrightText.text = $"Copyright © {DateTime.Now.Year} Rockhopper Games";
        }

        protected void Start()
        {
            // Title
            //Title.alpha = 0.0f;
            //LeanTween.alphaCanvas(Title, 1.0f, 0.15f).setDelay(0.25f);
            //Title.transform.localScale = Vector2.one * 0.9f;
            //LeanTween.scale(Title.gameObject, Vector2.one, 0.15f).setEase(LeanTweenType.easeOutQuint).setDelay(0.25f);

            // Popup graphics FG
            BackgroundObjects.alpha = 0.0f;
            //LeanTween.alphaCanvas(BackgroundObjects, 1.0f, 1.0f).setDelay(0.5f);
            //BackgroundObjects.transform.Translate(Vector3.down * 100.0f);
            //LeanTween.moveY(BackgroundObjects.gameObject, BackgroundObjects.transform.position.y + 100.0f, 1.0f).setEase(LeanTweenType.easeOutBack).setDelay(0.5f);

            // Popup graphics MG
            MidgroundObjects.alpha = 0.0f;
            //LeanTween.alphaCanvas(MidgroundObjects, 1.0f, 1.0f).setDelay(0.7f);
            //MidgroundObjects.transform.Translate(Vector3.down * 100.0f);
            //LeanTween.moveY(MidgroundObjects.gameObject, MidgroundObjects.transform.position.y + 100.0f, 1.0f).setEase(LeanTweenType.easeOutBack).setDelay(0.7f);

            // Popup graphics BG
            ForegroundObjects.alpha = 0.0f;
            //LeanTween.alphaCanvas(ForegroundObjects, 1.0f, 1.0f).setDelay(0.9f);
            //ForegroundObjects.transform.Translate(Vector3.down * 100.0f);
            //LeanTween.moveY(ForegroundObjects.gameObject, ForegroundObjects.transform.position.y + 100.0f, 1.0f).setEase(LeanTweenType.easeOutBack).setDelay(0.9f);
        }
    }
}
