using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Tofuwu.StackCats.Data;

namespace Tofuwu.StackCats
{
    public class FarmFlavoredPuzzle : Puzzle
    {
        /// <summary>
        /// Check if a block is movable.
        /// </summary>
        /// <param name="source">The source stack.</param>
        /// <param name="block">The block to check.</param>
        /// <returns>A flag indicating whether or not the provided block is movable.</returns>
        public override bool IsMovable(Stack source, Block block)
        {
            if (!source || !block || !source.Blocks.Contains(block)) return false;

            if (block.GetComponents<BlockComponent>().FirstOrDefault(bc => !bc.IsMovable)) return false;

            Type targetBlockType = block.GetComponent<PuzzleBlock>() ? typeof(PuzzleBlock) : block.GetComponent<BlockComponent>().GetType();
            if (targetBlockType == typeof(CatBlock)) return false;

            for (int i = source.Blocks.Count - 1; i >= 0; i--)
            {
                Block destinationBlock = source.Blocks[i];
                Type destinationBlockType = destinationBlock.GetComponent<PuzzleBlock>() ? typeof(PuzzleBlock) : destinationBlock.GetComponent<BlockComponent>().GetType();

                if (destinationBlock == block) return true;

                if (destinationBlockType == typeof(CatBlock) || destinationBlockType != targetBlockType) return false;
            }

            return true;
        }

        /// <summary>
        /// Check if a block is placeable on another block.
        /// </summary>
        /// <param name="from">The placed block.</param>
        /// <param name="to">The block on which the other is placed.</param>
        /// <returns>A flag indicating whether or not the provided block is placeable on the target block.</returns>
        public override bool IsPlaceable(Block from, Block to)
        {
            return from.GetComponents<BlockComponent>().All(bc => !bc.enabled || bc.IsPlaceableOn(to));
        }

        /// <summary>
        /// Get a list of rule exceptions preventing the block from being moved.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public override List<string> GetIsPlaceableRuleExceptions(Block from, Block to)
        {
            List<string> ruleExceptions = new List<string>();
            foreach(BlockComponent blockComponent in from.GetComponents<BlockComponent>())
            {
                ruleExceptions.AddRange(blockComponent.GetIsPlaceableOnRuleExceptions(to));
            }
            return ruleExceptions;
        }

        protected new void Awake()
        {
            base.Awake();
        }

        protected virtual void Start()
        {
            if (!IsEditMode)
            {
                if (IsComplete()) CompletePuzzle(PuzzleCompletionType.PuzzleSolved);
            }
        }

        protected override bool IsComplete()
        {
            bool isComplete = !PuzzleContainsCatBlocks();
            return isComplete;
        }

        private bool PuzzleContainsCatBlocks()
        {
            return Stacks.Any(s => s.Blocks.Any(b => b.GetComponent<CatBlock>()));
        }
    }
}