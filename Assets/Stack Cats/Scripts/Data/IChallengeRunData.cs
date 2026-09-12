using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    public interface IChallengeRunData
    {
        ChallengeRunDifficulty Difficulty { get; }
        bool IsComplete { get; }
        Dictionary<Currency, int> CurrencyReward { get; }
        IPresentData PresentReward { get; }
        IChallengePuzzleData CurrentPuzzle { get; }
        List<IChallengePuzzleCompletionData> CompletedPuzzles { get; }
        void StartNewPuzzle(string puzzleJsonData, int difficultyRating, int minMoves, int maxMoves, int numUndos);
        void UpdateCurrentPuzzle(string puzzleStateJsonData, int movesMade, List<string> catsSeen, List<string> newCatsSeen, int numUndosRemaining);
        bool UseLuckPotion(string puzzleJsonData);
        void CompleteCurrentPuzzle(PuzzleCompletionType completionType, int movesMade);
        void CompleteChallengeRun(Dictionary<Currency, int> currencyReward, IPresentData presentReward);
    }
}