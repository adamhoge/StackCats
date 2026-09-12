using System;
using System.Collections.Generic;
using Tofuwu.StackCats.Models;

namespace Tofuwu.StackCats.Data
{
    public interface IDailyPuzzleData
    {
        DateTime ForDate { get; }
        List<PuzzleModel> GetPuzzles();
        void CompletePuzzle(int puzzleIndex);
        bool IsPuzzleComplete(int puzzleIndex);
    }
}