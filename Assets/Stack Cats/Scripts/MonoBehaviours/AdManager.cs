using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Tofuwu.StackCats
{
    public class AdManager : MonoBehaviour, IUnityAdsShowListener, IUnityAdsInitializationListener, IUnityAdsLoadListener
    {
        public string AppleAppStoreId;
        public string GooglePlayStoreId;
        public int AdMinuteInterval = 30;

        public bool CanWatchAd { get { return AdIntervalRemaining == TimeSpan.Zero; } }

        public bool AdsEnabled { get { return Advertisement.isInitialized && _isAdLoaded; } }

        private StuffManager _stuffManager;
        private string _placementId;
        private bool _isAdLoaded;

        public TimeSpan AdIntervalRemaining
        {
            get
            {
                TimeSpan intervalRemaining = _gameManager.Data.EventData.LastAdDateTime.AddMinutes(AdMinuteInterval) - DateTime.Now;
                if (intervalRemaining < TimeSpan.Zero) intervalRemaining = TimeSpan.Zero;
                return intervalRemaining;
            }
        }

        private GameManager _gameManager;

        public void WatchAnAd()
        {
            _gameManager.ConfirmAction("Watch an ad?\n(Reward: <sprite index=0>)", ConfirmWatchAd);
        }

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
            _stuffManager = _gameManager.Stuff;

        }

        protected void Start()
        {
            string gameId = "";
#if UNITY_IOS
            gameId = AppleAppStoreId;
            _placementId = "iOS_Rewarded";
#elif UNITY_ANDROID
            gameId = GooglePlayStoreId;
            _placementId = "Android_Rewarded";
#endif
            if (!string.IsNullOrEmpty(gameId))
            {
                Advertisement.Initialize(gameId, false, this);
            }
        }

        private void ConfirmWatchAd()
        {
            if (CanWatchAd)
            {
                Advertisement.Show(_placementId, this);
            }
        }

        private void AddReward()
        {
            CurrencyAmountDictionary currencyReward = new CurrencyAmountDictionary();
            ItemAmountDictionary itemReward = new ItemAmountDictionary();
            GetPresentContents(ref currencyReward, ref itemReward);
            _gameManager.Stuff.AddPresent(null, currencyReward, itemReward);
        }

        private void OnAdResult(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    AddReward();
                    _gameManager.ConfirmAction("Received <sprite index=0>!");
                    _gameManager.Data.EventData.LastAdDateTime = DateTime.Now;
                    break;
                case ShowResult.Skipped:
                    Debug.Log("No yarn for you!");
                    break;
                case ShowResult.Failed:
                    Debug.Log("Something went wrong...");
                    break;
            }
        }

        private void GetPresentContents(ref CurrencyAmountDictionary currencyReward, ref ItemAmountDictionary itemReward)
        {
            float presentValue = UnityEngine.Random.value;

            if (presentValue >= 0.99f)
            {
                Item item = _stuffManager.GetRandomAdPresentItem(ItemRarity.VeryRare, true);
                if (item)
                {
                    itemReward.Add(item, 1);
                    return;
                }
            }

            if (presentValue >= 0.96f)
            {
                Item item = _stuffManager.GetRandomAdPresentItem(ItemRarity.Rare);
                if (item)
                {
                    itemReward.Add(item, 1);
                    return;
                }
            }

            if (presentValue >= 0.9f)
            {
                Item item = _stuffManager.GetRandomAdPresentItem(ItemRarity.Uncommon);
                if (item)
                {
                    itemReward.Add(item, 1);
                    return;
                }
            }

            if (presentValue >= 0.6f)
            {
                Item item = _stuffManager.GetRandomAdPresentItem(ItemRarity.Common);
                if (item)
                {
                    itemReward.Add(item, 1);
                    return;
                }
            }

            if (presentValue >= 0.3f)
            {
                Item item = _stuffManager.GetRandomAdPresentItem(ItemRarity.VeryCommon);
                if (item)
                {
                    itemReward.Add(item, 1);
                    return;
                }
            }

            currencyReward.Add(Currency.SilverPaw, 50);
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
        }

        public void OnUnityAdsShowStart(string placementId)
        {
        }

        public void OnUnityAdsShowClick(string placementId)
        {
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            switch (showCompletionState)
            {
                case UnityAdsShowCompletionState.SKIPPED:
                    break;
                case UnityAdsShowCompletionState.COMPLETED:
                    AddReward();
                    _gameManager.ConfirmAction("Received <sprite index=0>!");
                    _gameManager.Data.EventData.LastAdDateTime = DateTime.Now;
                    break;
                case UnityAdsShowCompletionState.UNKNOWN:
                    break;
            }

            Advertisement.Load(_placementId, this);
        }

        public void OnInitializationComplete()
        {
            Advertisement.Load(_placementId, this);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            _isAdLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
        }
    }
}