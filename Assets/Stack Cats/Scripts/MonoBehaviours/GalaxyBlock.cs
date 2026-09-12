using UnityEngine;
using System.Collections.Generic;

namespace Tofuwu.StackCats
{
    public delegate void GalaxyBlockTriggered(GalaxyBlock sender, Stack stack, int fromIndex, int toIndex);

    public class GalaxyBlock : PuzzleBlock
    {
        public event GalaxyBlockTriggered onGalaxyBlockTriggered;

        public AudioEvent RemoveBlocksSoundEffect;
        public SpriteRenderer BlockGraphic;
        public SpriteMask BackgroundMask;

        public override void OnMove()
        {
            Stack parentStack = _block.ParentStack;
            Block thisBlock = GetComponent<Block>();
            int blockIndex = parentStack.Blocks.IndexOf(thisBlock);
            for (int i = blockIndex - 1; i >= 0; i--)
            {
                Block targetBlock = _block.ParentStack.Blocks[i];
                PuzzleBlock puzzleBlock = targetBlock.GetComponent<PuzzleBlock>();
                if (!puzzleBlock) return;

                GalaxyBlock galaxyBlock = targetBlock.GetComponent<GalaxyBlock>();
                if (galaxyBlock)
                {
                    parentStack.RemoveBlocks(targetBlock, thisBlock, true);
                    if (onGalaxyBlockTriggered != null) onGalaxyBlockTriggered(this, parentStack, i, blockIndex);
                    break;
                }
            }
        }
    }
}