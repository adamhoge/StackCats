using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats
{
    public delegate void RaiseStacksMovesChanged(int movesRemaining);

    public class GalaxyFlavoredPuzzle : FarmFlavoredPuzzle
    {
        public event RaiseStacksMovesChanged onRaiseStacksMovesChanged;

        public int RaiseStacksInterval = 3;
        public GalaxyBlock GalaxyBlockPrefab;
        public ObjectPooler GalaxyBlockEffectObjectPooler;

        public int RaiseStacksMovesRemaining { get { return _raiseStacksMovesRemaining; } set { _raiseStacksMovesRemaining = value; } }

        private int _raiseStacksMovesRemaining;

        public GalaxyBlock CreateGalaxyBlock(int primaryNumber = 1, int secondaryNumber = 0)
        {
            if (!GalaxyBlockPrefab) return null;

            // Create the new galaxy block.
            GalaxyBlock newGalaxyBlock = Instantiate(GalaxyBlockPrefab);
            newGalaxyBlock.name = "Galaxy Block";
            newGalaxyBlock.PrimaryNumber = primaryNumber;
            newGalaxyBlock.SecondaryNumber = secondaryNumber;
            newGalaxyBlock.onGalaxyBlockTriggered += OnGalaxyBlockTriggered;

            return newGalaxyBlock;
        }

        public void RaiseStacksImmediate()
        {
            _raiseStacksMovesRemaining = RaiseStacksInterval;
            if (onRaiseStacksMovesChanged != null) onRaiseStacksMovesChanged(_raiseStacksMovesRemaining);

            RaiseStacks();
        }

        protected override void Start()
        {
            base.Start();

            if (_raiseStacksMovesRemaining == 0) _raiseStacksMovesRemaining = RaiseStacksInterval;
        }

        protected override void OnAfterBlockMoved(Stack source, Block block, Stack destination)
        {
            base.OnAfterBlockMoved(source, block, destination);

            if (!IsEditMode)
            {
                if (_isComplete) return;

                --_raiseStacksMovesRemaining;

                if (_raiseStacksMovesRemaining == 0)
                {
                    RaiseStacks();
                    _raiseStacksMovesRemaining = RaiseStacksInterval;
                }

                if (onRaiseStacksMovesChanged != null) onRaiseStacksMovesChanged(_raiseStacksMovesRemaining);
            }
        }

        private void RaiseStacks()
        {
            foreach (Stack stack in _stacks)
            {
                if (stack.IsAtMaxCapacity)
                {
                    CompletePuzzle(PuzzleCompletionType.PuzzleFailed);
                    break;
                }
            }

            foreach (Stack stack in _stacks)
            {
                Block block = null;

                float randomValue = Random.value;

                Block bottomBlock = stack.Blocks.Count > 0 ? stack.Blocks[0] : null;
                PuzzleBlock bottomPuzzleBlock = bottomBlock ? bottomBlock.GetComponent<PuzzleBlock>() : null;

                if (bottomBlock && randomValue > 0.6f)
                {
                    block = CreateCatBlock().Block;
                }
                else if (randomValue > 0.5f)
                {
                    PuzzleBlock newPuzzleBlock = PuzzleBuilder.CreateAdjacentPuzzleBlock(this, bottomPuzzleBlock, 1, 5);
                    block = newPuzzleBlock.Block;
                }
                else if (randomValue > 0.4f)
                {
                    block = CreateSumBlock(Random.value > 0.5f ? 1 : -1).Block;
                }
                else if (randomValue > 0.3f && bottomPuzzleBlock == null)
                {
                    block = CreateWildBlock().Block;
                }
                else if (randomValue > 0.1f)
                {
                    GalaxyBlock galaxyBlock = GalaxyFlavoredPuzzleBuilder.CreateAdjacentGalaxyBlock(this, bottomPuzzleBlock, 1, 5);
                    block = galaxyBlock.Block;
                }
                else
                {
                    block = CreateSumBlock(1).Block;
                }

                InsertBlock(stack, 0, block);

                foreach (Block stackBlock in stack.Blocks)
                {
                    LeanTween.cancel(stackBlock.gameObject);
                }

                foreach (FallingBlock fallingBlock in _fallingBlocks[stack])
                {
                    AnimateFallingBlock(stack, fallingBlock.Block, fallingBlock.FromIndex, fallingBlock.ToIndex + 1, fallingBlock.Delay);
                }
            }

            transform.localPosition = Vector2.down;
            LeanTween.moveLocalY(gameObject, 0.0f, 1.0f).setEase(LeanTweenType.easeOutQuint);
        }

        private void OnGalaxyBlockTriggered(GalaxyBlock sender, Stack stack, int fromIndex, int toIndex)
        {
            for (int i = fromIndex; i <= toIndex; i++)
            {
                ParticleEffect galaxyBlockEffect = (ParticleEffect)GalaxyBlockEffectObjectPooler.BorrowInstance();
                galaxyBlockEffect.transform.position = stack.transform.position + stack.GetBlockLocalPosition(i) + Vector3.up * 0.5f;
            }
        }
    }
}