using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Tofuwu.StackCats.UI
{
    public class OpenPresentsSceneUI : MonoBehaviour
    {
        public OpenPresentsScene OpenPresentsScene;
        public RectTransform PresentItemsAreaRectTransform;
        public Button BackButton;
        public TextMeshProUGUI PresentCountText;
        public Sprite YarnSprite;
        public Sprite MoniesSprite;
        public Color YarnBackgroundColor;
        public Color MoniesBackgroundColor;
        public PresentGetUI PresentGetPrefab;

        private CurrencyManager _currencyManager;
        private List<PresentGetUI> _presentGets = new List<PresentGetUI>();

        protected void OnEnable()
        {
            OpenPresentsScene.onPresentOpened += OnPresentOpened;
            OpenPresentsScene.onOpenPresentRemoved += OnOpenPresentRemoved;
        }

        protected void OnDisable()
        {
            OpenPresentsScene.onPresentOpened -= OnPresentOpened;
            OpenPresentsScene.onOpenPresentRemoved -= OnOpenPresentRemoved;
        }

        protected void Start()
        {
            UpdatePresentCountText();
        }

        protected void Update()
        {
            BackButton.interactable = !OpenPresentsScene.IsOpeningPresent && Time.time > OpenPresentsScene.LastPresentOpenedTime + 0.5f;
        }

        private void Awake()
        {
            _currencyManager = GameManager.Instance.Currency;
        }

        private void UpdatePresentCountText()
        {
            int numPresents = GameManager.Instance.Data.StuffData.GetPresents().Count;
            PresentCountText.text = numPresents > 0 ? numPresents.ToString() : "";
        }

        private void OnPresentOpened(Present present)
        {
            foreach (KeyValuePair<Currency, int> currency in present.PresentInfo.CurrencyContents)
            {
                PresentGetUI presentGet = Instantiate(PresentGetPrefab, PresentItemsAreaRectTransform);
                presentGet.PresentSprite = _currencyManager.CurrencyDetails[currency.Key].IconSprite;
                presentGet.Quantity = currency.Value;
                if (currency.Key == Currency.GoldPaw)
                {
                    presentGet.BackgroundColor = MoniesBackgroundColor;
                }
                _presentGets.Add(presentGet);
            }

            foreach (KeyValuePair<Item, int> item in present.PresentInfo.ItemContents)
            {
                PresentGetUI presentGet = Instantiate(PresentGetPrefab, PresentItemsAreaRectTransform);
                presentGet.PresentLabel = item.Key.Name;
                presentGet.PresentSprite = item.Key.Icon;
                presentGet.PresentColorShift = item.Key.ColorShift;
                presentGet.Quantity = item.Value;
                _presentGets.Add(presentGet);
            }

            Vector2 presentAreaRectSize = PresentItemsAreaRectTransform.rect.size;
            Vector2 presentScreenPosition = OpenPresentsScene.Camera.WorldToScreenPoint(present.transform.position);
            for (int i = 0; i < _presentGets.Count; i++)
            {
                PresentGetUI presentGet = _presentGets[i];
                float offset = -(_presentGets.Count - 1.0f) / 2 + i;
                Vector2 targetPosition = new Vector2(offset * presentAreaRectSize.x / _presentGets.Count, 0.0f);
                presentGet.transform.position = presentScreenPosition;
                presentGet.transform.localScale = Vector2.one * 0.5f;
                LeanTween.moveLocal(presentGet.gameObject, targetPosition, 0.5f).setEase(LeanTweenType.easeOutCubic);
                LeanTween.scale(presentGet.gameObject, Vector2.one, 0.5f).setEase(LeanTweenType.easeOutCubic);
                LeanTween.rotateAround(presentGet.PresentImage.gameObject, Vector3.up, 360.0f, 1.5f).setEase(LeanTweenType.easeOutElastic);
            }

            UpdatePresentCountText();
        }

        private void OnOpenPresentRemoved()
        {
            foreach (PresentGetUI presentGet in _presentGets)
            {
                Destroy(presentGet.gameObject);
            }
            _presentGets.Clear();
        }
    }
}