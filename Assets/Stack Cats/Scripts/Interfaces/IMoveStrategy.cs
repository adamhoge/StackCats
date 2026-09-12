namespace Tofuwu.StackCats
{
    public struct MoveStrategyResult
    {
        public bool succeeded;
        public bool shouldRemoveStrategy;
        public int numMovesMade;
        public float estimatedDifficulty;
    }

    public interface IMoveStrategy<TPuzzleArea, TPuzzle> where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
    {
        MoveStrategyResult PerformMoveStrategy(PuzzleGenerator<TPuzzleArea, TPuzzle> puzzleGenerator, GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo);
    }
}
