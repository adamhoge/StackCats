using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tofuwu.StackCats.Models
{
    public class ChallengeRunModel
    {
        public ChallengeRunDifficulty Difficulty { get; set; }

        public ChallengePuzzleModel CurrentPuzzle { get; set; }

        public List<ChallengePuzzleCompletionModel> CompletedPuzzles { get; set; }
    }
}
