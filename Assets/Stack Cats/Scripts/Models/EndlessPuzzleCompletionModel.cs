using System;

namespace Tofuwu.StackCats.Models
{
    [Serializable]
    public class EndlessPuzzleCompletionModel
    {
        public PuzzleArea PuzzleArea { get; set; }

        public PuzzleCompletionType CompletionType { get; set; }

        public int MinMoves { get; set; }

        public int MaxMoves { get; set; }

        public int MovesMade { get; set; }

        public int UndosUsed { get; set; }
    }
}
