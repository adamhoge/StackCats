using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats
{
    public delegate void JigsawBlockMoved(JigsawBlock jigsawBlock);

    public class JungleFlavoredPuzzle : FarmFlavoredPuzzle
    {
        public event JigsawBlockMoved onJigsawBlockMoved;

        public JigsawBlock JigsawBlockPrefab;

        public int NumJigsawPuzzles { get { return GetNumJigsawPuzzles(); } }

        public int NumCompletedJigsawPuzzles { get { return GetNumCompletedJigsawPuzzles(); } }

        public bool AddNewJigsawBlock(Stack stack, JigsawPuzzleObject jigsawPuzzleObject, int jigsawIndex)
        {
            // If the stack isn't a part of the puzzle or the cat block prefab isn't defined, don't add anything.
            if (!_stacks.Contains(stack) || !JigsawBlockPrefab) return false;

            // Create a new jigsaw block.
            JigsawBlock newJigsawBlock = Instantiate(JigsawBlockPrefab);
            newJigsawBlock.name = "Jigsaw Block";
            newJigsawBlock.JigsawPuzzleObject = jigsawPuzzleObject;
            newJigsawBlock.JigsawIndex = jigsawIndex;

            // Add it to the stack, or destroy it if unable to add it.
            bool wasAdded = stack.AddBlock(newJigsawBlock.GetComponent<Block>());
            if (!wasAdded) DestroyImmediate(newJigsawBlock.gameObject);
            return wasAdded;
        }

        protected override void OnBlockMoved(Stack source, Block block, Stack destination)
        {
            base.OnBlockMoved(source, block, destination);

            JigsawBlock jigsawBlock = block.GetComponent<JigsawBlock>();
            if (jigsawBlock && onJigsawBlockMoved != null) onJigsawBlockMoved(jigsawBlock);
        }

        protected override bool IsComplete()
        {
            List<JigsawBlock> jigsawBlocks = Stacks.SelectMany(stack => stack.Blocks, (stack, block) => block.GetComponent<JigsawBlock>()).Where(jigsawBlock => jigsawBlock).ToList();

            while (jigsawBlocks.Count > 0)
            {
                JigsawBlock jigsawBlock = jigsawBlocks[0];
                if (!jigsawBlock.IsComplete)
                {
                    jigsawBlocks.Clear();
                    return false;
                }

                jigsawBlocks = jigsawBlocks.Except(jigsawBlock.ConnectedJigsawBlocks).ToList();
            }

            return true;
        }

        protected override void OnPuzzleCompleted()
        {
            if (IsEditMode) return;

            //foreach (CatBlock catBlock in GetAllBlocksOfComponent<CatBlock>())
            //{
            //    AddCurrencyFound(catBlock, Currency.SilverPaw, catBlock.Yarn);

            //    if (CatBoxRemovedObjectPooler)
            //    {
            //        PoolObject destructionParticles = CatBoxRemovedObjectPooler.BorrowInstance();
            //        destructionParticles.transform.SetParent(transform);
            //        destructionParticles.transform.position = catBlock.transform.position + Vector3.up * 0.5f;
            //    }

            //    catBlock.CatPawSprite.sprite = null;
            //}
        }

        private int GetNumJigsawPuzzles()
        {
            int numJigsawPuzzles = 0;

            List<JigsawBlock> jigsawBlocks = Stacks.SelectMany(stack => stack.Blocks, (stack, block) => block.GetComponent<JigsawBlock>()).Where(jigsawBlock => jigsawBlock).ToList();

            while (jigsawBlocks.Count > 0)
            {
                // Increment number of jigsaw puzzles.
                ++numJigsawPuzzles;

                // Remove first found piece of current jigsaw puzzle object.
                JigsawPuzzleObject currentJigsawPuzzleObject = jigsawBlocks[0].JigsawPuzzleObject;
                for (int i = 0; i < currentJigsawPuzzleObject.JigsawSprites.Count; i++)
                {
                    jigsawBlocks.Remove(jigsawBlocks.First(j => j.JigsawPuzzleObject == currentJigsawPuzzleObject && j.JigsawIndex == i));
                }
            }

            return numJigsawPuzzles;
        }

        private int GetNumCompletedJigsawPuzzles()
        {
            int numCompletedJigsawPuzzles = 0;

            List<JigsawBlock> jigsawBlocks = Stacks.SelectMany(stack => stack.Blocks, (stack, block) => block.GetComponent<JigsawBlock>()).Where(jigsawBlock => jigsawBlock).ToList();

            while (jigsawBlocks.Count > 0)
            {
                JigsawBlock currentJigsawBlock = jigsawBlocks[0];

                if (currentJigsawBlock.IsComplete)
                {
                    ++numCompletedJigsawPuzzles;
                    foreach (JigsawBlock jigsawBlock in currentJigsawBlock.ConnectedJigsawBlocks)
                    {
                        jigsawBlocks.Remove(jigsawBlock);
                    }
                }
                else
                {
                    jigsawBlocks.Remove(currentJigsawBlock);
                }
            }

            return numCompletedJigsawPuzzles;
        }
    }
}