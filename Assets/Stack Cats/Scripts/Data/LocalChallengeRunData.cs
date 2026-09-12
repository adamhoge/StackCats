using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalChallengeRunData : IChallengeRunData
    {
        public ChallengeRunDifficulty Difficulty { get { return _difficulty; } }

        public bool IsComplete { get { return _isComplete; } }

        public Dictionary<Currency, int> CurrencyReward { get { return _currencyReward; } }

        public IPresentData PresentReward { get { return _presentReward; } }

        public IChallengePuzzleData CurrentPuzzle { get { return _currentPuzzle; } }

        public List<IChallengePuzzleCompletionData> CompletedPuzzles { get { return _completedPuzzles; } }

        private readonly ChallengeRunDifficulty _difficulty;
        private bool _isComplete;
        private Dictionary<Currency, int> _currencyReward;
        private IPresentData _presentReward;
        private IChallengePuzzleData _currentPuzzle;
        private readonly List<IChallengePuzzleCompletionData> _completedPuzzles = new List<IChallengePuzzleCompletionData>();

        public LocalChallengeRunData(ChallengeRunDifficulty difficulty)
        {
            _difficulty = difficulty;
        }

        public void StartNewPuzzle(string puzzleJsonData, int difficultyRating, int minMoves, int maxMoves, int numUndos)
        {
            if (_currentPuzzle != null) return;

            IChallengePuzzleData newPuzzleData = new LocalChallengePuzzleData(puzzleJsonData, difficultyRating, minMoves, maxMoves, numUndos);
            _currentPuzzle = newPuzzleData;
        }

        public void UpdateCurrentPuzzle(string puzzleStateJsonData, int movesMade, List<string> catsSeen, List<string> newCatsSeen, int numUndosRemaining)
        {
            if (_currentPuzzle == null) return;

            _currentPuzzle.PuzzleCurrentStateJsonData = puzzleStateJsonData;
            _currentPuzzle.MovesMade = movesMade;
            _currentPuzzle.NumUndosRemaining = numUndosRemaining;
            _currentPuzzle.CatsSeen = catsSeen;
            _currentPuzzle.NewCatsSeen = newCatsSeen;
        }

        public bool UseLuckPotion(string puzzleJsonData)
        {
            if (_currentPuzzle == null) return false;

            _currentPuzzle.WasLuckPotionUsed = true;
            _currentPuzzle.PuzzleJsonData = puzzleJsonData;
            _currentPuzzle.PuzzleCurrentStateJsonData = puzzleJsonData;
            return true;
        }

        public void CompleteCurrentPuzzle(PuzzleCompletionType completionType, int movesMade)
        {
            IChallengePuzzleCompletionData completionData = new LocalChallengePuzzleCompletionData(completionType, _currentPuzzle.DifficultyRating, _currentPuzzle.MinMoves, _currentPuzzle.MaxMoves, movesMade);
            _completedPuzzles.Add(completionData);
            _currentPuzzle = null;
        }

        public void CompleteChallengeRun(Dictionary<Currency, int> currencyReward, IPresentData presentReward)
        {
            _isComplete = true;
            _currencyReward = currencyReward;
            _presentReward = presentReward;
        }
    }
}
