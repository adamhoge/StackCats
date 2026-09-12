using System;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalEndlessRunData : IEndlessRunData
    {
        public bool IsComplete { get { return _isComplete; } }

        public IEndlessPuzzleData CurrentPuzzle { get { return _currentPuzzle; } }

        public List<IEndlessPuzzleCompletionData> CompletedPuzzles { get { return _completedPuzzles; } }

        private bool _isComplete;
        private IEndlessPuzzleData _currentPuzzle;
        private readonly List<IEndlessPuzzleCompletionData> _completedPuzzles = new List<IEndlessPuzzleCompletionData>();

        public void StartNewPuzzle(string puzzleJsonData, int difficultyRating, int minMoves, int maxMoves, int numUndos)
        {
            if (_currentPuzzle != null) return;

            IEndlessPuzzleData newPuzzleData = new LocalEndlessPuzzleData(puzzleJsonData, difficultyRating, minMoves, maxMoves, numUndos);
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

        public void CompleteCurrentPuzzle(PuzzleCompletionType completionType, int movesMade)
        {
            IEndlessPuzzleCompletionData completionData = new LocalEndlessPuzzleCompletionData(completionType, _currentPuzzle.DifficultyRating, _currentPuzzle.MinMoves, _currentPuzzle.MaxMoves, movesMade, _currentPuzzle.NumUndosRemaining);
            _completedPuzzles.Add(completionData);
            _currentPuzzle = null;
            if (completionType == PuzzleCompletionType.PuzzleFailed) _isComplete = true;
        }

        public int GetCompletedPuzzleScore(IEndlessPuzzleCompletionData completedPuzzle)
        {
            return 25 + (_completedPuzzles.IndexOf(completedPuzzle) + 1) * 2 - 1 + completedPuzzle.UndosRemaining;
        }

        public int GetScore()
        {
            int numCompletedPuzzles = _completedPuzzles.Count;
            int score = 25 * numCompletedPuzzles + numCompletedPuzzles * numCompletedPuzzles + _completedPuzzles.Sum(p => p.UndosRemaining);

            return score;
        }
    }
}
