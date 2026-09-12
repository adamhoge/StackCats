using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats.UI
{
    public class CatSightingsOverlayScreen : OverlayScreen
    {
        public List<Cat> CatsSighted;
        public CatSightingUI CatSightingPrefab;
        public RectTransform CatSightingsContainer;

        private List<CatSightingUI> _catSightings;
        private bool _isInfoStarted;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            _isInfoStarted = false;
            _catSightings = new List<CatSightingUI>();
            CatsSighted = CatsSighted.OrderBy(c => c.Name).ToList();
            foreach (Cat cat in CatsSighted)
            {
                CatSightingUI newCatSightingItem = Instantiate(CatSightingPrefab, CatSightingsContainer);
                newCatSightingItem.Cat = cat;
                _catSightings.Add(newCatSightingItem);
            }

            CatSightingsContainer.WrapChildren();

            float delay = TransitionInDuration / 2;
            for (int i = 0; i < _catSightings.Count; i++)
            {
                CatSightingUI catSighting = _catSightings[i];
                catSighting.transform.Translate(Vector2.right * 5.0f);
                catSighting.CanvasGroup.alpha = 0.0f;
                LeanTween.moveLocalX(catSighting.gameObject, 0.0f, TransitionInDuration - delay).setEase(TransitionInTween).setDelay(delay / 2 + i * delay / 2);
                LeanTween.alphaCanvas(catSighting.CanvasGroup, 1.0f, 0.5f).setDelay(delay / 2 + i * delay / 2);
            }
        }

        public override void OnHidden()
        {
            base.OnHidden();
            foreach (CatSightingUI catSightingUI in _catSightings)
            {
                LeanTween.cancel(catSightingUI.gameObject);
            }
            foreach (RectTransform rectTransform in CatSightingsContainer)
            {
                Destroy(rectTransform.gameObject);
            }
            _catSightings.Clear();
        }

        protected override void Update()
        {
            base.Update();

            if (_isActive && _activeTimeElapsed > 0.5f)
            {
                if (!_isInfoStarted)
                {
                    foreach (CatSightingUI catSighting in _catSightings)
                    {
                        catSighting.ShowNext();
                    }
                    _isInfoStarted = true;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    bool showingInfo = false;
                    foreach (CatSightingUI catSighting in _catSightings)
                    {
                        showingInfo = catSighting.ShowNext() || showingInfo;
                    }
                    if (!showingInfo) Dismiss();
                }
            }
        }
    }
}