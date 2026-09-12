using Tofuwu.StackCats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats.Procedural
{
    public class MoveOrCoverJigsawPieceStrategy<TPuzzleArea, TPuzzle> : IMoveStrategy<TPuzzleArea, TPuzzle> where TPuzzleArea : PuzzleArea where TPuzzle : Puzzle
    {
        // Strategy:
        // 1. Get a random single jigsaw piece
        // 2. Get a random removable jigsaw piece
        // 3. If have both, randomize between em
        // 4. If XOR, do whichever one is available
        // 5. If NOR, fail move, maybe remove as an option?
        public MoveStrategyResult PerformMoveStrategy(PuzzleGenerator<TPuzzleArea, TPuzzle> puzzleGenerator, GeneratedPuzzleInfo<TPuzzleArea, TPuzzle> generatedPuzzleInfo)
        {
            TPuzzle puzzle = generatedPuzzleInfo.Puzzle;

            var blockToMoveStack = GetJigsawBlockToMoveIndex(puzzle);

            if (blockToMoveStack == null) return new MoveStrategyResult
            {
                succeeded = false
            };

            var destinationStack = GetDestinationStack(puzzle, blockToMoveStack);

            if (destinationStack == null) return new MoveStrategyResult
            {
                succeeded = false
            };
            
            var sourceStack = blockToMoveStack;
            var jigsawBlockToMove = sourceStack.TopBlock;
            sourceStack.RemoveBlock(jigsawBlockToMove);

            destinationStack.AddBlock(jigsawBlockToMove);

            return new MoveStrategyResult
            {
                succeeded = true,
                estimatedDifficulty = 0.5f,
                numMovesMade = 1
            };
        }

        private Stack GetDestinationStack(TPuzzle puzzle, Stack blockToMoveIndex)
        {
            var stacks = new List<Stack>(puzzle.Stacks);
            stacks.Remove(blockToMoveIndex);
            var invalidStacks = stacks.Where(s => s.IsAtMaxCapacity || HasTwoJigsawBlocksOnTop(s));
            stacks = stacks.Except(invalidStacks).ToList();

            if (stacks.Count == 0) return null;

            int minStackCount = stacks.Min(s => s.Blocks.Count);
            var lowStacks = stacks.Where(s => s.Blocks.Count == minStackCount);

            return stacks[UnityEngine.Random.Range(0, stacks.Count)];
        }

        private Stack GetJigsawBlockToMoveIndex(TPuzzle puzzle)
        {
            var stacks = puzzle.Stacks;
            var prospectiveStacks = stacks.Where(s => 
                (s.TopBlock && s.TopBlock.GetComponent<PuzzleBlock>() 
                    && s.TopBlock.GetBlockBelow() 
                    && !s.TopBlock.GetBlockBelow().GetComponent<CatBlock>()) || 
                HasTwoJigsawBlocksOnTop(s)).ToList();

            if (prospectiveStacks.Count == 0) return null;

            return prospectiveStacks[UnityEngine.Random.Range(0, prospectiveStacks.Count)];
        }

        private bool HasTwoJigsawBlocksOnTop(Stack stack)
        {
            var topBlock = stack.TopBlock;

            if (!topBlock) return false;

            if (!topBlock.GetComponent<JigsawBlock>()) return false;

            var blockBelow = topBlock.GetBlockBelow();

            if (!blockBelow) return false;

            return blockBelow.GetComponent<JigsawBlock>();
        }
    }
}
