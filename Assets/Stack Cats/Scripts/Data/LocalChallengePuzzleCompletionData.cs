using System;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalChallengePuzzleCompletionData : IChallengePuzzleCompletionData
    {
        public PuzzleCompletionType CompletionType { get { return _puzzleCompletionType; } }

        public int DifficultyRating { get { return _difficultyRating; } }

        public int MinMoves { get { return _minMoves; } }

        public int MaxMoves { get { return _maxMoves; } }

        public int MovesMade { get { return _movesMade; } }

        private readonly PuzzleCompletionType _puzzleCompletionType;
        private readonly int _difficultyRating;
        private readonly int _minMoves;
        private readonly int _maxMoves;
        private readonly int _movesMade;

        public LocalChallengePuzzleCompletionData(PuzzleCompletionType completionType, int difficultyRating, int minMoves, int maxMoves, int movesMade)
        {
            _puzzleCompletionType = completionType;
            _difficultyRating = difficultyRating;
            _minMoves = minMoves;
            _maxMoves = maxMoves;
            _movesMade = movesMade;
        }
    }
}
