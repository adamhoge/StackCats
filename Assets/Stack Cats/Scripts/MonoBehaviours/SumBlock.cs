using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class SumBlock : BlockComponent
    {
        public Color PositiveColor = Color.green;
        public Color NegativeColor = Color.red;
        public AudioEvent PositiveSoundEffect;
        public AudioEvent NegativeSoundEffect;
        public TextMeshPro SumText;
        public SpriteRenderer BlockGraphic;
        public SpriteRenderer ActionIndicatorSpriteRenderer;
        public SpriteRenderer SumBackground;
        public SpriteMask SumBackgroundMask;
        public float BaseEffectDuration = 0.5f;
        public float MaxAddEffectDuration = 1.0f;
        public SumBlockDestructionEffect DestructionEffect;

        public int SumValue { get { return _sumValue; } set { SetSumValue(value); } }

        [SerializeField]
        [HideInInspector]
        private int _sumValue = 1;

        public override bool IsPlaceableOn(Block block)
        {
            if (!block) return true;

            WildBlock wildBlock = block.GetComponent<WildBlock>();
            if (wildBlock)
            {
                return false;
            }

            PuzzleBlock puzzleBlock = block.GetComponent<PuzzleBlock>();
            if (!puzzleBlock) return true;

            int totalSumValue = SumValue;
            Block blockAbove = Block.GetBlockAbove();
            SumBlock sumBlockAbove = blockAbove ? blockAbove.GetComponent<SumBlock>() : null;
            while (sumBlockAbove)
            {
                totalSumValue += sumBlockAbove.SumValue;
                blockAbove = blockAbove.GetBlockAbove();
                sumBlockAbove = blockAbove ? blockAbove.GetComponent<SumBlock>() : null;
            }

            int minSummedValue = puzzleBlock.PrimaryNumber + totalSumValue;
            if (minSummedValue < 1) return false;

            PuzzleBlock maxPuzzleBlock = puzzleBlock;
            Block blockBelow = maxPuzzleBlock.Block.GetBlockBelow();
            PuzzleBlock puzzleBlockBelow = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            while (puzzleBlockBelow)
            {
                maxPuzzleBlock = puzzleBlockBelow;
                blockBelow = blockBelow.GetBlockBelow();
                puzzleBlockBelow = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            }

            return maxPuzzleBlock.PrimaryNumber + totalSumValue <= 9;
        }

        public override List<string> GetIsPlaceableOnRuleExceptions(Block block)
        {
            List<string> ruleExceptions = new List<string>();

            WildBlock wildBlock = block.GetComponent<WildBlock>();
            if (wildBlock)
            {
                ruleExceptions.Add("Sum Blocks cannot be placed on Wild Blocks");
                return ruleExceptions;
            }

            PuzzleBlock puzzleBlock = block.GetComponent<PuzzleBlock>();
            if (!puzzleBlock) return ruleExceptions;

            int totalSumValue = SumValue;
            Block blockAbove = Block.GetBlockAbove();
            SumBlock sumBlockAbove = blockAbove ? blockAbove.GetComponent<SumBlock>() : null;
            while (sumBlockAbove)
            {
                totalSumValue += sumBlockAbove.SumValue;
                blockAbove = blockAbove.GetBlockAbove();
                sumBlockAbove = blockAbove ? blockAbove.GetComponent<SumBlock>() : null;
            }

            int minSummedValue = puzzleBlock.PrimaryNumber + totalSumValue;
            if (minSummedValue < 1) ruleExceptions.Add("Puzzle Block values must be between 1 and 9");

            PuzzleBlock maxPuzzleBlock = puzzleBlock;
            Block blockBelow = maxPuzzleBlock.Block.GetBlockBelow();
            PuzzleBlock puzzleBlockBelow = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            while (puzzleBlockBelow)
            {
                maxPuzzleBlock = puzzleBlockBelow;
                blockBelow = blockBelow.GetBlockBelow();
                puzzleBlockBelow = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            }
            if (maxPuzzleBlock.PrimaryNumber + totalSumValue > 9) { ruleExceptions.Add("Puzzle Block values must be between 1 and 9"); }

            return ruleExceptions;
        }

        public override void OnMove()
        {
            Stack parentStack = _block.ParentStack;
            if (!parentStack) return;

            Block blockBelow = _block.GetBlockBelow();
            float addEffectDuration = MaxAddEffectDuration / parentStack.MaxBlocks;

            int sumValue = _sumValue;

            PuzzleBlock puzzleBlockBelow = blockBelow ? blockBelow.GetComponent<PuzzleBlock>() : null;
            if (puzzleBlockBelow && puzzleBlockBelow.enabled)
            {
                // Combine all sum blocks above.
                foreach (Block block in _block.GetBlocksAbove())
                {
                    SumBlock sumBlock = block.GetComponent<SumBlock>();
                    if (sumBlock) sumValue += sumBlock.SumValue;
                    parentStack.RemoveBlock(block, true);
                }

                // Apply sum to all puzzle blocks below (until a non-puzzle block is reached).
                int startIndex = parentStack.Blocks.Count - 2;
                for (int i = startIndex; i >= 0; i--)
                {
                    PuzzleBlock puzzleBlock = parentStack.Blocks[i].GetComponent<PuzzleBlock>();
                    if (puzzleBlock)
                    {
                        SumPuzzleBlock(puzzleBlock, sumValue, BaseEffectDuration + (startIndex - i) * addEffectDuration);
                    }
                    else break;
                }

                // Play a sound effect.
                GameManager gameManager = GameManager.Instance;
                if (_sumValue > 0)
                {
                    gameManager.Audio.PlaySoundEffect(PositiveSoundEffect);
                }
                else
                {
                    gameManager.Audio.PlaySoundEffect(NegativeSoundEffect);
                }

                // Remove the sum block from the stack.
                // TODO: Possible issue if multiple components want to destroy a block.
                _block.ParentStack.RemoveBlock(_block, true);
            }
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();

            SumBlockDestructionEffect destructionEffect = Instantiate(DestructionEffect, transform.parent);
            destructionEffect.SumBlock.SumValue = SumValue;
            destructionEffect.transform.position = transform.position;
        }

        protected void Update()
        {
            float positionY = (Time.time * 0.25f) % 1.0f;
            if (_sumValue < 0) positionY = 1.0f - positionY;
            SumBackground.transform.localPosition = Vector2.up * positionY;
        }

        private void SumPuzzleBlock(PuzzleBlock puzzleBlock, int sumValue, float effectDuration)
        {
            if (!puzzleBlock) return;

            puzzleBlock.PrimaryNumber += sumValue;
            puzzleBlock.Block.FlashBlock(sumValue >= 0 ? PositiveColor : NegativeColor, effectDuration, LeanTweenType.easeInSine);
        }

        private void SetSumValue(int sumValue)
        {
            if (sumValue == _sumValue) return;

            _sumValue = sumValue;

            // Update the visual text.
            if (_sumValue > 0)
            {
                BlockGraphic.color = PositiveColor;
                ActionIndicatorSpriteRenderer.color = PositiveColor;
                SumBackground.flipY = false;
                SumText.text = "+" + _sumValue;
            }
            else
            {
                BlockGraphic.color = NegativeColor;
                ActionIndicatorSpriteRenderer.color = NegativeColor;
                SumBackground.flipY = true;
                SumText.text = _sumValue.ToString();
            }
        }
    }
}