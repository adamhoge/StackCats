using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class ChallengeStartOverlayScreen : OverlayScreen
    {
        public ChallengeRunScene ChallengeRunScene;
        public TextMeshProUGUI DifficultyText;
        public RectTransform RewardsRectTransform;
        public Button StartButton;
        public CurrencyAmountUI StartCostCurrencyAmount;
        public CurrencyAmountUI HeldSilverPawsCurrency;
        public CurrencyAmountUI CurrencyAmountPrefab;

        private PuzzleManager _puzzleManager;
        private CurrencyManager _currencyManager;

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
            _currencyManager = GameManager.Instance.Currency;
        }

        protected void OnEnable()
        {
            ChallengeRunScene.onDifficultyChanged += OnDifficultyChanged;
            _currencyManager.onCurrencyChanged += OnCurrencyChanged;
            StartButton.onClick.AddListener(OnStartButtonClicked);
            StartButton.interactable = ChallengeRunScene.CanPlay;
            HeldSilverPawsCurrency.Amount = _currencyManager.GetCurrencyHeld(Currency.SilverPaw);
            UpdateRewardInformation();
        }

        protected void OnDisable()
        {
            ChallengeRunScene.onDifficultyChanged -= OnDifficultyChanged;
            _currencyManager.onCurrencyChanged -= OnCurrencyChanged;
            StartButton.onClick.RemoveListener(OnStartButtonClicked);
            UpdateRewardInformation();
        }

        protected void Start()
        {
            DifficultyText.text = ChallengeRunScene.SelectedDifficulty.ToChallengeRunDifficultyString();
            StartCostCurrencyAmount.CurrencyType = Currency.SilverPaw;
            StartCostCurrencyAmount.Amount = ChallengeRunScene.PuzzleArea.ChallengeRunCost;

            StartButton.image.color = ChallengeRunScene.PuzzleArea.PuzzleTheme.UIColor;
        }

        private void OnCurrencyChanged(Currency currency, int amount, int totalMonies)
        {
            StartButton.interactable = ChallengeRunScene.CanPlay;
            if (currency == Currency.SilverPaw) HeldSilverPawsCurrency.Amount = totalMonies;
        } 

        private void OnDifficultyChanged(ChallengeRunDifficulty difficulty)
        {
            DifficultyText.text = difficulty.ToChallengeRunDifficultyString();
            UpdateRewardInformation();
        }

        private void UpdateRewardInformation()
        {
            foreach(RectTransform rectTransform in RewardsRectTransform)
            {
                Destroy(rectTransform.gameObject);
            }

            List<ChallengePuzzleReward> rewards = ChallengeRunScene.PuzzleArea.ChallengeRunRewards.Where(r => r.AtDifficulty == ChallengeRunScene.SelectedDifficulty).ToList();
            CurrencyAmountDictionary currencyRewardTotals = new CurrencyAmountDictionary();
            int goldPaws = _puzzleManager.MaxChallengeRunPuzzles - rewards.Count;
            currencyRewardTotals.Add(Currency.GoldPaw, goldPaws);
            foreach (ChallengePuzzleReward reward in rewards)
            {
                foreach(KeyValuePair<Currency, int> currencyReward in reward.CurrencyReward)
                {
                    if (currencyRewardTotals.ContainsKey(currencyReward.Key))
                    {
                        currencyRewardTotals[currencyReward.Key] += currencyReward.Value;
                    }
                    else
                    {
                        currencyRewardTotals.Add(currencyReward.Key, currencyReward.Value);
                    }
                }
            }

            foreach(KeyValuePair<Currency, int> currencyRewardTotal in currencyRewardTotals)
            {
                CurrencyAmountUI rewardAmount = Instantiate(CurrencyAmountPrefab, RewardsRectTransform);
                rewardAmount.CurrencyType = currencyRewardTotal.Key;
                rewardAmount.Amount = currencyRewardTotal.Value;
            }
        }

        private void OnStartButtonClicked()
        {
            ChallengeRunScene.StartChallengeRun();
            Dismiss();
        }
    }
}