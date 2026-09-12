using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats.Procedural
{
    public class AddRequiredSumBlockStrategy<TPuzzleArea, TPuzzle> : SumBlockStrategy, IMoveStrategy<TPuzzleArea, TPuzzle> where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
    {
        public float Difficulty { get { return DIFFICULTY; } }

        private static float DIFFICULTY = 1.5f;

        private readonly int _maxSumBlocks;

        public AddRequiredSumBlockStrategy(int maxSumBlocks)
        {
            _maxSumBlocks = maxSumBlocks;
        }

        public MoveStrategyResult PerformMoveStrategy(PuzzleGenerator<TPuzzleArea, TPuzzle> puzzleGenerator, GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo)
        {
            TPuzzle puzzle = generatedPuzzleInfo.Puzzle;

            List<Stack> prospectiveSourceStacks = puzzle.Stacks.Where(s => IsValidSourceStack(s)).ToList();
            while (prospectiveSourceStacks.Count > 0)
            {
                Stack tryStack = prospectiveSourceStacks[Random.Range(0, prospectiveSourceStacks.Count)];
                Block sourceBlock = tryStack.TopBlock;

                List<Stack> prospectiveDestinationStacks = puzzle.Stacks.Where(s => 
                    (!s.TopBlock || !s.TopBlock.GetComponent<SumBlock>()) &&
                    puzzle.GetNumBlocksPlaceableOnStack(s) > 1 &&
                    s != tryStack && 
                    puzzle.GetNumMovableBlocksInStack(s) < 3).ToList();
                if (prospectiveDestinationStacks.Count > 0)
                {
                    Stack destinationStack = prospectiveDestinationStacks[Random.Range(0, prospectiveDestinationStacks.Count)];
                    int sumValue = GetRandomSumValueForStack(tryStack);
                    if (!destinationStack.HasBlockComponent<CatBlock>() || Random.value > 0.75f)
                    {
                        puzzle.AddNewCatBlock(destinationStack);
                    }
                    puzzle.AddNewSumBlock(destinationStack, sumValue);
                    for (int i = tryStack.Blocks.Count - 1; i >= 0 && tryStack.Blocks[i].GetComponent<PuzzleBlock>(); i--)
                    {
                        tryStack.Blocks[i].GetComponent<PuzzleBlock>().PrimaryNumber -= sumValue;
                    }

                    return new MoveStrategyResult
                    {
                        succeeded = true,
                        shouldRemoveStrategy = puzzle.GetBlockComponentCount<SumBlock>() >= _maxSumBlocks,
                        estimatedDifficulty = 0.5f,
                        numMovesMade = 2
                    };
                }
                else
                {
                    prospectiveSourceStacks.Remove(tryStack);
                }
            }

            return new MoveStrategyResult { succeeded = false };
        }
    }
}
