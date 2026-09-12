using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats.Models
{
    [Serializable]
    public class ChallengePuzzleModel
    {
        public string PuzzleJsonData { get; set; }

        public string PuzzleCurrentStateJsonData { get; set; }

        public int DifficultyRating { get; set; }

        public int MinMoves { get; set; }

        public int MaxMoves { get; set; }

        public bool WasStarted { get; set; }

        public bool WasLuckPotionUsed { get; set; }

        public int MovesMade { get; set; }

        public int UndosRemaining { get; set; }

        public List<Cat> CatsSeen { get; set; }

        public List<Cat> NewCatsSeen { get; set; }
    }
}