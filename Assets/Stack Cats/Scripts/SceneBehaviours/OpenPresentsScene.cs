using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void RemovedOpenPresent();

    public class OpenPresentsScene : SceneBehaviour
    {
        public Camera Camera;
        public event PresentOpened onPresentOpened;
        public event RemovedOpenPresent onOpenPresentRemoved;

        public Present PresentPrefab;

        public bool IsOpeningPresent { get { return _currentPresent && _currentPresent.State == PresentState.Opening; } }

        public float LastPresentOpenedTime { get { return _lastPresentOpenedTime; } }

        private StuffManager _stuff;
        private CurrencyManager _currency;
        private Present _currentPresent;
        private float _lastPresentOpenedTime;

        public void GrabPresent()
        {
            List<PresentInfo> presents = _stuff.GetPresents();

            if (_currentPresent == null && presents.Count > 0)
            {
                Present grabbedPresent = Instantiate(PresentPrefab, transform);
                grabbedPresent.PresentInfo = presents[0];
                grabbedPresent.onPresentOpened += OnPresentOpened;
                grabbedPresent.transform.Translate(Vector3.up * 0.5f);
                LeanTween.moveLocal(grabbedPresent.gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutBounce);
                LeanTween.alpha(grabbedPresent.gameObject, 0.0f, 0.0f);
                LeanTween.alpha(grabbedPresent.gameObject, 1.0f, 0.25f);
                _currentPresent = grabbedPresent;
            }
        }

        public void RemoveOpenedPresent()
        {
            if (!IsOpeningPresent && Time.time > _lastPresentOpenedTime + 0.25f)
            {
                _currentPresent.onPresentOpened -= OnPresentOpened;
                LeanTween.scale(_currentPresent.gameObject, Vector3.zero, 0.1f).setDestroyOnComplete(true);
                _currentPresent = null;
                if (onOpenPresentRemoved != null) onOpenPresentRemoved();
            }
        }

        public void Interact()
        {
            if (_currentPresent)
            {
                if (_currentPresent.State == PresentState.Opened)
                {
                    RemoveOpenedPresent();
                    GrabPresent();
                }
                else
                {
                    _currentPresent.OpenPresent();
                }
            }
        }

        public void GoHome()
        {
            if (!IsOpeningPresent && Time.time > _lastPresentOpenedTime + 0.5f)
            {
                GameManager.Instance.GoHome();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _stuff = _gameManager.Stuff;
            _currency = _gameManager.Currency;
        }

        protected override void Start()
        {
            base.Start();

            _lastPresentOpenedTime = -1.0f;
            GrabPresent();
        }

        private void OnPresentOpened(Present present)
        {
            foreach (KeyValuePair<Currency, int> currency in present.PresentInfo.CurrencyContents)
            {
                _currency.ChangeCurrency(currency.Key, currency.Value);
            }
            foreach (KeyValuePair<Item, int> item in present.PresentInfo.ItemContents)
            {
                _stuff.AddItem(item.Key, item.Value);
            }
            _stuff.RemovePresent(present.PresentInfo.PresentId);
            _lastPresentOpenedTime = Time.time;
            if (onPresentOpened != null) onPresentOpened(present);
        }
    }
}