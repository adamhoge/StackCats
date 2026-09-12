namespace Tofuwu.StackCats
{
    public interface IOldMoveStrategy
    {
        float Difficulty { get; }
        bool PerformMoveAction(Puzzle puzzle);
        bool ShouldRemoveStrategy { get; }
    }
}