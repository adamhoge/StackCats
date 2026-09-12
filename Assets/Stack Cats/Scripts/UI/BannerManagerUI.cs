using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    public class BannerManagerUI : MonoBehaviour
    {
        public RectTransform BannerArea;
        public Image BannerCatPaw;

        private readonly Queue<BannerUI> _bannerQueue = new Queue<BannerUI>();
        private BannerUI _currentBanner;
        private float _bannerTimeElapsed;

        public void DisplayBanner(BannerUI banner)
        {
            banner.gameObject.SetActive(false);
            banner.transform.SetParent(BannerArea, false);

            _bannerQueue.Enqueue(banner);

            if (_currentBanner == null) LoadNextBanner();
        }

        protected void Update()
        {
            if (_currentBanner != null)
            {
                _bannerTimeElapsed += Time.deltaTime;

                if (_bannerTimeElapsed >= _currentBanner.Duration) { 
                    LoadNextBanner();
                }
            }
        }

        private void LoadNextBanner()
        {
            if (_currentBanner != null)
            {
                BannerUI banner = _currentBanner;
                float bannerHeight = banner.GetComponent<RectTransform>().rect.height;
                banner.transform.SetSiblingIndex(0);
                LeanTween.moveLocalY(BannerCatPaw.gameObject, 128.0f, 0.25f).setEase(LeanTweenType.easeOutSine);
                LeanTween.moveLocalY(BannerCatPaw.gameObject, 384.0f, 0.5f).setEase(LeanTweenType.easeOutQuint).setDelay(0.25f);
                LeanTween.moveLocalY(_currentBanner.gameObject, bannerHeight, 0.5f).setEase(LeanTweenType.easeOutQuint).setDelay(0.25f);
                Destroy(_currentBanner.gameObject, 0.75f);
                _currentBanner = null;
            }

            if (_bannerQueue.Count == 0) return;

            _currentBanner = _bannerQueue.Dequeue();
            BannerUI newBanner = _currentBanner;
            _currentBanner.gameObject.SetActive(true);
            _bannerTimeElapsed = 0.0f;
            _currentBanner.transform.Translate(Vector2.up * newBanner.GetComponent<RectTransform>().rect.height);
            LeanTween.moveLocalY(_currentBanner.gameObject, 0.0f, 0.5f).setEase(LeanTweenType.easeOutQuint).setDelay(0.25f);
        }
    }
}