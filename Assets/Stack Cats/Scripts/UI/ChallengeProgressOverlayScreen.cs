using System.Linq;
using System.Collections.Generic;
using Tofuwu.StackCats.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Tofuwu.StackCats.UI
{
    public class ChallengeProgressOverlayScreen : OverlayScreen
    {
        public ChallengeRunScene ChallengeRunScene;
        public Button UsePotionButton;
        public Button PlayPuzzleButton;
        public Button EndRunButton;
        public TextMeshProUGUI DifficultyText;
        public RectTransform CompletionRectTransform;
        public ChallengePuzzleCompletionUI CompletionPrefab;
        public int CompletionItemsColumnCount = 5;
        public Image RewardBagImage;
        public Image RewardBagAreaIconImage;
        public RectTransform RewardCurrenciesRectTransform;
        public RectTransform LuckyPositionActiveRectTransform;
        public List<Sprite> RewardBagSprites;
        public float MinRewardBagHeight = 300.0f;
        public float MaxRewardBagHeight = 400.0f;
        private PuzzleManager _puzzleManager;
        private ChallengeRunModel _currentChallengeRun;
        private List<ChallengePuzzleCompletionUI> _completedPuzzleItems = new List<ChallengePuzzleCompletionUI>();

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            _currentChallengeRun = ChallengeRunScene.CurrentChallengeRun;

            bool isStarted = _currentChallengeRun.CurrentPuzzle.MovesMade > 0;

            UsePotionButton.gameObject.SetActive(ChallengeRunScene.CanUseLuckPotion);
            LuckyPositionActiveRectTransform.gameObject.SetActive(_currentChallengeRun.CurrentPuzzle.WasLuckPotionUsed);
            PlayPuzzleButton.GetComponentInChildren<TextMeshProUGUI>().text = isStarted ? "Resume Puzzle" : "Start Puzzle";
            PlayPuzzleButton.image.color = ChallengeRunScene.PuzzleArea.PuzzleTheme.UIColor;
            DifficultyText.text = "Difficulty: " + _currentChallengeRun.Difficulty.ToChallengeRunDifficultyString();
            RewardBagAreaIconImage.sprite = ChallengeRunScene.PuzzleArea.PuzzleAreaIconSprite;
            UpdatePuzzleCompletion();
            UpdateRewardsEarned();

            _puzzleManager.OneOffChallengePuzzleCompleted.ConsumeAll(OnChallengePuzzleCompleted);
        }

        public override void OnActive()
        {
            base.OnActive();
        }

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void OnEnable()
        {
            EndRunButton.onClick.AddListener(OnEndRunButtonClicked);
            ChallengeRunScene.onStateChanged += OnChallengeRunSceneStateChanged;
            ChallengeRunScene.onLuckPotionUsed += OnLuckPotionUsed;
        }

        protected void OnDisable()
        {
            EndRunButton.onClick.RemoveListener(OnEndRunButtonClicked);
            ChallengeRunScene.onStateChanged -= OnChallengeRunSceneStateChanged;
            ChallengeRunScene.onLuckPotionUsed -= OnLuckPotionUsed;
        }

        private void UpdatePuzzleCompletion()
        {
            foreach (ChallengePuzzleCompletionUI challengeCompletionItem in _completedPuzzleItems)
            {
                challengeCompletionItem.onPuzzleCompleted -= OnPuzzleCompleted;
                Destroy(challengeCompletionItem.gameObject);
            }
            _completedPuzzleItems.Clear();

            List<ChallengePuzzleCompletionModel> completedPuzzles = _currentChallengeRun.CompletedPuzzles;
            List<ChallengePuzzleReward> rewards = ChallengeRunScene.PuzzleArea.ChallengeRunRewards.Where(r => r.AtDifficulty == _currentChallengeRun.Difficulty).ToList();
            int numCompletedPuzzles = completedPuzzles.Count;
            int numCompletionItems = _puzzleManager.MaxChallengeRunPuzzles;
            for (int i = 0; i < numCompletionItems; i++)
            {
                ChallengePuzzleCompletionUI newCompletionItem = Instantiate(CompletionPrefab, CompletionRectTransform);
                newCompletionItem.CompletedColor = ChallengeRunScene.PuzzleArea.PuzzleTheme.BackgroundColor;
                newCompletionItem.PuzzleIndex = i;
                if (i < completedPuzzles.Count) newCompletionItem.IsComplete = completedPuzzles[i] != null;
                newCompletionItem.onPuzzleCompleted += OnPuzzleCompleted;
                _completedPuzzleItems.Add(newCompletionItem);
            }
        }

        private void UpdateRewardsEarned(bool excludeLastPuzzle = false)
        {
            List<ChallengePuzzleCompletionModel> completedPuzzles = _currentChallengeRun.CompletedPuzzles;
            int numCompletedPuzzles = completedPuzzles.Count;
            if (excludeLastPuzzle) --numCompletedPuzzles;
            Sprite rewardBagSprite = GetRewardBagSprite(numCompletedPuzzles);
            if (rewardBagSprite)
            {
                RewardBagImage.gameObject.SetActive(true);
                RewardBagImage.sprite = rewardBagSprite;
                float rewardImageHeight = GetRewardImageHeight(numCompletedPuzzles);
                RewardBagImage.rectTransform.sizeDelta = new Vector2(0, rewardImageHeight);
            }
            else
            {
                RewardBagImage.gameObject.SetActive(false);
            }
        }

        private Sprite GetRewardBagSprite(int numCompletedPuzzles)
        {
            if (numCompletedPuzzles <= 0) return null;

            int rewardBagInterval = 10 / (RewardBagSprites.Count - 1);
            int rewardBagSpriteIndex = numCompletedPuzzles / rewardBagInterval;
            return RewardBagSprites[rewardBagSpriteIndex];
        }

        private float GetRewardImageHeight(int numCompletedPuzzles)
        {
            float incrementalHeight = MaxRewardBagHeight - MinRewardBagHeight;
            return MinRewardBagHeight + incrementalHeight / 8 * (numCompletedPuzzles - 1);
        }

        private void OnEndRunButtonClicked()
        {
            ChallengeRunScene.EndChallengeRun();
        }

        private void OnChallengeRunSceneStateChanged(ChallengeRunSceneState state)
        {
            Dismiss();
        }

        private void OnLuckPotionUsed()
        {
            UsePotionButton.gameObject.SetActive(false);
            LuckyPositionActiveRectTransform.gameObject.SetActive(true);
        }

        private void OnChallengePuzzleCompleted(PuzzleManager.ChallengePuzzleCompletionEvent completionInfo)
        {
            if (completionInfo.CompletionType != PuzzleCompletionType.PuzzleSolved) return;

            ChallengePuzzleCompletionUI lastCompletedPuzzle = _completedPuzzleItems.Last(cp => cp.IsComplete);
            RewardBagImage.gameObject.transform.localScale = Vector3.one;
            UpdateRewardsEarned(true);
            lastCompletedPuzzle.Complete();
        }

        private void OnPuzzleCompleted(int puzzleIndex)
        {
            UpdateRewardsEarned();
            LeanTween.scale(RewardBagImage.gameObject, Vector3.one * 1.1f, 0.75f).setEase(LeanTweenType.easeOutElastic);
        }
    }
}