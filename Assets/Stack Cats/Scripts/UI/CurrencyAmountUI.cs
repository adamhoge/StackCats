using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class CurrencyAmountUI : MonoBehaviour
    {
        public Image IconImage;
        public TextMeshProUGUI AmountText;

        /// <summary>
        /// The type of currency held to be displayed.
        /// </summary>
        public Currency CurrencyType { get { return _currencyType; } set { SetCurrencyType(value); } }

        /// <summary>
        /// The type of currency held to be displayed.
        /// </summary>
        public int Amount { get { return _amount; } set { SetAmount(value); } }

        private CurrencyManager _currencyManager;

        [SerializeField]
        [HideInInspector]
        private Currency _currencyType;

        [SerializeField]
        [HideInInspector]
        private int _amount;

        protected void Awake()
        {
            _currencyManager = GameManager.Instance.Currency;
        }

        protected void Start()
        {
            IconImage.sprite = _currencyManager.CurrencyDetails[CurrencyType].IconSprite;
            AmountText.text = CurrencyType.ToCurrencyString(_amount);
        }

        private void SetCurrencyType(Currency currencyType)
        {
            if (_currencyType == currencyType) return;

            _currencyType = currencyType;
            //EditorUtility.SetDirty(this);
            if (_currencyManager)
            {
                IconImage.sprite = _currencyManager.CurrencyDetails[CurrencyType].IconSprite;
            }
        }

        private void SetAmount(int amount)
        {
            if (_amount == amount) return;

            _amount = amount;
            //EditorUtility.SetDirty(this);
            if (_currencyManager)
            {
                AmountText.text = CurrencyType.ToCurrencyString(_amount);
            }
        }
    }
}