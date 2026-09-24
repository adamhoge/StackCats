using System;
using System.Collections.Generic;
using RockhopperGames.StackCats.Models;

namespace RockhopperGames.StackCats.Data
{
    public interface IDailyPuzzleData
    {
        DateTime ForDate { get; }
        List<PuzzleModel> GetPuzzles();
        void CompletePuzzle(int puzzleIndex);
        bool IsPuzzleComplete(int puzzleIndex);
    }
}
