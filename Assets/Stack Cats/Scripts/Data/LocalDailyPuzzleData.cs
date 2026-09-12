using System;
using System.Collections.Generic;
using Tofuwu.StackCats.Models;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalDailyPuzzleData : IDailyPuzzleData
    {
        public DateTime ForDate { get { throw new NotImplementedException(); } }

        private DailyPuzzlesModel _model;
        private List<int> _completedPuzzleIndices = new List<int>();

        public LocalDailyPuzzleData(DailyPuzzlesModel model)
        {
            _model = model;
        }

        public List<PuzzleModel> GetPuzzles()
        {
            return _model.Puzzles;
        }

        public void CompletePuzzle(int puzzleIndex)
        {
            _completedPuzzleIndices.Add(puzzleIndex);
        }

        public bool IsPuzzleComplete(int puzzleIndex)
        {
            return _completedPuzzleIndices.Contains(puzzleIndex);
        }
    }
}