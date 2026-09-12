namespace Tofuwu.StackCats.Data
{
    public interface IEndlessPuzzleCompletionData
    {
        PuzzleCompletionType CompletionType { get; }
        string PuzzleAreaId { get; }
        int DifficultyRating { get; }
        int MinMoves { get; }
        int MaxMoves { get; }
        int MovesMade { get; }
        int UndosRemaining { get; }
    }
}