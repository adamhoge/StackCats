using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class StarEarnedUI : MonoBehaviour
    {
        public bool WasPreviouslyEarned;
        public int MoveRequirement;
        public int NumMovesMade;
        public TextMeshProUGUI MoveRequirementText;
        public Image StarImage;
        public Image StarBaseImage;
        public CanvasGroup StarFlashCanvasGroup;
        public Sprite PreviouslyEarnedSprite;
        public Sprite NotPreviouslyEarned;
        public AudioEvent StarEarnedSoundEffect;
        public float Delay;

        public bool IsAnimationExecuted { get { return _isAnimationExecuted; } }

        public float AnimationExecutedAt { get { return _animationExecutedAt; } }

        private float _startTime;
        private bool _isAnimationExecuted;
        private float _animationExecutedAt;

        public void SkipAnimation()
        {
            if (!_isAnimationExecuted)
            {
                _isAnimationExecuted = true;
                _animationExecutedAt = Time.time;

                if (NumMovesMade <= MoveRequirement)
                {
                    ShowStar();
                }
            }
        }

        protected void Start()
        {
            StarImage.gameObject.SetActive(false);

            MoveRequirementText.text = MoveRequirement.ToString();
            if (WasPreviouslyEarned)
            {
                StarBaseImage.sprite = PreviouslyEarnedSprite;
                StarBaseImage.color = Color.gray;
            }
            else
            {
                StarBaseImage.sprite = NotPreviouslyEarned;
                StarBaseImage.color = Color.white;
            }

            _startTime = Time.time;
        }

        protected void Update()
        {
            if (!_isAnimationExecuted && _startTime + Delay <= Time.time)
            {
                _isAnimationExecuted = true;
                _animationExecutedAt = Time.time;

                if (NumMovesMade <= MoveRequirement)
                {
                    ShowStar();
                    GameManager.Instance.Audio.PlaySoundEffect(StarEarnedSoundEffect);
                }
            }
        }

        private void ShowStar()
        {
            StarImage.gameObject.SetActive(true);
            StarFlashCanvasGroup.alpha = 1.0f;
            LeanTween.alphaCanvas(StarFlashCanvasGroup, 0.0f, 0.5f).setEase(LeanTweenType.easeOutSine);
            LeanTween.scale(StarImage.gameObject, Vector2.one * 1.5f, 0.5f).setEase(LeanTweenType.punch);
            LeanTween.scale(StarFlashCanvasGroup.gameObject, Vector2.one * 1.5f, 0.5f).setEase(LeanTweenType.punch);
        }
    }
}