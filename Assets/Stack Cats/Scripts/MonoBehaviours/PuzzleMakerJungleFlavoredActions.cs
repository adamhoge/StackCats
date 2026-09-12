using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats
{
    public class PuzzleMakerJungleFlavoredActions : PuzzleMakerActions
    {
        private JungleFlavoredPuzzleArea _jungleFlavoredPuzzleArea;
        private JungleFlavoredPuzzle _jungleFlavoredPuzzle;

        protected virtual void Start()
        {
            _jungleFlavoredPuzzleArea = (JungleFlavoredPuzzleArea)PuzzleArea;
            _jungleFlavoredPuzzle = (JungleFlavoredPuzzle)Puzzle;
        }

        // Add Jigsaw Blocks
        public void AddRandomJigsawObjectBlocksToTop(Stack stack)
        {
            int maxJigsawSize = stack.MaxBlocks - stack.Blocks.Count;
            List<JigsawPuzzleObject> usableJigsawPuzzleObjects = _jungleFlavoredPuzzleArea.JigsawPuzzleObjects.List.Where(jo => jo.JigsawSprites.Count <= maxJigsawSize).ToList();
            int jigsawsCount = usableJigsawPuzzleObjects.Count;

            if (usableJigsawPuzzleObjects.Count == 0) return;

            JigsawPuzzleObject jigsawPuzzleObject = usableJigsawPuzzleObjects[Random.Range(0, jigsawsCount)];

            for (int i = 0; i < jigsawPuzzleObject.JigsawSprites.Count; i++)
            {
                _jungleFlavoredPuzzle.AddNewJigsawBlock(stack, jigsawPuzzleObject, i);
            }
        }
    }
}
