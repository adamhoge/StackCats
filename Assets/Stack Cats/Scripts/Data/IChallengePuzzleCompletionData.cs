namespace Tofuwu.StackCats.Data
{
    public interface IChallengePuzzleCompletionData
    {
        PuzzleCompletionType CompletionType { get; }
        int DifficultyRating { get; }
        int MinMoves { get; }
        int MaxMoves { get; }
        int MovesMade { get; }
    }
}