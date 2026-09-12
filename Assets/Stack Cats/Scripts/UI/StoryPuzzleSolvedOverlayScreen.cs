using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class StoryPuzzleSolvedOverlayScreen : PuzzleSolvedOverlayScreen
    {
        public StoryPuzzleScene StoryPuzzleScene;
        public StoryPuzzle StoryPuzzle;
        public int NumMovesMade;
        public Dictionary<Currency, int> CurrencyEarned;
        public CanvasGroup MoveCountLabelCanvasGroup;
        public CanvasGroup MoveCountTextCanvasGroup;
        public TextMeshProUGUI MoveCountText;
        public RectTransform StarRequirementsContainer;
        public StarEarnedUI StarsEarnedPrefab;
        public AudioEvent StarEarnedSoundEffect;

        private PuzzleManager _puzzleManager;
        private float _transitionInTime;
        private List<StarEarnedUI> _starsEarned = new List<StarEarnedUI>();
        private bool _starsEarnedDisplayed;
        private bool _isExiting;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            MoveCountLabelCanvasGroup.alpha = 0.0f;
            MoveCountTextCanvasGroup.alpha = 0.0f;
            MoveCountText.text = NumMovesMade + " moves";

            StarRequirementsContainer.gameObject.SetActive(false);

            foreach (StarEarnedUI starEarned in _starsEarned)
            {
                Destroy(starEarned.gameObject);
            }
            _starsEarned.Clear();

            _isExiting = false;

            _transitionInTime = Time.time;
        }

        public override void OnActive()
        {
            base.OnActive();

            LeanTween.alphaCanvas(MoveCountLabelCanvasGroup, 1.0f, 0.0f).setDelay(0.5f).setEase(LeanTweenType.easeOutQuint);
            LeanTween.alphaCanvas(MoveCountTextCanvasGroup, 1.0f, 0.0f).setDelay(0.75f).setEase(LeanTweenType.easeOutQuint);
            LeanTween.scale(MoveCountLabelCanvasGroup.gameObject, Vector2.one * 1.25f, 0.5f).setDelay(0.5f).setEase(LeanTweenType.punch);
            LeanTween.scale(MoveCountTextCanvasGroup.gameObject, Vector2.one * 1.25f, 0.5f).setDelay(0.75f).setEase(LeanTweenType.punch);
        }

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected new void Update()
        {
            base.Update();

            if (_isActive)
            {
                if (_starsEarnedDisplayed && Input.GetMouseButtonDown(0))
                {
                    if (_starsEarned.Any(s => !s.IsAnimationExecuted))
                    {
                        bool shouldPlaySound = false;
                        foreach (StarEarnedUI starEarned in _starsEarned)
                        {
                            if (!starEarned.IsAnimationExecuted)
                            {
                                starEarned.SkipAnimation();
                                shouldPlaySound = shouldPlaySound || starEarned.NumMovesMade <= starEarned.MoveRequirement;
                            }
                        }

                        if (shouldPlaySound)
                        {
                            GameManager.Instance.Audio.PlaySoundEffect(StarEarnedSoundEffect);
                        }
                    }
                    else if (!_isExiting && _starsEarned.All(s => s.AnimationExecutedAt + 0.5f < Time.time))
                    {
                        StoryPuzzleScene.StopPlaying();
                        _isExiting = true;
                    }
                }
            }

            if (!_starsEarnedDisplayed && Time.time > _transitionInTime + 2.0f)
            {
                StarRequirementsContainer.gameObject.SetActive(true);

                for (int i = 0; i < StoryPuzzle.StarMoveRequirements.Count; i++)
                {
                    int moveRequirement = StoryPuzzle.StarMoveRequirements[i];

                    StarEarnedUI starEarned = Instantiate(StarsEarnedPrefab, StarRequirementsContainer);
                    starEarned.MoveRequirement = moveRequirement;
                    starEarned.NumMovesMade = NumMovesMade;
                    starEarned.Delay = 0.25f + i * 0.25f;

                    _starsEarned.Add(starEarned);
                }

                _starsEarnedDisplayed = true;
            }
        }
    }
}