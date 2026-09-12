using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public enum CatSightingUIState
    {
        NotStarted,
        DisplayingBondingProgress,
        DisplayingRewards,
        DisplayingBondedEvent,
        AllInfoDisplayed
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class CatSightingUI : MonoBehaviour
    {
        public Cat Cat;
        public Image CatPortraitImage;
        public RectTransform NewCatLabel;
        public Sprite UnopenedPresentIcon;
        public Sprite OpenedPresentIcon;
        public RectTransform BondingGuageRectTransform;
        public RectTransform PresentIconsRectTransform;
        public RectTransform PresentReceivedRectTransform;
        public FillTransform BondingGuageMaskFillTransform;
        public CanvasGroup BondingEventFaderCanvasGroup;
        public TextMeshProUGUI CatNameText;
        public Image BackgroundImage;
        public Image ScrollingBackgroundImage;
        public Color BondedColor;
        public Color NotBondedPortraitColor = new(0, 0, 0, 0.1f);
        public Sprite BondedSprite;
        public GameObject UncommonBorder;
        public GameObject RareBorder;

        public CanvasGroup CanvasGroup { get { return _canvasGroup; } }

        public CatSightingUIState State { get { return _state; } }

        private CatSightingUIState _state;
        private float _stateTimeElapsed;
        private CatManager _cats;
        private CanvasGroup _canvasGroup;
        private int _sightingsCount;
        private bool _isAlreadyBonded;
        private Dictionary<int, Image> _presentIcons = new Dictionary<int, Image>();

        public bool ShowNext()
        {
            if (_state == CatSightingUIState.AllInfoDisplayed) return false;

            switch (_state)
            {
                case CatSightingUIState.NotStarted:
                    ChangeState(CatSightingUIState.DisplayingBondingProgress);
                    break;
                case CatSightingUIState.DisplayingBondingProgress:
                    ChangeState(CatSightingUIState.DisplayingRewards);
                    break;
                case CatSightingUIState.DisplayingRewards:
                    ChangeState(_sightingsCount == Cat.BondedAt ? CatSightingUIState.DisplayingBondedEvent : CatSightingUIState.AllInfoDisplayed);
                    break;
                case CatSightingUIState.DisplayingBondedEvent:
                    ChangeState(CatSightingUIState.AllInfoDisplayed);
                    break;
            }

            return true;
        }

        protected void Awake()
        {
            _cats = GameManager.Instance.Cats;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected void Start()
        {
            if (!Cat)
            {
                Debug.LogError("No cat provided. Removing CatSightingUI object.");
                Destroy(gameObject);
            }
            else
            {
                _sightingsCount = _cats.GetSightingsCount(Cat);
                _isAlreadyBonded = _sightingsCount > Cat.BondedAt;

                if (CatPortraitImage)
                {
                    CatPortraitImage.sprite = Cat.Portrait;
                    CatPortraitImage.color = _isAlreadyBonded ? Color.white : NotBondedPortraitColor;
                }
                if (CatNameText) CatNameText.text = _isAlreadyBonded ? Cat.Name : "Name: ???";
                if (NewCatLabel) NewCatLabel.gameObject.SetActive(_sightingsCount == 0);
                if (PresentReceivedRectTransform) PresentReceivedRectTransform.gameObject.SetActive(false);
                if (UncommonBorder) UncommonBorder.SetActive(Cat.Rarity == Rarity.Uncommon);
                if (RareBorder) RareBorder.SetActive(Cat.Rarity == Rarity.Rare);

                if (_isAlreadyBonded)
                {
                    // Hide irrelevant UI stuff
                    if (BondingGuageRectTransform) BondingGuageRectTransform.gameObject.SetActive(false);
                    if (PresentIconsRectTransform) PresentIconsRectTransform.gameObject.SetActive(false);
                    ChangeState(CatSightingUIState.AllInfoDisplayed);

                    // Vertically center name
                    RectTransform nameRectTransform = CatNameText.rectTransform;
                    nameRectTransform.anchorMin = new Vector2(nameRectTransform.anchorMin.x, 0.0f);
                    nameRectTransform.anchorMax = new Vector2(nameRectTransform.anchorMax.x, 1.0f);

                    // Background stuff
                    if (BackgroundImage) BackgroundImage.color = BondedColor;
                    if (ScrollingBackgroundImage) ScrollingBackgroundImage.sprite = BondedSprite;
                }
                else
                {
                    BondingGuageMaskFillTransform.Amount = Mathf.Clamp((_sightingsCount - 1) / (float)Cat.BondedAt, 0.0f, 1.0f);

                    foreach (CatReward reward in Cat.Rewards)
                    {
                        float presentIconPositionTransform = reward.AtSightingCount / (float)Cat.BondedAt;
                        Image presentIcon = new GameObject("Present Icon").AddComponent<Image>();
                        presentIcon.transform.SetParent(PresentIconsRectTransform, false);
                        presentIcon.rectTransform.anchorMin = new Vector2(presentIconPositionTransform, 0.5f);
                        presentIcon.rectTransform.anchorMax = new Vector2(presentIconPositionTransform, 0.5f);
                        presentIcon.rectTransform.sizeDelta = Vector2.one * 64;
                        presentIcon.sprite = _sightingsCount > reward.AtSightingCount ? OpenedPresentIcon : UnopenedPresentIcon;
                        _presentIcons.Add(reward.AtSightingCount, presentIcon);
                    }
                }
            }
        }

        protected void Update()
        {
            _stateTimeElapsed += Time.deltaTime;

            switch (_state)
            {
                case CatSightingUIState.DisplayingBondingProgress:
                    if (!BondingGuageMaskFillTransform.IsFilling)
                    {
                        ShowNext();
                    }
                    break;
                case CatSightingUIState.DisplayingRewards:
                    if (_stateTimeElapsed > 1.0f)
                    {
                        ShowNext();
                    }
                    break;
                case CatSightingUIState.DisplayingBondedEvent:
                    if (_stateTimeElapsed > 1.0f)
                    {
                        ShowNext();
                    }
                    break;
            }
        }

        private void ChangeState(CatSightingUIState state)
        {
            if (_state == state) return;

            switch (_state)
            {
                case CatSightingUIState.NotStarted:
                    break;
                case CatSightingUIState.DisplayingBondingProgress:
                    BondingGuageMaskFillTransform.SetAmountImmediate(Mathf.Clamp(_sightingsCount / (float)Cat.BondedAt, 0.0f, 1.0f));
                    break;
                case CatSightingUIState.DisplayingRewards:
                    break;
            }

            _state = state;
            _stateTimeElapsed = 0.0f;

            switch (_state)
            {
                case CatSightingUIState.NotStarted:
                    break;
                case CatSightingUIState.DisplayingBondingProgress:
                    BondingGuageMaskFillTransform.Amount = Mathf.Clamp(_sightingsCount / (float)Cat.BondedAt, 0.0f, 1.0f);
                    break;
                case CatSightingUIState.DisplayingRewards:
                    if (_presentIcons.ContainsKey(_sightingsCount))
                    {
                        _presentIcons[_sightingsCount].sprite = OpenedPresentIcon;
                        if (PresentReceivedRectTransform)
                        {
                            PresentReceivedRectTransform.gameObject.SetActive(true);
                            LeanTween.scale(PresentReceivedRectTransform.gameObject, Vector3.one * 1.5f, 0.5f).setEase(LeanTweenType.punch);
                        }
                    }
                    if (_sightingsCount != Cat.BondedAt) ShowNext();
                    break;
                case CatSightingUIState.DisplayingBondedEvent:
                    if (CatPortraitImage) CatPortraitImage.color = Color.white;
                    if (BondingGuageRectTransform) BondingGuageRectTransform.gameObject.SetActive(false);
                    if (PresentIconsRectTransform) PresentIconsRectTransform.gameObject.SetActive(false);
                    if (CatNameText) CatNameText.text = Cat.Name;
                    if (BackgroundImage) BackgroundImage.color = BondedColor;
                    if (ScrollingBackgroundImage) ScrollingBackgroundImage.sprite = BondedSprite;
                    RectTransform nameRectTransform = CatNameText.rectTransform;
                    nameRectTransform.anchorMin = new Vector2(nameRectTransform.anchorMin.x, 0.0f);
                    nameRectTransform.anchorMax = new Vector2(nameRectTransform.anchorMax.x, 1.0f);
                    if (BondingEventFaderCanvasGroup)
                    {
                        BondingEventFaderCanvasGroup.alpha = 1.0f;
                        LeanTween.alphaCanvas(BondingEventFaderCanvasGroup, 0.0f, 2.0f).setEase(LeanTweenType.easeInSine);
                    }
                    break;
                case CatSightingUIState.AllInfoDisplayed:
                    break;
            }
        }
    }
}