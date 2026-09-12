using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class PuzzleMakerJungleFlavoredActionsController : PuzzleMakerActionsController
    {
        private PuzzleMakerJungleFlavoredActions _puzzleMakerJungleFlavoredActions;

        protected override void Start()
        {
            base.Start();

            _puzzleMakerJungleFlavoredActions =
                (PuzzleMakerJungleFlavoredActions)PuzzleMakerActions;
        }

        protected override void AddBlocks(int stackIndex, int blockIndex, bool invertAction)
        {
            if (stackIndex < 0 || stackIndex >= Puzzle.Stacks.Count)
                return;
            Stack stack = PuzzleMakerActions.Puzzle.Stacks[stackIndex];

            if (!invertAction)
            {
                switch (_currentBlockType)
                {
                    case BlockType.CatBlock:
                        _puzzleMakerJungleFlavoredActions.AddCatBlock(stack);
                        break;
                    case BlockType.PuzzleBlock:
                        _puzzleMakerJungleFlavoredActions.AddPuzzleBlock(stack);
                        break;
                    case BlockType.WildBlock:
                        _puzzleMakerJungleFlavoredActions.AddWildBlock(stack);
                        break;
                    case BlockType.SumBlock:
                        _puzzleMakerJungleFlavoredActions.AddSumBlock(stack, 1);
                        break;
                    case BlockType.TerrainBlock:
                        _puzzleMakerJungleFlavoredActions.AddTerrainBlock(stack);
                        break;
                    case BlockType.RemovalBlock:
                        _puzzleMakerJungleFlavoredActions.AddRemovalBlock(stack);
                        break;
                    case BlockType.PressureBlock:
                        _puzzleMakerJungleFlavoredActions.AddPressureBlock(stack);
                        break;
                }
            }
            else
            {
                PuzzleMakerActions.RemoveBlocksAt(stack, blockIndex);
            }
        }
    }
}
