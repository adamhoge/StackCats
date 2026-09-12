using Tofuwu.StackCats.Data;
using Tofuwu.StackCats.Models;
using System.Linq;

namespace Tofuwu.StackCats
{
    public static class DataModelMapper
    {
        public static ChallengePuzzleModel MapToChallengePuzzleModel(IChallengePuzzleData challengePuzzleData)
        {
            if (challengePuzzleData == null)
            {
                return null;
            }
            else
            {
                return new ChallengePuzzleModel
                {
                    PuzzleJsonData = challengePuzzleData.PuzzleJsonData,
                    PuzzleCurrentStateJsonData = challengePuzzleData.PuzzleCurrentStateJsonData,
                    DifficultyRating = challengePuzzleData.DifficultyRating,
                    MinMoves = challengePuzzleData.MinMoves,
                    MaxMoves = challengePuzzleData.MaxMoves,
                    MovesMade = challengePuzzleData.MovesMade,
                    UndosRemaining = challengePuzzleData.NumUndosRemaining,
                    WasLuckPotionUsed = challengePuzzleData.WasLuckPotionUsed
                };
            }
        }

        public static ChallengePuzzleCompletionModel MapToChallengePuzzleCompletionModel(IChallengePuzzleCompletionData challengePuzzleCompletionData)
        {
            if (challengePuzzleCompletionData == null)
            {
                return null;
            }
            else
            {
                return new ChallengePuzzleCompletionModel
                {
                    CompletionType = challengePuzzleCompletionData.CompletionType,
                    MinMoves = challengePuzzleCompletionData.MinMoves,
                    MaxMoves = challengePuzzleCompletionData.MaxMoves,
                    MovesMade = challengePuzzleCompletionData.MovesMade
                };
            }
        }

        public static EndlessPuzzleModel MapToEndlessPuzzleModel(IEndlessPuzzleData endlessPuzzleData, PuzzleManager puzzleManager)
        {
            if (endlessPuzzleData == null)
            {
                return null;
            }
            else
            {
                return new EndlessPuzzleModel
                {
                    PuzzleJsonData = endlessPuzzleData.PuzzleJsonData,
                    PuzzleCurrentStateJsonData = endlessPuzzleData.PuzzleCurrentStateJsonData,
                    DifficultyRating = endlessPuzzleData.DifficultyRating,
                    MinMoves = endlessPuzzleData.MinMoves,
                    MaxMoves = endlessPuzzleData.MaxMoves,
                    MovesMade = endlessPuzzleData.MovesMade,
                    UndosRemaining = endlessPuzzleData.NumUndosRemaining
                };
            }
        }

        public static EndlessPuzzleCompletionModel MapToEndlessPuzzleCompletionModel(IEndlessPuzzleCompletionData endlessPuzzleCompletionData, PuzzleManager puzzleManager)
        {
            if (endlessPuzzleCompletionData == null)
            {
                return null;
            }
            else
            {
                return new EndlessPuzzleCompletionModel
                {
                    PuzzleArea = puzzleManager.PuzzleAreaCollection.GetById(endlessPuzzleCompletionData.PuzzleAreaId),
                    CompletionType = endlessPuzzleCompletionData.CompletionType,
                    MinMoves = endlessPuzzleCompletionData.MinMoves,
                    MaxMoves = endlessPuzzleCompletionData.MaxMoves,
                    MovesMade = endlessPuzzleCompletionData.MovesMade
                };
            }
        }
    }
}
