using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    public interface IEndlessPuzzleData
    {
        string PuzzleJsonData { get; set; }
        string PuzzleCurrentStateJsonData { get; set; }
        int DifficultyRating { get; }
        int MinMoves { get; }
        int MaxMoves { get; }
        int MovesMade { get; set; }
        int NumUndosRemaining { get; set; }
        List<string> CatsSeen { get; set; }
        List<string> NewCatsSeen { get; set; }
    }
}