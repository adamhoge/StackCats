using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class ChallengePuzzleFailedOverlayScreen : OverlayScreen
    {
        public ChallengeRunScene ChallengeRunScene;
        public CanvasGroup BannerCanvasGroup;

        protected new void Update()
        {
            base.Update();

            if (_isActive && _activeTimeElapsed > 1.0f && Input.GetMouseButtonDown(0))
            {
                Dismiss();
                ChallengeRunScene.StopPlayingCurrentPuzzle();
            }
        }

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            LeanTween.alphaCanvas(BannerCanvasGroup, 1.0f, 0.25f).setDelay(0.5f);
            LeanTween.scale(BannerCanvasGroup.gameObject, Vector3.one * 2, 0.5f).setEase(LeanTweenType.punch).setDelay(0.5f);
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            LeanTween.alphaCanvas(BannerCanvasGroup, 0.0f, 0.25f);
            LeanTween.rotate(BannerCanvasGroup.gameObject, Vector3.right * 90.0f, 0.25f);
        }

        protected void OnEnable()
        {
            BannerCanvasGroup.alpha = 0.0f;
            BannerCanvasGroup.transform.localScale = Vector3.one;
            BannerCanvasGroup.transform.rotation = Quaternion.identity;
        }
    }
}