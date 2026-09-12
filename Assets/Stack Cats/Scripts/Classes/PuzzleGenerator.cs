using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats
{
    public delegate void PerformedMoveAction<TPuzzleArea, TPuzzle>(GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo) where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle;
    public delegate void PuzzleGenerated<TPuzzleArea, TPuzzle>(GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo) where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle;

    public class GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
    {
        public TPuzzle Puzzle { get; set; }
        public TPuzzleArea PuzzleArea { get; set; }
        public float EstimatedDifficulty { get; set; }
        public int ObjectiveDifficulty { get; set; }
        public int NumStacks { get; set; }
        public int MaxBlocks { get; set; }
        public int MoveBlockAllowance { get; set; }
        public int NumMovesMade { get; set; }
    }

    public abstract class PuzzleGenerator<TPuzzleArea, TPuzzle> where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
    {
        public event PerformedMoveAction<TPuzzleArea, TPuzzle> onPerformedMoveAction;
        public event PuzzleGenerated<TPuzzleArea, TPuzzle> onPuzzleGenerated;

        protected TPuzzleArea _puzzleArea;

        protected const float MAX_DIFFICULTY = 100.0f;
        protected const int MAX_MOVE_BLOCK_ALLOWANCE = 2;

        public PuzzleGenerator(TPuzzleArea puzzleArea)
        {
            _puzzleArea = puzzleArea;
        }

        /// <summary>
        /// Generate a new puzzle.
        /// </summary>
        /// <returns></returns>
        public virtual GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> Generate(int difficulty)
        {
            // If the puzzle area isn't specified, return nothing.
            if (!_puzzleArea)
            {
                Debug.LogError("PuzzleBuilder: Must specify a Puzzle Area to build a puzzle.");
                return null;
            }

            GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo = new GeneratedPuzzleInfo<TPuzzleArea, TPuzzle>();
            generatedPuzzleInfo.PuzzleArea = _puzzleArea;
            generatedPuzzleInfo.ObjectiveDifficulty = difficulty;
            generatedPuzzleInfo.MoveBlockAllowance = Mathf.RoundToInt(MAX_MOVE_BLOCK_ALLOWANCE * (1.0f - (difficulty / MAX_DIFFICULTY)));

            // Instantiate the puzzle.
            TPuzzle puzzle = Object.Instantiate((TPuzzle)_puzzleArea.PuzzlePrefab);
            puzzle.name = _puzzleArea.PuzzlePrefab.name;
            puzzle.IsEditMode = true;
            generatedPuzzleInfo.Puzzle = puzzle;

            // Create the finished puzzle.
            CreateFinishedPuzzle(generatedPuzzleInfo);

            // Get move strategies to unsolve the puzzle.
            Dictionary<IMoveStrategy<TPuzzleArea, TPuzzle>, float> moveStrategies = GetMoveStrategies(difficulty);

            // Perform move actions.
            int failedMoves = 0;
            while (generatedPuzzleInfo.EstimatedDifficulty <= difficulty && failedMoves < 25)
            {
                IMoveStrategy<TPuzzleArea, TPuzzle> moveStrategy = GetMoveStrategy(moveStrategies, generatedPuzzleInfo);
                MoveStrategyResult moveResult = PerformMoveStrategy(moveStrategy, generatedPuzzleInfo);
                if (moveResult.succeeded)
                {
                    generatedPuzzleInfo.NumMovesMade += moveResult.numMovesMade;
                    generatedPuzzleInfo.EstimatedDifficulty += moveResult.estimatedDifficulty;
                    onPerformedMoveAction?.Invoke(generatedPuzzleInfo);
                }
                else
                {
                    ++failedMoves;
                }

                if (moveResult.shouldRemoveStrategy)
                {
                    moveStrategies.Remove(moveStrategy);
                    if (moveStrategies.Count == 0)
                    {
                        break;
                    }
                }
            }

            return generatedPuzzleInfo;
        }

        public IEnumerator GeneratePuzzleAtInterval(int difficulty, float interval)
        {
            // If the puzzle area isn't specified, return nothing.
            if (!_puzzleArea)
            {
                Debug.LogError("PuzzleBuilder: Must specify a Puzzle Area to build a puzzle.");
                yield break;
            }

            GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo = new GeneratedPuzzleInfo<TPuzzleArea, TPuzzle>();
            generatedPuzzleInfo.PuzzleArea = _puzzleArea;
            generatedPuzzleInfo.ObjectiveDifficulty = difficulty;

            // Instantiate the puzzle.
            TPuzzle puzzle = Object.Instantiate((TPuzzle)_puzzleArea.PuzzlePrefab);
            puzzle.name = _puzzleArea.PuzzlePrefab.name;
            puzzle.IsEditMode = true;
            generatedPuzzleInfo.Puzzle = puzzle;

            // Create the finished puzzle.
            CreateFinishedPuzzle(generatedPuzzleInfo);

            // Get move strategies to unsolve the puzzle.
            Dictionary<IMoveStrategy<TPuzzleArea, TPuzzle>, float> moveStrategies = GetMoveStrategies(difficulty);

            // Perform move actions.
            int failedMoves = 0;
            while (generatedPuzzleInfo.EstimatedDifficulty <= difficulty && failedMoves < 25)
            {
                IMoveStrategy<TPuzzleArea, TPuzzle> moveStrategy = GetMoveStrategy(moveStrategies, generatedPuzzleInfo);
                MoveStrategyResult moveResult = PerformMoveStrategy(moveStrategy, generatedPuzzleInfo);
                if (moveResult.succeeded)
                {
                    generatedPuzzleInfo.NumMovesMade += moveResult.numMovesMade;
                    generatedPuzzleInfo.EstimatedDifficulty += moveResult.estimatedDifficulty;
                    onPerformedMoveAction?.Invoke(generatedPuzzleInfo);
                }
                else
                {
                    ++failedMoves;
                }

                if (moveResult.shouldRemoveStrategy)
                {
                    moveStrategies.Remove(moveStrategy);
                    if(moveStrategies.Count == 0)
                    {
                        break;
                    }
                }

                yield return new WaitForSeconds(interval);
            }

            if (onPuzzleGenerated != null) onPuzzleGenerated(generatedPuzzleInfo);

            yield return null;
        }

        public abstract Stack GetPreferredSourceStack(TPuzzle puzzle, List<Stack> usableStacks);

        public abstract Stack GetPreferredDestinationStack(TPuzzle puzzle, List<Stack> usableStacks);

        protected abstract void CreateFinishedPuzzle(GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo);

        protected abstract Dictionary<IMoveStrategy<TPuzzleArea, TPuzzle>, float> GetMoveStrategies(int difficulty);

        protected virtual IMoveStrategy<TPuzzleArea, TPuzzle> GetMoveStrategy(
            Dictionary<IMoveStrategy<TPuzzleArea, TPuzzle>, float> moveStrategies,
            GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo)
        {
            float randomMoveActionValue = Random.value;
            float tryTotal = 0.0f;

            IMoveStrategy<TPuzzleArea, TPuzzle> moveStrategy = null;
            foreach (KeyValuePair<IMoveStrategy<TPuzzleArea, TPuzzle>, float> tryMoveStrategy in moveStrategies)
            {
                tryTotal += tryMoveStrategy.Value;
                if (tryTotal >= randomMoveActionValue)
                {
                    moveStrategy = tryMoveStrategy.Key;
                    break;
                }
            }

            if (moveStrategy == null)
            {
                float maxMoveStrategyValue = moveStrategies.Max(ms => ms.Value);
                moveStrategy = moveStrategies.FirstOrDefault(ms => ms.Value == maxMoveStrategyValue).Key;
            }

            return moveStrategy;
        }

        protected virtual MoveStrategyResult PerformMoveStrategy(
            IMoveStrategy<TPuzzleArea, TPuzzle> moveStrategy,
            GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo)
        {
            if (moveStrategy == null)
            {
                return new MoveStrategyResult { succeeded = false };
            }

            return moveStrategy.PerformMoveStrategy(this, generatedPuzzleInfo);
        }
    }
}
