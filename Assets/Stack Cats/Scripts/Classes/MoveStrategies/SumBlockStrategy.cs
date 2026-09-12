using UnityEngine;

namespace Tofuwu.StackCats.Procedural
{
    public abstract class SumBlockStrategy
    {
        protected bool IsValidSourceStack(Stack stack)
        {
            if (stack.IsEmpty || stack.IsAtMaxCapacity) return false;

            WildBlock wildBlock = stack.TopBlock.GetComponent<WildBlock>();
            if (wildBlock) return false;

            PuzzleBlock topPuzzleBlock = stack.TopBlock.GetComponent<PuzzleBlock>();
            if (!topPuzzleBlock) return false;

            if (stack.Blocks.Count < 9 || topPuzzleBlock.PrimaryNumber != 9) return true;

            return GetLowestMovablePuzzleBlock(topPuzzleBlock).PrimaryNumber != 1;
        }

        protected PuzzleBlock GetLowestMovablePuzzleBlock(PuzzleBlock topPuzzleBlock)
        {
            if (!topPuzzleBlock) return null;

            PuzzleBlock puzzleBlock = topPuzzleBlock;
            Block blockBelow = puzzleBlock.Block.GetBlockBelow();
            PuzzleBlock puzzleBlockBelow = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            while (puzzleBlockBelow)
            {
                puzzleBlock = puzzleBlockBelow;

                blockBelow = puzzleBlock.Block.GetBlockBelow();
                puzzleBlockBelow = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            }

            return puzzleBlock;
        }

        protected int GetRandomSumValueForStack(Stack stack)
        {
            PuzzleBlock topPuzzleBlock = stack.TopBlock.GetComponent<PuzzleBlock>();
            int minSummableValue = 1 - topPuzzleBlock.PrimaryNumber; // 0, -1, -2, -3, -4, -5, -6, -7, -8
            if (minSummableValue == 0) minSummableValue = 1;
            int maxSummableValue = (9 - GetLowestMovablePuzzleBlock(topPuzzleBlock).PrimaryNumber); // 0, 1, 2, 3, 4, 5, 6, 7, 8
            if (maxSummableValue == 0) maxSummableValue = -1;

            int randomSummableValue = Random.Range(minSummableValue, maxSummableValue + 1);

            if (randomSummableValue != 0) return -randomSummableValue;

            if (minSummableValue == -1) return 1;

            if (maxSummableValue == 1) return -1;

            return Random.value >= 0.5f ? -1 : 1;
        }

    }
}
