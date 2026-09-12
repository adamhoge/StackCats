using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    public interface IEndlessRunData
    {
        bool IsComplete { get; }
        IEndlessPuzzleData CurrentPuzzle { get; }
        List<IEndlessPuzzleCompletionData> CompletedPuzzles { get; }
        void StartNewPuzzle(string puzzleJsonData, int difficultyRating, int minMoves, int maxMoves, int numUndos);
        void UpdateCurrentPuzzle(string puzzleStateJsonData, int movesMade, List<string> catsSeen, List<string> newCatsSeen, int numUndosRemaining);
        void CompleteCurrentPuzzle(PuzzleCompletionType completionType, int movesMade);
        int GetCompletedPuzzleScore(IEndlessPuzzleCompletionData completedPuzzle);
        int GetScore();
    }
}