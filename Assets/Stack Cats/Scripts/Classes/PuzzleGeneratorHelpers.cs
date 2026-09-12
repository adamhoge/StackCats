using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats.Procedural
{
    /// <summary>
    /// Provides helper methods for generating puzzles.
    /// </summary>
    public static class PuzzleGeneratorHelpers
    {
        public static Stack SelectStackWithLeastishMovableBlocks(Puzzle puzzle, List<Stack> stacks)
        {
            int lowestMovableBlocks = stacks.Min(s => puzzle.GetNumMovableBlocksInStack(s));
            return stacks.Where(s => puzzle.GetNumMovableBlocksInStack(s) <= lowestMovableBlocks + 1).ToList().SelectRandom();
        }
    }
}
