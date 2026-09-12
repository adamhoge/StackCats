using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats.Procedural
{
    public static class OldPuzzleGeneratorHelpers
    {
        public static int GetLowestMovableIndex(this Puzzle puzzle)
        {
            int lowestMovableIndex = -1;
            foreach (Stack stack in puzzle.Stacks)
            {
                Block lowestBlock = stack.Blocks.FirstOrDefault(block => puzzle.IsMovable(stack, block) && !block.HasBlockBelow<CatBlock>());
                int lowestBlockIndex = stack.Blocks.IndexOf(lowestBlock);
                if (lowestBlock && (lowestMovableIndex == -1 || lowestBlockIndex < lowestMovableIndex))
                {
                    lowestMovableIndex = lowestBlockIndex;
                }
            }

            return lowestMovableIndex;
        }

        public static int GetLowestMovableIndexOfBlockType<T>(this Puzzle puzzle) where T : BlockComponent
        {
            int lowestMovableIndex = -1;
            foreach (Stack stack in puzzle.Stacks)
            {
                Block lowestBlock = stack.Blocks.FirstOrDefault(block => 
                  puzzle.IsMovable(stack, block) && 
                  block.GetComponent<T>() &&
                  !block.HasBlockBelow<CatBlock>());

                int lowestBlockIndex = stack.Blocks.IndexOf(lowestBlock);
                if (lowestBlock && (lowestMovableIndex == -1 || lowestBlockIndex < lowestMovableIndex))
                {
                    lowestMovableIndex = lowestBlockIndex;
                }
            }

            return lowestMovableIndex;
        }
    }
}
