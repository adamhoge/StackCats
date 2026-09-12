using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [RequireComponent(typeof(Block))]
    public class JigsawBlock : BlockComponent
    {
        public SpriteRenderer JigsawSpriteRenderer;
        public JigsawPuzzleObject JigsawPuzzleObject;
        public int JigsawIndex;

        /// <summary>
        /// Flag indicating whether or not the jigsaw block is complete.
        /// </summary>
        // TODO: Should the need arise, flag for completion could be determined as blocks are added or removed from the stack.
        public bool IsComplete { get { return !JigsawPuzzleObject || GetConnectedJigsawBlocks().Count == JigsawPuzzleObject.JigsawSprites.Count; } }

        /// <summary>
        /// Get a list of connected JigsawBlocks (including this one).
        /// </summary>
        /// <returns>A list of connected JigsawBlocks (including this one).</returns>
        public List<JigsawBlock> ConnectedJigsawBlocks { get { return GetConnectedJigsawBlocks(); } }
        
        protected void Start()
        {
            if (JigsawPuzzleObject && JigsawSpriteRenderer)
            {
                JigsawSpriteRenderer.sprite = JigsawPuzzleObject.JigsawSprites[JigsawIndex];
                Block.BlockOverlayMask.sprite = JigsawPuzzleObject.JigsawSprites[JigsawIndex];
            }
        }

        private List<JigsawBlock> GetConnectedJigsawBlocks()
        {
            List<JigsawBlock> connectedJigsawBlocks = new List<JigsawBlock> { this };

            if (JigsawPuzzleObject)
            {
                var currentJigsawIndex = JigsawIndex + 1;
                for (Block currentBlock = Block.GetBlockAbove(); currentBlock && currentJigsawIndex < JigsawPuzzleObject.JigsawSprites.Count; currentBlock = currentBlock.GetBlockAbove())
                {
                    JigsawBlock currentJigsawBlock = currentBlock.GetComponent<JigsawBlock>();
                    if (!currentJigsawBlock || currentJigsawBlock.JigsawPuzzleObject != JigsawPuzzleObject || currentJigsawBlock.JigsawIndex != currentJigsawIndex) break;
                    connectedJigsawBlocks.Insert(connectedJigsawBlocks.Count - 1, currentJigsawBlock);
                    ++currentJigsawIndex;
                }

                currentJigsawIndex = JigsawIndex - 1;
                for (Block currentBlock = Block.GetBlockBelow(); currentBlock && currentJigsawIndex >= 0; currentBlock = currentBlock.GetBlockBelow())
                {
                    JigsawBlock currentJigsawBlock = currentBlock.GetComponent<JigsawBlock>();
                    if (!currentJigsawBlock || currentJigsawBlock.JigsawPuzzleObject != JigsawPuzzleObject || currentJigsawBlock.JigsawIndex != currentJigsawIndex) break;
                    connectedJigsawBlocks.Insert(0, currentJigsawBlock);
                    --currentJigsawIndex;
                }
            }

            return connectedJigsawBlocks;
        }
    }
}