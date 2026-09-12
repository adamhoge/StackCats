using System;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalEndlessPuzzleCompletionData : IEndlessPuzzleCompletionData
    {
        public PuzzleCompletionType CompletionType { get { return _puzzleCompletionType; } }

        public string PuzzleAreaId { get { return _puzzleAreaId; } }

        public int DifficultyRating { get { return _difficultyRating; } }

        public int MinMoves { get { return _minMoves; } }

        public int MaxMoves { get { return _maxMoves; } }

        public int MovesMade { get { return _movesMade; } }

        public int UndosRemaining { get { return _undosRemaining; } }

        private readonly PuzzleCompletionType _puzzleCompletionType;
        private readonly string _puzzleAreaId;
        private readonly int _difficultyRating;
        private readonly int _minMoves;
        private readonly int _maxMoves;
        private readonly int _movesMade;
        private readonly int _undosRemaining;

        public LocalEndlessPuzzleCompletionData(PuzzleCompletionType completionType, int difficultyRating, int minMoves, int maxMoves, int movesMade, int undosRemaining)
        {
            _puzzleCompletionType = completionType;
            _difficultyRating = difficultyRating;
            _minMoves = minMoves;
            _maxMoves = maxMoves;
            _movesMade = movesMade;
            _undosRemaining = undosRemaining;
        }
    }
}
