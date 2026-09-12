using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class CurrencyHeldUI : MonoBehaviour
    {
        public Image IconImage;
        public Sprite PlaceholderSprite;
        public TextMeshProUGUI AmountText;

        /// <summary>
        /// The type of currency held to be displayed.
        /// </summary>
        public Currency CurrencyType { get { return _currencyType; } set { SetCurrencyType(value); } }

        private CurrencyManager _currencyManager;
        private PuzzleManager _puzzleManager;

        [SerializeField]
        [HideInInspector]
        private Currency _currencyType;

        protected void Awake()
        {
            _currencyManager = GameManager.Instance.Currency;
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void Start()
        {
            UpdateCurrencyIcon();
            UpdateCurrencyText();
        }

        protected void OnEnable()
        {
            _currencyManager.onCurrencyChanged += OnCurrencyChanged;
        }

        protected void OnDisable()
        {
            _currencyManager.onCurrencyChanged -= OnCurrencyChanged;
        }

        private void SetCurrencyType(Currency currencyType)
        {
            if (_currencyType == currencyType) return;

            _currencyType = currencyType;
            //EditorUtility.SetDirty(this);
            if (_currencyManager)
            {
                UpdateCurrencyIcon();
                UpdateCurrencyText();
            }
        }

        private void UpdateCurrencyIcon()
        {
            CurrencyDetails currencyDetails = _currencyManager.CurrencyDetails[CurrencyType];
            if (currencyDetails.PuzzleArea && _puzzleManager.IsAreaLocked(currencyDetails.PuzzleArea))
            {
                IconImage.sprite = PlaceholderSprite;
            }
            else
            {
                IconImage.sprite = currencyDetails.IconSprite;
            }
        }

        private void UpdateCurrencyText()
        {
            CurrencyDetails currencyDetails = _currencyManager.CurrencyDetails[CurrencyType];
            if (currencyDetails.PuzzleArea && _puzzleManager.IsAreaLocked(currencyDetails.PuzzleArea))
            {
                AmountText.text = "";
            }
            else
            {
                AmountText.text = CurrencyType.ToCurrencyString(_currencyManager.GetCurrencyHeld(CurrencyType));
            }
        }

        private void OnCurrencyChanged(Currency currency, int amount, int total)
        {
            if (currency == CurrencyType)
            {
                UpdateCurrencyText();
            }
        }
    }
}