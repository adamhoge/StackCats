using System.Collections.Generic;

namespace Tofuwu.StackCats.Models
{
    public class EndlessRunModel
    {
        public EndlessPuzzleModel CurrentPuzzle { get; set; }
        public List<EndlessPuzzleCompletionModel> CompletedPuzzles { get; set; }
        public int Score { get; set; }
        public bool IsComplete { get; set; }
    }
}
