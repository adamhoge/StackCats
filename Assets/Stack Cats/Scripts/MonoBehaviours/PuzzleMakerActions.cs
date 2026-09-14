using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class PuzzleMakerActions : MonoBehaviour
    {
        public Puzzle Puzzle;
        public PuzzleArea PuzzleArea;

        // Resize Puzzle
        public void ResizePuzzle(int numStacks, int maxBlocks)
        {
            int curNumStacks = Puzzle.Stacks.Count;

            int stackDifference = numStacks - curNumStacks;
            if (stackDifference > 0)
            {
                for (int i = 0; i < stackDifference; i++)
                {
                    Puzzle.AddNewStack();
                }
            }
            else
            {
                for (int i = 0; i > stackDifference; i--)
                {
                    Stack stack = Puzzle.Stacks.Last();
                    Puzzle.RemoveStack(stack);
                    Destroy(stack.gameObject);
                }
            }

            Puzzle.MaxStackHeight = maxBlocks;
        }

        // Add Cat Block
        public void AddCatBlock(Stack stack)
        {
            Puzzle.AddNewCatBlock(stack);
        }

        // Add Puzzle Block
        public void AddPuzzleBlock(Stack stack)
        {
            PuzzleBuilder.AddRandomPuzzleBlockToTop(Puzzle, stack, 1, 9);
        }

        // Add Wild Block
        public void AddWildBlock(Stack stack)
        {
            Puzzle.AddNewWildBlock(stack);
        }

        // Add Sum Block
        public void AddSumBlock(Stack stack, int sumValue)
        {
            Puzzle.AddNewSumBlock(stack, sumValue);
        }

        // Add Terrain Block
        public void AddTerrainBlock(Stack stack)
        {
            Puzzle.AddNewTerrainBlock(stack);
        }

        // Add Removal Block
        public void AddRemovalBlock(Stack stack)
        {
            Puzzle.AddNewRemovalBlock(stack);
        }

        // Add Removal Block
        public void AddPressureBlock(Stack stack)
        {
            Puzzle.AddNewPressureBlock(stack);
        }

        public void AddRestrictedBlock(Stack stack)
        {
            Puzzle.AddNewRestrictedBlock(stack);
        }

        public void RemoveBlockAt(Stack stack, int blockIndex)
        {
            if (stack == null || blockIndex >= stack.Blocks.Count)
                return;

            Block block = stack.Blocks[blockIndex];
            stack.RemoveBlock(block, true);
        }

        // Remove blocks at and above the provided index.
        public void RemoveBlocksAt(Stack stack, int blockIndex)
        {
            if (stack == null || blockIndex >= stack.Blocks.Count)
                return;

            while (stack.Blocks.Count > blockIndex)
            {
                Block block = stack.TopBlock;
                stack.RemoveBlock(block);
                Destroy(block.gameObject);
            }
        }

        public void SumTopPuzzleBlocks(Stack stack, int amount)
        {
            int sumIndex = stack.Blocks.Count - 1;
            while (sumIndex >= 0)
            {
                PuzzleBlock puzzleBlock = stack.Blocks[sumIndex].GetComponent<PuzzleBlock>();
                if (!puzzleBlock)
                    break;

                puzzleBlock.PrimaryNumber += amount;
                --sumIndex;
            }
        }

        public void SumBlock(Stack stack, Block block, int amount)
        {
            PuzzleBlock puzzleBlock = block.GetComponent<PuzzleBlock>();
            if (puzzleBlock)
                puzzleBlock.PrimaryNumber += amount;

            SumBlock sumBlock = block.GetComponent<SumBlock>();
            if (sumBlock)
                sumBlock.SumValue += amount;

            PressureBlock pressureBlock = block.GetComponent<PressureBlock>();
            if (pressureBlock)
                pressureBlock.BreakingPoint += amount;
        }

        public void ShiftBlock(Stack stack, Block block, int amount)
        {
            int blockIndex = stack.Blocks.IndexOf(block);

            if (blockIndex + amount < 0 || blockIndex + amount > stack.Blocks.Count - 1)
                return;

            stack.RemoveBlock(block);
            stack.InsertBlock(block, blockIndex + amount);
        }
    }
}
