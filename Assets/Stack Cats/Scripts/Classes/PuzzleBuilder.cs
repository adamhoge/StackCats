using Tofuwu.StackCats.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public static class PuzzleBuilder
    {
        /// <summary>
        /// Add a random puzzle block to the top of the provided stack.
        /// </summary>
        /// <param name="puzzle">The puzzle.</param>
        /// <param name="stack">The puzzle stack.</param>
        /// <param name="min">The minimum puzzle block value.</param>
        /// <param name="min">The maximum puzzle block value.</param>
        public static bool AddRandomPuzzleBlockToTop(Puzzle puzzle, Stack stack, int min = 1, int max = 9)
        {
            PuzzleBlock topPuzzleBlock = stack.TopBlock ? stack.TopBlock.GetComponent<PuzzleBlock>() : null;
            PuzzleBlock puzzleBlock = CreateAdjacentPuzzleBlock(puzzle, topPuzzleBlock, min, max);

            if (!puzzleBlock) return false;

            return puzzle.AddBlock(stack, puzzleBlock.Block);
        }

        public static PuzzleBlock CreateAdjacentPuzzleBlock(Puzzle puzzle, PuzzleBlock puzzleBlock, int min = 1, int max = 9)
        {
            int primaryNumber;
            int secondaryNumber;
            if (puzzleBlock)
            {
                primaryNumber = puzzleBlock.PrimaryNumber - 1;
                secondaryNumber = (puzzleBlock.SecondaryNumber + 1) % 2;
            }
            else
            {
                primaryNumber = Random.Range(min, max);
                secondaryNumber = Random.Range(0, 2);
            }

            if (primaryNumber < 1 || primaryNumber > 9) return null;

            return puzzle.CreatePuzzleBlock(primaryNumber, secondaryNumber);
        }

        /// <summary>
        /// Get a stack of a size that falls into the provided range.
        /// </summary>
        /// <param name="stacks"></param>
        /// <param name="minCount"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static Stack GetRandomStackByCountRange(IEnumerable<Stack> stacks, int minCount = 0, int maxCount = 10)
        {
            List<Stack> stacksInRange = stacks.Where(s => s.Blocks.Count >= minCount && s.Blocks.Count <= maxCount).ToList();

            return stacksInRange.Count > 0 ? stacksInRange[Random.Range(0, stacksInRange.Count)] : null;
        }

        /// <summary>
        /// Get all stacks with a number of movable blocks that fall into the provided range (except excluded stacks).
        /// </summary>
        /// <param name="puzzle"></param>
        /// <param name="minCount"></param>
        /// <param name="maxCount"></param>
        /// <param name="excludedStacks"></param>
        /// <returns></returns>
        public static List<Stack> GetStacksByMovableCountRange(Puzzle puzzle, int minCount = 1, int maxCount = 10, List<Stack> excludedStacks = null)
        {
            if (excludedStacks == null) excludedStacks = new List<Stack>();
            List<Stack> stacksInRange = new List<Stack>();
            foreach (Stack stack in puzzle.Stacks)
            {
                if (!excludedStacks.Contains(stack))
                {
                    int numMovableBlocks = GetNumMovableBlocks(puzzle, stack);
                    if (numMovableBlocks >= minCount && numMovableBlocks <= maxCount) stacksInRange.Add(stack);
                }
            }

            return stacksInRange;
        }

        /// <summary>
        /// Add a cat block to a stack and then move blocks on top of it.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <param name="sourceStack"></param>
        /// <param name="sourceBlock"></param>
        /// <param name="destinationStack"></param>
        /// <returns></returns>
        public static bool AddMoveCatBlock(Puzzle puzzle, Stack sourceStack, Block sourceBlock, Stack destinationStack)
        {
            puzzle.AddNewCatBlock(destinationStack);

            if (puzzle.MoveBlock(sourceStack, sourceBlock, destinationStack, true))
            {
                return true;
            }

            Object.Destroy(destinationStack.TopBlock.gameObject);
            destinationStack.RemoveBlock(destinationStack.TopBlock);
            return false;
        }

        /// <summary>
        /// Move a sum block to a stack and then move blocks on top of it.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <param name="sumBlockValue"></param>
        /// <param name="sourceStack"></param>
        /// <param name="sourceBlock"></param>
        /// <param name="destinationStack"></param>
        /// <returns></returns>
        public static bool AddMoveSumBlock(Puzzle puzzle, int sumBlockValue, Stack sourceStack, Block sourceBlock, Stack destinationStack)
        {
            puzzle.AddNewSumBlock(destinationStack, sumBlockValue);

            List<Stack> tryStacks = puzzle.Stacks.Where(s => s != destinationStack).ToList();
            Stack sumStack = tryStacks[Random.Range(0, tryStacks.Count)];
            PuzzleBlock puzzleBlock = sumStack.TopBlock ? sumStack.TopBlock.GetComponent<PuzzleBlock>() : null;
            while (puzzleBlock)
            {
                puzzleBlock.PrimaryNumber -= sumBlockValue;
                Block blockBelow = puzzleBlock.Block.GetBlockBelow();
                puzzleBlock = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            }

            if (puzzle.MoveBlock(sourceStack, sourceBlock, destinationStack, true))
            {
                return true;
            }

            puzzleBlock = sumStack.TopBlock.GetComponent<PuzzleBlock>();
            while (puzzleBlock)
            {
                puzzleBlock.PrimaryNumber += sumBlockValue;
                Block blockBelow = puzzleBlock.Block.GetBlockBelow();
                puzzleBlock = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            }
            Object.Destroy(destinationStack.TopBlock.gameObject);
            destinationStack.RemoveBlock(destinationStack.TopBlock);
            return false;
        }

        /// <summary>
        /// Get the number of movable blocks in a stack.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <param name="stack"></param>
        /// <returns></returns>
        public static int GetNumMovableBlocks(Puzzle puzzle, Stack stack)
        {
            bool blockBelowIsCatBlock = false;
            int i;
            for (i = 0; i < stack.Blocks.Count; i++)
            {
                Block block = stack.Blocks[i];
                if (!blockBelowIsCatBlock && puzzle.IsMovable(stack, stack.Blocks[i])) break;

                blockBelowIsCatBlock = block.GetComponent<CatBlock>();
            }

            return stack.Blocks.Count - i;
        }

        /// <summary>
        /// Moves a random set of blocks from one stack to another (if possible).
        /// </summary>
        /// <returns>A flag indicating whether or not the blocks were successfully moved.</returns>
        public static bool MoveRandomBlocks(Puzzle puzzle)
        {
            //Stack stack = GetRandomStackByCountRange(puzzle.Stacks, 1);
            //int numMovableBlocks = GetNumMovableBlocks(puzzle, stack);

            return false;
        }

        public static string GetPuzzleJsonData(Puzzle puzzle)
        {
            string jsonData = "";

            if (puzzle.GetType() == typeof(GalaxyFlavoredPuzzle))
                jsonData = JsonUtility.ToJson(GalaxyFlavoredPuzzleBuilder.BuildToModel((GalaxyFlavoredPuzzle)puzzle));
            else if (puzzle.GetType() == typeof(NightFlavoredPuzzle))
                jsonData = JsonUtility.ToJson(NightFlavoredPuzzleBuilder.BuildToModel((NightFlavoredPuzzle)puzzle));
            else if (puzzle.GetType() == typeof(JungleFlavoredPuzzle))
                jsonData = JsonUtility.ToJson(JungleFlavoredPuzzleBuilder.BuildToModel((JungleFlavoredPuzzle)puzzle));
            else if (puzzle.GetType() == typeof(DesertFlavoredPuzzle))
                jsonData = JsonUtility.ToJson(DesertFlavoredPuzzleBuilder.BuildToModel((DesertFlavoredPuzzle)puzzle));
            else if (puzzle.GetType() == typeof(FarmFlavoredPuzzle))
                jsonData = JsonUtility.ToJson(FarmFlavoredPuzzleBuilder.BuildToModel((FarmFlavoredPuzzle)puzzle));

            return jsonData;
        }

        public static Puzzle BuildFromModel(string puzzleJsonData, PuzzleArea puzzleArea, CatManager catManager)
        {
            Puzzle puzzlePrefab = puzzleArea.PuzzlePrefab;

            Puzzle puzzle = null;
            try
            {
                if (puzzlePrefab.GetType() == typeof(GalaxyFlavoredPuzzle))
                {
                    puzzle = GalaxyFlavoredPuzzleBuilder.BuildFromModel(JsonUtility.FromJson<GalaxyFlavoredPuzzleModel>(puzzleJsonData), (GalaxyFlavoredPuzzle)puzzlePrefab, catManager);
                }
                else if (puzzlePrefab.GetType() == typeof(NightFlavoredPuzzle))
                {
                    puzzle = NightFlavoredPuzzleBuilder.BuildFromModel(JsonUtility.FromJson<NightFlavoredPuzzleModel>(puzzleJsonData), (NightFlavoredPuzzle)puzzlePrefab, catManager);
                }
                else if (puzzlePrefab.GetType() == typeof(DesertFlavoredPuzzle))
                {
                    puzzle = DesertFlavoredPuzzleBuilder.BuildFromModel(JsonUtility.FromJson<DesertFlavoredPuzzleModel>(puzzleJsonData), (DesertFlavoredPuzzle)puzzlePrefab, catManager);
                }
                else if (puzzlePrefab.GetType() == typeof(JungleFlavoredPuzzle))
                {
                    puzzle = JungleFlavoredPuzzleBuilder.BuildFromModel(JsonUtility.FromJson<JungleFlavoredPuzzleModel>(puzzleJsonData), (JungleFlavoredPuzzleArea)puzzleArea, catManager);
                }
                else if (puzzlePrefab.GetType() == typeof(FarmFlavoredPuzzle))
                {
                    puzzle = FarmFlavoredPuzzleBuilder.BuildFromModel(JsonUtility.FromJson<PuzzleModel>(puzzleJsonData), (FarmFlavoredPuzzle)puzzlePrefab, catManager);
                }
            }
            catch
            {
                GameManager.Instance.ConfirmAction("Unable to load puzzle.");
            }

            return puzzle;
        }
    }
}