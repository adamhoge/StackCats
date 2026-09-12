using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Tofuwu.StackCats.Models;

namespace Tofuwu.StackCats.UI
{
    public class ChallengeEndedOverlayScreen : OverlayScreen
    {
        public ChallengeRunScene ChallengeRunScene;
        public Button CloseButton;
        public RectTransform EarnedRewardsRectTransform;
        public CurrencyAmountUI CurrencyAmountPrefab;

        private PuzzleManager _puzzleManager;

        public override void OnActive()
        {
            base.OnActive();

            _puzzleManager.OneOffChallengePuzzleCompleted.ClearAll();
        }

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void OnEnable()
        {
            DisplayRewards();
            CloseButton.onClick.AddListener(OnCloseButtonClicked);
        }

        protected void OnDisable()
        {
            CloseButton.onClick.RemoveListener(OnCloseButtonClicked);
        }

        private void DisplayRewards()
        {
            foreach (RectTransform rectTransform in EarnedRewardsRectTransform)
            {
                Destroy(rectTransform.gameObject);
            }

            ChallengeRunModel completedChallengeRun = ChallengeRunScene.CurrentChallengeRun;
            List<ChallengePuzzleCompletionModel> completedPuzzles = ChallengeRunScene.CurrentChallengeRun.CompletedPuzzles;
            List<ChallengePuzzleReward> rewards = ChallengeRunScene.PuzzleArea.ChallengeRunRewards.Where(r => r.AtDifficulty == completedChallengeRun.Difficulty).ToList();
            int numCompletedPuzzles = completedPuzzles.Count;
            int goldPaws = 0;
            int farmGems = 0;
            int jungleGems = 0;
            int cityGems = 0;
            int desertGems = 0;
            int galaxyGems = 0;

            for (int i = 0; i < numCompletedPuzzles; i++)
            {
                ChallengePuzzleReward rewardAtIndex = rewards.FirstOrDefault(r => r.AtPuzzleIndex == i);
                CurrencyAmountDictionary currencyRewards = rewardAtIndex != null ? rewardAtIndex.CurrencyReward : _puzzleManager.DefaultCurrencyReward;
                if (currencyRewards.ContainsKey(Currency.GoldPaw)) goldPaws += currencyRewards[Currency.GoldPaw];
                if (currencyRewards.ContainsKey(Currency.FarmGem)) farmGems += currencyRewards[Currency.FarmGem];
                if (currencyRewards.ContainsKey(Currency.JungleGem)) jungleGems += currencyRewards[Currency.JungleGem];
                if (currencyRewards.ContainsKey(Currency.CityGem)) cityGems += currencyRewards[Currency.CityGem];
                if (currencyRewards.ContainsKey(Currency.DesertGem)) desertGems += currencyRewards[Currency.DesertGem];
                if (currencyRewards.ContainsKey(Currency.GalaxyGem)) galaxyGems += currencyRewards[Currency.GalaxyGem];
            }

            if (goldPaws > 0)
            {
                CurrencyAmountUI rewardAmount = Instantiate(CurrencyAmountPrefab, EarnedRewardsRectTransform);
                rewardAmount.CurrencyType = Currency.GoldPaw;
                rewardAmount.Amount = goldPaws;
            }
            if (farmGems > 0)
            {
                CurrencyAmountUI rewardAmount = Instantiate(CurrencyAmountPrefab, EarnedRewardsRectTransform);
                rewardAmount.CurrencyType = Currency.FarmGem;
                rewardAmount.Amount = farmGems;
            }
            if (jungleGems > 0)
            {
                CurrencyAmountUI rewardAmount = Instantiate(CurrencyAmountPrefab, EarnedRewardsRectTransform);
                rewardAmount.CurrencyType = Currency.JungleGem;
                rewardAmount.Amount = jungleGems;
            }
            if (cityGems > 0)
            {
                CurrencyAmountUI rewardAmount = Instantiate(CurrencyAmountPrefab, EarnedRewardsRectTransform);
                rewardAmount.CurrencyType = Currency.CityGem;
                rewardAmount.Amount = cityGems;
            }
            if (desertGems > 0)
            {
                CurrencyAmountUI rewardAmount = Instantiate(CurrencyAmountPrefab, EarnedRewardsRectTransform);
                rewardAmount.CurrencyType = Currency.DesertGem;
                rewardAmount.Amount = desertGems;
            }
            if (galaxyGems > 0)
            {
                CurrencyAmountUI rewardAmount = Instantiate(CurrencyAmountPrefab, EarnedRewardsRectTransform);
                rewardAmount.CurrencyType = Currency.GalaxyGem;
                rewardAmount.Amount = galaxyGems;
            }
        }

        private void OnCloseButtonClicked()
        {
            Dismiss();
        }
    }
}