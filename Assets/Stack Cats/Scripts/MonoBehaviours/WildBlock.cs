using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [RequireComponent(typeof(PuzzleBlock))]
    public class WildBlock : BlockComponent
    {
        public SpriteRenderer WildBlockSymbol;
        public SpriteRenderer BeanSprite;
        public SpriteRenderer TopActivationIndicator;
        public SpriteRenderer BottomActivationIndicator;

        private PuzzleBlock _puzzleBlockPlaceholder;

        protected override void Awake()
        {
            base.Awake();

            _puzzleBlockPlaceholder = GetComponent<PuzzleBlock>();
            DisablePuzzleBlock();
        }

        public override bool IsPlaceableOn(Block otherBlock)
        {
            if (!otherBlock) return true;

            SumBlock sumBlock = otherBlock.GetComponent<SumBlock>();
            if (sumBlock) return false;

            PuzzleBlock puzzleBlock = otherBlock.GetComponent<PuzzleBlock>();
            if (!puzzleBlock || !puzzleBlock.enabled) return true;

            int numWildBlocks = 1;
            Block currentBlockAbove = Block.GetBlockAbove();
            while (currentBlockAbove)
            {
                ++numWildBlocks;
                currentBlockAbove = currentBlockAbove.GetBlockAbove();
            }

            return puzzleBlock.PrimaryNumber - numWildBlocks > 0;
        }

        public override List<string> GetIsPlaceableOnRuleExceptions(Block otherBlock)
        {
            List<string> ruleExceptions = new List<string>();

            SumBlock sumBlock = otherBlock.GetComponent<SumBlock>();
            if (sumBlock)
            {
                ruleExceptions.Add("Wild Blocks cannot be placed on Sum Blocks");
                return ruleExceptions;
            }

            PuzzleBlock puzzleBlock = otherBlock.GetComponent<PuzzleBlock>();
            if (!puzzleBlock || !puzzleBlock.enabled)
            {
                return ruleExceptions;
            }

            int numWildBlocks = 1;
            Block currentBlockAbove = Block.GetBlockAbove();
            while (currentBlockAbove)
            {
                ++numWildBlocks;
                currentBlockAbove = currentBlockAbove.GetBlockAbove();
            }

            if (puzzleBlock.PrimaryNumber - numWildBlocks < 1) { ruleExceptions.Add("Wild Block values must be between 1 and 9"); }

            return ruleExceptions;
        }

        public override void OnStackChanged()
        {
            base.OnStackChanged();

            TryTriggerAgainstBlock(Block.GetBlockAbove(), 1);
        }

        public override void OnMove()
        {
            base.OnMove();

            TryTriggerAgainstBlock(Block.GetBlockBelow(), -1);
        }

        private void TryTriggerAgainstBlock(Block block, int direction)
        {
            if (!block) return;

            PuzzleBlock puzzleBlock = block.GetComponent<PuzzleBlock>();
            if (!puzzleBlock || !puzzleBlock.enabled) return;

            Block.FlashBlock(Color.white, 0.5f);
            int primaryNumber = puzzleBlock.PrimaryNumber + direction;
            int secondaryNumber = (puzzleBlock.SecondaryNumber + 1) % 2;
            EnablePuzzleBlock(primaryNumber, secondaryNumber);
            DestroyImmediate(this);
        }

        private void DisablePuzzleBlock()
        {
            _puzzleBlockPlaceholder.enabled = false;
            _puzzleBlockPlaceholder.PrimaryNumberText.enabled = false;
            _puzzleBlockPlaceholder.SecondaryNumberSprite.enabled = false;
        }

        private void EnablePuzzleBlock(int primaryNumber, int secondaryNumber)
        {
            _puzzleBlockPlaceholder.enabled = true;
            _puzzleBlockPlaceholder.PrimaryNumberText.enabled = true;
            _puzzleBlockPlaceholder.SecondaryNumberSprite.enabled = true;
            _puzzleBlockPlaceholder.PrimaryNumber = primaryNumber;
            _puzzleBlockPlaceholder.SecondaryNumber = secondaryNumber;

            Destroy(WildBlockSymbol);
            Destroy(BeanSprite);
            Destroy(TopActivationIndicator);
            Destroy(BottomActivationIndicator);
        }
    }
}