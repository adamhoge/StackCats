using UnityEngine;
using TMPro;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tofuwu.StackCats
{
    [DisallowMultipleComponent]
    public class PuzzleBlock : BlockComponent
    {
        public TextMeshPro PrimaryNumberText;
        public SpriteRenderer SecondaryNumberSprite;
        public Color SecondaryNumberColor0 = new Color(0.4f, 0.4f, 0.4f);
        public Color SecondaryNumberColor1 = new Color(1.0f, 0.6f, 0.8f);

        /// <summary>
        /// The primary number value of the puzzle block.
        /// </summary>
        public int PrimaryNumber { get { return _primaryNumber; } set { SetPrimaryNumber(value); } }

        /// <summary>
        /// The secondary number value of the puzzle block.
        /// </summary>
        public int SecondaryNumber { get { return _secondaryNumber; } set { SetSecondaryNumber(value); } }

        [SerializeField]
        [HideInInspector]
        private int _primaryNumber = 1;

        [SerializeField]
        [HideInInspector]
        private int _secondaryNumber;

        public override bool IsPlaceableOn(Block otherBlock)
        {
            if (!otherBlock || otherBlock.GetComponent<CatBlock>()) return true;

            if (otherBlock.GetComponent<SumBlock>()) return false;

            if (otherBlock.GetComponent<WildBlock>())
            {
                int numWildBlocks = 1;
                Block blockBelow = otherBlock.GetBlockBelow();
                WildBlock wildBlockBelow = blockBelow ? blockBelow.GetComponent<WildBlock>() : null;
                while (wildBlockBelow)
                {
                    ++numWildBlocks;
                    blockBelow = blockBelow.GetBlockBelow();
                    wildBlockBelow = blockBelow ? blockBelow.GetComponent<WildBlock>() : null;
                }

                return PrimaryNumber <= (9 - numWildBlocks);
            }

            PuzzleBlock otherPuzzleBlock = otherBlock.GetComponent<PuzzleBlock>();
            if (!otherPuzzleBlock) return true;

            bool isValidPrimaryPlacement = otherPuzzleBlock.PrimaryNumber - _primaryNumber == 1;
            if (!isValidPrimaryPlacement) return false;

            bool isValidSecondaryPlacement = Mathf.Abs(_secondaryNumber - otherPuzzleBlock.SecondaryNumber) == 1;
            return isValidSecondaryPlacement;
        }

        public override List<string> GetIsPlaceableOnRuleExceptions(Block otherBlock)
        {
            List<string> ruleExceptions = new List<string>();

            if (!otherBlock || GetComponent<WildBlock>()) return ruleExceptions;

            if (otherBlock.GetComponent<SumBlock>())
            {
                ruleExceptions.Add("Puzzle Blocks cannot be placed on Sum Blocks");
            }

            if (otherBlock.GetComponent<WildBlock>())
            {
                int numWildBlocks = 1;
                Block blockBelow = otherBlock.GetBlockBelow();
                WildBlock wildBlockBelow = blockBelow ? blockBelow.GetComponent<WildBlock>() : null;
                while (wildBlockBelow)
                {
                    ++numWildBlocks;
                    blockBelow = blockBelow.GetBlockBelow();
                    wildBlockBelow = blockBelow ? blockBelow.GetComponent<WildBlock>() : null;
                }

                if (PrimaryNumber <= numWildBlocks) { ruleExceptions.Add("Wild Block values must be between 1 and 9"); }
            }

            PuzzleBlock onPuzzleBlock = otherBlock.GetComponent<PuzzleBlock>();
            if (onPuzzleBlock)
            {
                bool isValidPrimaryPlacement = onPuzzleBlock.PrimaryNumber - _primaryNumber == 1;
                if (!isValidPrimaryPlacement) { ruleExceptions.Add("Puzzle Blocks must be one less than the one below it"); }

                bool isValidSecondaryPlacement = Mathf.Abs(_secondaryNumber - onPuzzleBlock.SecondaryNumber) == 1;
                if (!isValidSecondaryPlacement) { ruleExceptions.Add("Puzzle Blocks must be alternating colors"); }
            }

            return ruleExceptions;
        }

        protected void Start()
        {
            if (PrimaryNumberText) PrimaryNumberText.text = _primaryNumber.ToString();

            if (SecondaryNumberSprite)
            {
                Color colorValue = _secondaryNumber == 1 ? SecondaryNumberColor1 : SecondaryNumberColor0;
                SecondaryNumberSprite.color = colorValue;
            }
        }

        private void SetPrimaryNumber(int value)
        {
            if (value == _primaryNumber) return;

            if (value > 9) Debug.Log("Primary Number set above 9");
            if (value < 1) Debug.Log("Primary Number set to 0");

            _primaryNumber = value;
            if (PrimaryNumberText) PrimaryNumberText.text = _primaryNumber.ToString();
        }

        private void SetSecondaryNumber(int value)
        {
            if (value == _secondaryNumber) return;

            _secondaryNumber = value;
            if (SecondaryNumberSprite)
            {
                Color colorValue = _secondaryNumber == 1 ? SecondaryNumberColor1 : SecondaryNumberColor0;
                SecondaryNumberSprite.color = colorValue;
            }
        }
    }
}