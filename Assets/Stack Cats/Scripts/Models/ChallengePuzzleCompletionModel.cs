using System;

namespace Tofuwu.StackCats.Models
{
    [Serializable]
    public class ChallengePuzzleCompletionModel
    {
        public PuzzleCompletionType CompletionType { get; set; }

        public int MinMoves { get; set; }

        public int MaxMoves { get; set; }

        public int MovesMade { get; set; }
    }
}
