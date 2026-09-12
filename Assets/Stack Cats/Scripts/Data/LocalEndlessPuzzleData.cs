using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalEndlessPuzzleData : IEndlessPuzzleData
    {
        public string PuzzleJsonData { get { return _puzzleJsonData; } set { _puzzleJsonData = value; } }

        public string PuzzleCurrentStateJsonData { get { return _puzzleCurrentStateJsonData; } set { _puzzleCurrentStateJsonData = value; } }

        public int DifficultyRating { get { return _difficultyRating; } }

        public int MovesMade { get { return _movesMade; } set { _movesMade = value; } }

        public int MinMoves { get { return _minMoves; } }

        public int MaxMoves { get { return _maxMoves; } }

        public int NumUndosRemaining { get { return _numUndosRemaining; } set { _numUndosRemaining = value; } }

        public List<string> CatsSeen { get { return _catsSeen; } set { _catsSeen = value; } }

        public List<string> NewCatsSeen { get { return _newCatsSeen; } set { _newCatsSeen = value; } }

        private string _puzzleJsonData;
        private string _puzzleCurrentStateJsonData;
        private readonly int _difficultyRating;
        private int _movesMade;
        private int _numUndosRemaining;
        private readonly int _minMoves;
        private readonly int _maxMoves;
        private List<string> _catsSeen;
        private List<string> _newCatsSeen;

        public LocalEndlessPuzzleData(string puzzleJsonData, int difficultyRating, int minMoves, int maxMoves, int numUndos)
        {
            _puzzleJsonData = puzzleJsonData;
            _puzzleCurrentStateJsonData = puzzleJsonData;
            _difficultyRating = difficultyRating;
            _minMoves = minMoves;
            _maxMoves = maxMoves;
            _numUndosRemaining = numUndos;
            _catsSeen = new List<string>();
            _newCatsSeen = new List<string>();
        }
    }
}
