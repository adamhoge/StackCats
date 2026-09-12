using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    public interface IPuzzleData
    {
        IDailyPuzzleData DailyPuzzles { get; set; }
        string CurrentAreaId { get; set; }
        PuzzleMode CurrentMode { get; set; }
        string CurrentStoryPuzzleId { get; set; }
        ChallengeRunDifficulty DefaultChallengeRunDifficulty { get; set; }
        float CurrentLuck { get; set; }
        int SpecialPuzzlesRemaining { get; set; }
        bool IsAreaLocked(string areaId);
        void UnlockArea(string areaId);
        bool IsProgressionPuzzleLocked(string puzzleId);
        bool IsProgressionPuzzleCompleted(string puzzleId);
        int? GetStoryPuzzleBestMoveScore(string puzzleId);
        void CompleteProgressionPuzzle(string puzzleId, DateTime completionDateTime, float duration, int numMovesMade);
        void UnlockProgressionPuzzle(string puzzleId);
        IChallengeRunData StartChallengeRun(string areaId, ChallengeRunDifficulty difficulty);
        IChallengePuzzleData StartChallengePuzzle(string areaId, string puzzleJsonData, int difficultyRating, int minMoves, int maxMoves, int numUndos);
        void UpdateChallengePuzzle(string areaId, string puzzleStateJsonData, int movesMade, List<string> catsSeen, List<string> newCatsSeen, int numUndosRemaining);
        bool UseLuckPotion(string areaId, string puzzleJsonData);
        void CompleteChallengePuzzle(string areaId, PuzzleCompletionType completionType, int movesMade);
        void CompleteChallengeRun(string areaId, Dictionary<Currency, int> currencyReward, IPresentData presentData);
        void EndChallengeRun(string areaId);
        IChallengeRunData GetCurrentChallengeRun(string areaId);
        IChallengeRunData GetLastCompletedChallengeRun(string areaId);
        List<IChallengeRunData> GetCompletedChallengeRuns(string areaId);
        bool IsChallengeRunDifficultyCompleted(string areaId, ChallengeRunDifficulty difficulty);
        IEndlessRunData StartEndlessRun(string areaId);
        IEndlessPuzzleData StartEndlessPuzzle(string areaId, string puzzleJsonData, int difficultyRating, int minMoves, int maxMoves, int numUndoes);
        void UpdateEndlessPuzzle(string areaId, string puzzleStateJsonData, int movesMade, List<string> catsSeen, List<string> newCatsSeen, int numUndosRemaining);
        void CompleteEndlessPuzzle(string areaId, PuzzleCompletionType completionType, int movesMade);
        void EndEndlessRun(string areaId);
        IEndlessRunData GetCurrentEndlessRun(string areaId);
        List<IEndlessRunData> GetCompletedEndlessRuns(string areaId);
    }
}