using System;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats
{
    public static class PuzzleInformationHelper
    {
        public static List<Stack> GetStacksByMovableCountRange(this Puzzle puzzle, int minCount = 1, int maxCount = 10)
        {
            List<Stack> potentialStacks = new List<Stack>(puzzle.Stacks);

            List<Stack> usableStacks = new List<Stack>();
            foreach (Stack stack in potentialStacks)
            {
                int numMovableBlocks = puzzle.GetNumMovableBlocksInStack(stack);
                if (numMovableBlocks >= minCount && numMovableBlocks <= maxCount)
                {
                    usableStacks.Add(stack);
                }
            }

            return usableStacks;
        }

        public static int GetNumMovableBlocksInStack(this Puzzle puzzle, Stack stack)
        {
            int numMovableBlocks = 0;
            Block currentBlock = stack.TopBlock;
            while (currentBlock)
            {
                if (!puzzle.IsMovable(stack, currentBlock)) return numMovableBlocks;

                if (currentBlock.GetComponent<SumBlock>()) return numMovableBlocks;

                Block blockBelow = currentBlock.GetBlockBelow();
                if (!blockBelow) return numMovableBlocks + 1;

                if (blockBelow.GetComponent<CatBlock>()) return numMovableBlocks;

                ++numMovableBlocks;
                currentBlock = blockBelow;
            }

            return numMovableBlocks;
        }

        public static List<Type> GetAllBlockTypes(this Puzzle puzzle)
        {
            List<Type> blockTypes = new List<Type>();

            foreach(Stack stack in puzzle.Stacks)
            {
                foreach(Block block in stack.Blocks)
                {
                    foreach(BlockComponent blockComponent in block.GetComponents<BlockComponent>())
                    {
                        Type blockType = blockComponent.GetType();
                        if (!blockTypes.Contains(blockType)){
                            blockTypes.Add(blockType);
                        }
                    }
                }
            }

            return blockTypes;
        }
        public static int GetBlockComponentCount<T>(this Puzzle puzzle) where T : BlockComponent
        {
            return puzzle.Stacks.Sum(s => s.Blocks.Count(b => b.GetComponent<T>()));
        }

        public static List<T> GetAllBlocksOfComponent<T>(this Puzzle puzzle) where T : BlockComponent
        {
            List<T> blockComponents = new List<T>();

            foreach (Stack stack in puzzle.Stacks)
            {
                foreach (Block block in stack.Blocks)
                {
                    T blockComponent = block.GetComponent<T>();
                    if (blockComponent) blockComponents.Add(blockComponent);
                }
            }

            return blockComponents;
        }

        public static bool HasBlockComponent<T>(this Stack stack) where T : BlockComponent
        {
            return stack.Blocks.FirstOrDefault(b => b.GetComponent<T>());
        }

        public static bool IsImmovableInTopNumBlocks(this Stack stack, int numBlocks)
        {
            if (stack.IsEmpty) return true;

            Block currentBlock = stack.TopBlock;
            if(currentBlock.GetComponents<BlockComponent>().Any(b => !b.IsMovable))
            {
                return true;
            }
            for (int i = 0; i < numBlocks - 1; i++)
            {
                currentBlock = currentBlock.GetBlockBelow();
                if (!currentBlock || currentBlock.GetComponents<BlockComponent>().Any(b => !b.IsMovable))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
