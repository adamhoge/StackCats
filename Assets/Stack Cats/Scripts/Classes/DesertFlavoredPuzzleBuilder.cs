using System.Collections.Generic;
using System.Linq;
using Tofuwu.StackCats.Models;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public static class DesertFlavoredPuzzleBuilder
    {
        /// <summary>
        /// Build a puzzle from a puzzle model.
        /// </summary>
        /// <param name="puzzleModel">The puzzle model used to build the puzzle.</param>
        /// <param name="puzzlePrefab">The prefab used to generate the puzzle.</param>
        /// <returns></returns>
        public static DesertFlavoredPuzzle BuildFromModel(
            DesertFlavoredPuzzleModel puzzleModel,
            DesertFlavoredPuzzle puzzlePrefab,
            CatManager catManager
        )
        {
            DesertFlavoredPuzzle puzzle = Object.Instantiate(puzzlePrefab);
            puzzle.name = "Desert Flavored Puzzle";
            puzzle.MaxStackHeight = puzzleModel.MaxStackHeight;
            puzzle.NumMovesMade = puzzleModel.NumMovesMade;
            puzzle.IsSpecial = puzzleModel.IsSpecial;

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
                    else if (blockModel.HasRemovalBlock)
                    {
                        puzzle.AddNewRemovalBlock(stack);
                    }
                    else if (blockModel.HasPressureBlock)
                    {
                        puzzle.AddNewPressureBlock(stack, blockModel.PressureBlock.BreakingPoint);
                    }
                    else if (blockModel.HasRestrictedBlock)
                    {
                        puzzle.AddNewRestrictedBlock(stack);
                    }
                }
            }

            puzzle.StackHeightRequirements = puzzleModel.StackHeightRequirements;

            return puzzle;
        }

        /// <summary>
        /// Build a puzzle model from a puzle.
        /// </summary>
        /// <param name="puzzle">The puzzle from which the puzzle model will be built.</param>
        /// <returns>The puzzle model built from the provided puzzle.</returns>
        public static DesertFlavoredPuzzleModel BuildToModel(DesertFlavoredPuzzle puzzle)
        {
            DesertFlavoredPuzzleModel puzzleModel = new DesertFlavoredPuzzleModel();

            foreach (Stack stack in puzzle.Stacks)
            {
                StackModel stackModel = new StackModel();

                foreach (Block block in stack.Blocks)
                {
                    BlockModel blockModel = new BlockModel();

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
            puzzleModel.StackHeightRequirements = new List<int>(puzzle.StackHeightRequirements);

            return puzzleModel;
        }
    }
}
