using System.Collections.Generic;
using System.Linq;
using Tofuwu.StackCats.Models;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public static class GalaxyFlavoredPuzzleBuilder
    {
        /// <summary>
        /// Build a puzzle from a puzzle model.
        /// </summary>
        /// <param name="puzzleModel">The puzzle model used to build the puzzle.</param>
        /// <param name="puzzlePrefab">The prefab used to generate the puzzle.</param>
        /// <returns></returns>
        public static GalaxyFlavoredPuzzle BuildFromModel(
            GalaxyFlavoredPuzzleModel puzzleModel,
            GalaxyFlavoredPuzzle puzzlePrefab,
            CatManager catManager
        )
        {
            GalaxyFlavoredPuzzle puzzle = Object.Instantiate(puzzlePrefab);
            puzzle.name = "Galaxy Flavored Puzzle";
            puzzle.MaxStackHeight = puzzleModel.MaxStackHeight;
            puzzle.NumMovesMade = puzzleModel.NumMovesMade;
            puzzle.IsSpecial = puzzleModel.IsSpecial;
            puzzle.RaiseStacksInterval = puzzleModel.RaiseStacksInterval;
            puzzle.RaiseStacksMovesRemaining =
                puzzle.RaiseStacksInterval - puzzle.NumMovesMade % puzzle.RaiseStacksInterval;

            foreach (StackModel stackModel in puzzleModel.Stacks)
            {
                Stack stack = puzzle.AddNewStack();
                stack.MaxBlocks = puzzleModel.MaxStackHeight;
                foreach (BlockModel blockModel in stackModel.Blocks)
                {
                    if (blockModel.HasWildBlock)
                    {
                        puzzle.AddNewWildBlock(stack);
                    }
                    else if (blockModel.HasPuzzleBlock)
                    {
                        puzzle.AddNewPuzzleBlock(
                            stack,
                            blockModel.PuzzleBlock.PrimaryNumber,
                            blockModel.PuzzleBlock.SecondaryNumber
                        );
                    }
                    else if (blockModel.HasCatBlock)
                    {
                        puzzle.AddNewCatBlock(
                            stack,
                            catManager.CatCollection.GetById(blockModel.CatBlock.CatId),
                            blockModel.CatBlock.Yarn,
                            blockModel.CatBlock.CatBlockId
                        );
                    }
                    else if (blockModel.HasSumBlock)
                    {
                        puzzle.AddNewSumBlock(stack, blockModel.SumBlock.SumValue);
                    }
                    else if (blockModel.HasTerrainBlock)
                    {
                        puzzle.AddNewTerrainBlock(stack);
                    }
                    else if (blockModel.HasGalaxyBlock)
                    {
                        GalaxyBlock galaxyBlock = puzzle.CreateGalaxyBlock(
                            blockModel.GalaxyBlock.PrimaryNumber,
                            blockModel.GalaxyBlock.SecondaryNumber
                        );
                        puzzle.AddBlock(stack, galaxyBlock.Block);
                    }
                    else if (blockModel.HasRemovalBlock)
                    {
                        puzzle.AddNewRemovalBlock(stack);
                    }
                    else if (blockModel.HasPressureBlock)
                    {
                        puzzle.AddNewPressureBlock(stack, blockModel.PressureBlock.BreakingPoint);
                    }
                }
            }

            return puzzle;
        }

        /// <summary>
        /// Build a puzzle model from a puzle.
        /// </summary>
        /// <param name="puzzle">The puzzle from which the puzzle model will be built.</param>
        /// <returns>The puzzle model built from the provided puzzle.</returns>
        public static GalaxyFlavoredPuzzleModel BuildToModel(GalaxyFlavoredPuzzle puzzle)
        {
            GalaxyFlavoredPuzzleModel puzzleModel = new GalaxyFlavoredPuzzleModel();

            foreach (Stack stack in puzzle.Stacks)
            {
                StackModel stackModel = new StackModel();

                foreach (Block block in stack.Blocks)
                {
                    BlockModel blockModel = new BlockModel();

                    GalaxyBlock galaxyBlock = block.GetComponent<GalaxyBlock>();
                    if (galaxyBlock)
                    {
                        blockModel.HasGalaxyBlock = true;
                        blockModel.GalaxyBlock = new GalaxyBlockComponentModel
                        {
                            PrimaryNumber = galaxyBlock.PrimaryNumber,
                            SecondaryNumber = galaxyBlock.SecondaryNumber,
                        };
                    }
                    else
                    {
                        WildBlock wildBlock = block.GetComponent<WildBlock>();
                        if (wildBlock)
                        {
                            blockModel.HasWildBlock = true;
                            blockModel.WildBlock = new WildBlockComponentModel();
                        }
                        else
                        {
                            PuzzleBlock puzzleBlock = block.GetComponent<PuzzleBlock>();
                            if (puzzleBlock)
                            {
                                blockModel.HasPuzzleBlock = true;
                                blockModel.PuzzleBlock = new PuzzleBlockComponentModel
                                {
                                    PrimaryNumber = puzzleBlock.PrimaryNumber,
                                    SecondaryNumber = puzzleBlock.SecondaryNumber,
                                };
                            }
                        }
                    }

                    CatBlock catBlock = block.GetComponent<CatBlock>();
                    if (catBlock)
                    {
                        string catId = catBlock.Cat ? catBlock.Cat.GetId() : "";
                        blockModel.HasCatBlock = true;
                        blockModel.CatBlock = new CatBlockComponentModel
                        {
                            CatId = catId,
                            Yarn = catBlock.Yarn,
                            CatBlockId = catBlock.CatBlockId,
                        };
                    }

                    SumBlock sumBlock = block.GetComponent<SumBlock>();
                    if (sumBlock)
                    {
                        blockModel.HasSumBlock = true;
                        blockModel.SumBlock = new SumBlockComponentModel
                        {
                            SumValue = sumBlock.SumValue,
                        };
                    }

                    LockBlock terrainBlock = block.GetComponent<LockBlock>();
                    if (terrainBlock)
                    {
                        blockModel.HasTerrainBlock = true;
                        blockModel.TerrainBlock = new TerrainBlockComponentModel();
                    }

                    KeyBlock removalBlock = block.GetComponent<KeyBlock>();
                    if (removalBlock)
                    {
                        blockModel.HasRemovalBlock = true;
                        blockModel.RemovalBlock = new RemovalBlockComponentModel();
                    }

                    PressureBlock pressureBlock = block.GetComponent<PressureBlock>();
                    if (pressureBlock)
                    {
                        blockModel.HasPressureBlock = true;
                        blockModel.PressureBlock = new PressureBlockComponentModel
                        {
                            BreakingPoint = pressureBlock.BreakingPoint,
                        };
                    }

                    stackModel.Blocks.Add(blockModel);
                }

                puzzleModel.Stacks.Add(stackModel);
            }

            puzzleModel.MaxStackHeight = puzzle.MaxStackHeight;
            puzzleModel.NumMovesMade = puzzle.NumMovesMade;
            puzzleModel.IsSpecial = puzzle.IsSpecial;
            puzzleModel.RaiseStacksInterval = puzzle.RaiseStacksInterval;

            return puzzleModel;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="puzzleArea"></param>
        /// <param name="numStacks"></param>
        /// <param name="maxBlocks"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public static GalaxyFlavoredPuzzle GenerateNew(
            PuzzleArea puzzleArea,
            int numStacks,
            int maxBlocks = 10,
            int difficulty = 1
        )
        {
            difficulty = 12;

            // Create the empty puzzle.
            GalaxyFlavoredPuzzle puzzle = Object.Instantiate(
                (GalaxyFlavoredPuzzle)puzzleArea.PuzzlePrefab
            );
            puzzle.name = "Galaxy Puzzle";
            puzzle.MaxStackHeight = maxBlocks;
            for (int i = 0; i < numStacks; i++)
                puzzle.AddNewStack();

            // Calculate build info based on difficulty.
            int baseNumBlocks = numStacks * 2;
            int rangedNumBlocks = numStacks;
            int difficultyNumBlocks = numStacks * maxBlocks - 1 - baseNumBlocks - rangedNumBlocks;
            int numBlocks =
                baseNumBlocks
                + Random.Range(0, rangedNumBlocks)
                + (int)((float)difficulty / 20 * difficultyNumBlocks);
            int numSumBlocks = difficulty >= 10 ? Random.Range(0, difficulty / 4) : 0;
            int numCatBlocks = numStacks + 2 + Random.Range(0, difficulty / 2);
            int numPuzzleBlocks = numBlocks - numSumBlocks - numCatBlocks;
            int maxSumBlockValueChange = Mathf.FloorToInt(
                (float)numSumBlocks / 2 * (numSumBlocks <= 2 ? 2 : 1)
            );
            int minPuzzleBlockValue = 1 + maxSumBlockValueChange;
            int maxPuzzleBlockValue = 5 - maxSumBlockValueChange;

            minPuzzleBlockValue = 1;
            maxPuzzleBlockValue = 5;

            // Begin edit mode.
            puzzle.IsEditMode = true;

            // Add puzzle blocks. Ensure that one has a maximum of three blocks.
            Stack lowStack = puzzle.Stacks[Random.Range(0, puzzle.Stacks.Count)];
            int lowStackSize = Random.Range(0, 3);
            int stackSize = (numPuzzleBlocks - lowStackSize) / (numStacks - 1);
            foreach (Stack stack in puzzle.Stacks)
            {
                for (int i = 0; i < (stack == lowStack ? lowStackSize : stackSize); i++)
                {
                    if (Random.value > 0.75f)
                    {
                        PuzzleBuilder.AddRandomPuzzleBlockToTop(
                            puzzle,
                            stack,
                            minPuzzleBlockValue,
                            maxPuzzleBlockValue
                        );
                    }
                    else if (Random.value > 0.4f)
                    {
                        AddRandomGalaxyBlockToTop(
                            puzzle,
                            stack,
                            minPuzzleBlockValue,
                            maxPuzzleBlockValue
                        );
                    }
                }
            }

            // Get sum block indices.
            int numSumAndCatBlocks = numCatBlocks + numSumBlocks;
            List<int> sumBlockIndices = new List<int>();
            for (int i = 0; i < numSumBlocks; i++)
            {
                int newSumBlockIndex = Random.Range(0, numSumAndCatBlocks);
                while (sumBlockIndices.Contains(newSumBlockIndex))
                    newSumBlockIndex = Random.Range(0, numSumAndCatBlocks);
                sumBlockIndices.Add(newSumBlockIndex);
            }

            // Shuffle in sum and cat blocks.
            int blocksAdded = 0;
            int padding = 2;
            while (blocksAdded < numSumAndCatBlocks && padding >= 0)
            {
                Stack destinationStack = null;
                List<Stack> stacksByMovableCountRange = PuzzleBuilder.GetStacksByMovableCountRange(
                    puzzle,
                    0,
                    padding
                );
                if (stacksByMovableCountRange.Count > 0)
                {
                    int minStackSize = stacksByMovableCountRange.Min(s => s.Blocks.Count);
                    destinationStack = PuzzleBuilder.GetRandomStackByCountRange(
                        stacksByMovableCountRange,
                        minStackSize,
                        minStackSize + padding
                    );
                }
                if (!destinationStack)
                {
                    int minStackSize = puzzle.Stacks.Min(s => s.Blocks.Count);
                    int maxStackSize = minStackSize + padding;
                    if (maxStackSize >= maxBlocks - 3)
                        maxStackSize = maxBlocks - 3;
                    destinationStack = PuzzleBuilder.GetRandomStackByCountRange(
                        puzzle.Stacks,
                        minStackSize,
                        maxStackSize
                    );
                    if (!destinationStack)
                        break;
                }

                int spaceAvailable = destinationStack.MaxBlocks - destinationStack.Blocks.Count - 1;
                int maxMovableBlocks = puzzle.Stacks.Max(s =>
                    PuzzleBuilder.GetNumMovableBlocks(puzzle, s)
                );
                if (maxMovableBlocks > spaceAvailable)
                    maxMovableBlocks = spaceAvailable;
                if (maxMovableBlocks < 1)
                    break;

                int usablePadding = padding >= maxMovableBlocks ? maxMovableBlocks - 1 : padding;
                List<Stack> potentialSourceStacks = PuzzleBuilder.GetStacksByMovableCountRange(
                    puzzle,
                    1,
                    maxBlocks,
                    new List<Stack> { destinationStack }
                );
                Stack sourceStack =
                    potentialSourceStacks.Count > 0
                        ? potentialSourceStacks[Random.Range(0, potentialSourceStacks.Count)]
                        : null;
                if (!sourceStack)
                    break;

                int numMovableSourceBlocks = PuzzleBuilder.GetNumMovableBlocks(puzzle, sourceStack);
                if (numMovableSourceBlocks > maxMovableBlocks)
                    numMovableSourceBlocks = maxMovableBlocks;
                int minIndex = sourceStack.Blocks.Count - numMovableSourceBlocks;
                int maxIndex = minIndex + usablePadding;
                if (maxIndex > sourceStack.Blocks.Count)
                    maxIndex = sourceStack.Blocks.Count;
                Block sourceBlock = sourceStack.Blocks[Random.Range(minIndex, maxIndex)];

                bool blockAdded;
                if (sumBlockIndices.Contains(blocksAdded))
                {
                    int sumBlockValue =
                        (numSumBlocks <= 2 ? 2 : 1) * (Random.value >= .5f ? 1 : -1);

                    blockAdded = PuzzleBuilder.AddMoveSumBlock(
                        puzzle,
                        sumBlockValue,
                        sourceStack,
                        sourceBlock,
                        destinationStack
                    );
                }
                else
                {
                    blockAdded = PuzzleBuilder.AddMoveCatBlock(
                        puzzle,
                        sourceStack,
                        sourceBlock,
                        destinationStack
                    );
                }

                if (blockAdded)
                {
                    ++blocksAdded;
                }
                else if (padding > 0)
                {
                    --padding;
                }
                else
                    break;
            }

            PuzzleBuilder.MoveRandomBlocks(puzzle);

            // End edit mode and return the puzzle.
            puzzle.IsEditMode = false;
            return puzzle;
        }

        /// <summary>
        /// Add a random puzzle block to the top of the provided stack.
        /// </summary>
        /// <param name="puzzle">The puzzle.</param>
        /// <param name="stack">The puzzle stack.</param>
        /// <param name="min">The minimum puzzle block value.</param>
        /// <param name="min">The maximum puzzle block value.</param>
        public static bool AddRandomGalaxyBlockToTop(
            GalaxyFlavoredPuzzle puzzle,
            Stack stack,
            int min = 1,
            int max = 9
        )
        {
            PuzzleBlock topPuzzleBlock = stack.TopBlock
                ? stack.TopBlock.GetComponent<PuzzleBlock>()
                : null;
            GalaxyBlock galaxyBlock = CreateAdjacentGalaxyBlock(puzzle, topPuzzleBlock);

            return puzzle.AddBlock(stack, galaxyBlock.Block);
        }

        /// <summary>
        /// Create a galaxy block that has values adjacent to the provided block.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <param name="puzzleBlock"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static GalaxyBlock CreateAdjacentGalaxyBlock(
            GalaxyFlavoredPuzzle puzzle,
            PuzzleBlock puzzleBlock,
            int min = 1,
            int max = 9
        )
        {
            int primaryNumber;
            int secondaryNumber;
            if (puzzleBlock)
            {
                bool incrementPrimary = false;
                incrementPrimary =
                    puzzleBlock.PrimaryNumber == min
                    || Random.Range(0, 2) == 0 && puzzleBlock.PrimaryNumber < max;
                primaryNumber = puzzleBlock.PrimaryNumber + (incrementPrimary ? 1 : -1);
                secondaryNumber = (puzzleBlock.SecondaryNumber + 1) % 2;
            }
            else
            {
                primaryNumber = Random.Range(min, max + 1);
                secondaryNumber = Random.Range(0, 2);
            }

            return puzzle.CreateGalaxyBlock(primaryNumber, secondaryNumber);
        }
    }
}
