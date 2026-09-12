using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class AdInfo : MonoBehaviour
    {
        public Button WatchAdButton;
        public TextMeshProUGUI AdIntervalRemainingText;

        private AdManager _ads;

        public void WatchAnAdd()
        {
            _ads.WatchAnAd();
        }

        protected void Awake()
        {
            _ads = GameManager.Instance.Ads;
        }

        protected void Update()
        {
            if(!_ads.AdsEnabled)
            {
                WatchAdButton.gameObject.SetActive(false);
                AdIntervalRemainingText.gameObject.SetActive(false);
                return;
            }

            WatchAdButton.interactable = _ads.CanWatchAd;

            TimeSpan adIntervalRemaining = _ads.AdIntervalRemaining;
            AdIntervalRemainingText.text = adIntervalRemaining == TimeSpan.Zero ? "" : string.Format("{0:D2}:{1:D2}", adIntervalRemaining.Minutes, adIntervalRemaining.Seconds);
        }
    }
}