using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats
{
    public class CurrencyUI : MonoBehaviour
    {
        public TextMeshProUGUI YarnText;
        public TextMeshProUGUI MoniesText;
        public float IncrementDelay = 0.1f;

        private CurrencyManager _currency;
        private int _lastYarn;
        private int _curYarn;
        private int _lastMonies;
        private int _curMonies;
        private float _lastYarnIncrement;
        private float _lastMoniesIncrement;

        protected void Awake()
        {
            _currency = GameManager.Instance.Currency;
        }

        protected void Update()
        {
            if (_lastYarn != _curYarn)
            {
                if (IncrementDelay == 0)
                {
                    _lastYarn = _curYarn;
                }
                else if (Time.time > _lastYarnIncrement + IncrementDelay)// / Mathf.Abs(_lastYarn - _curYarn) * 2.0f)
                {
                    _lastYarnIncrement = Time.time;
                    _lastYarn += _lastYarn < _curYarn ? 1 : -1;
                }
                if (YarnText) YarnText.text = CurrencyHelper.ToCurrencyString(Currency.SilverPaw, _lastYarn);

            }

            if (_lastMonies != _curMonies)
            {
                if (IncrementDelay == 0)
                {
                    _lastMonies = _curMonies;
                }
                else if (Time.time > _lastMoniesIncrement + IncrementDelay)// / Mathf.Abs(_lastMonies - _curMonies) * 2.0f)
                {
                    _lastMoniesIncrement = Time.time;
                    _lastMonies += _lastMonies < _curMonies ? 1 : -1;
                    if (MoniesText) MoniesText.text = string.Format("{0:n0}", _lastMonies);
                }
            }
        }

        protected void OnEnable()
        {
            _currency.onCurrencyChanged += OnCurrencyChanged;

            _curYarn = _currency.GetCurrencyHeld(Currency.SilverPaw);
            _lastYarn = _curYarn;
            _curMonies = _currency.GetCurrencyHeld(Currency.GoldPaw);
            _lastMonies = _curMonies;
            if (YarnText) YarnText.text = CurrencyHelper.ToCurrencyString(Currency.SilverPaw, _curYarn);
            if (MoniesText) MoniesText.text = string.Format("{0:n0}", _curMonies);
        }

        protected void OnDisable()
        {
            _currency.onCurrencyChanged -= OnCurrencyChanged;
        }

        private void OnCurrencyChanged(Currency currency, int amount, int total)
        {
            switch (currency)
            {
                case Currency.SilverPaw:
                    _curYarn = total;
                    break;
                case Currency.GoldPaw:
                    _curMonies = total;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("currency", currency, null);
            }
        }
    }
}