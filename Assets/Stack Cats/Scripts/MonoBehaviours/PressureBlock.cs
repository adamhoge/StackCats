using UnityEngine;
using TMPro;

namespace Tofuwu.StackCats
{
    public delegate void PressureBlockBroke(PressureBlock pressureBlock, Stack stack, int atIndex);

    public class PressureBlock : BlockComponent
    {
        public event PressureBlockBroke onPressureBlockBroke;

        /// <summary>
        /// The amount of blocks above the pressure block required to break it.
        /// </summary>
        public int BreakingPoint { get { return _breakingPoint; } set { SetBreakingPoint(value); } }

        public override bool IsMovable => false;

        /// <summary>
        /// Text indicating the number of blocks above the pressure block.
        /// </summary>
        public TextMeshPro BlocksAboveText;

        /// <summary>
        /// Text indicating the breaking point of the pressure block.
        /// </summary>
        public TextMeshPro BreakingPointText;

        /// <summary>
        /// The visual effect shown when the pressure block is destroyed.
        /// </summary>
        public PressureBlockDestructionEffect DestructionEffect;

        [SerializeField]
        [HideInInspector]
        private int _breakingPoint = 3;

        public override void OnStackChanged()
        {
            int blocksAbove = Block.GetBlocksAbove().Count;

            BlocksAboveText.text = blocksAbove.ToString();

            if (blocksAbove >= BreakingPoint)
            {
                Stack parentStack = Block.ParentStack;
                int blockIndex = parentStack.Blocks.IndexOf(Block);
                Block.ParentStack.RemoveBlock(Block, true);
                if (onPressureBlockBroke != null) onPressureBlockBroke(this, parentStack, blockIndex);
            }
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();

            PressureBlockDestructionEffect destructionEffect = Instantiate(DestructionEffect);
            destructionEffect.BreakingPointText.text = BreakingPointText.text;
            destructionEffect.BlocksAboveText.text = BlocksAboveText.text;
            destructionEffect.transform.position = transform.position;
        }

        protected void Start()
        {
            if (BlocksAboveText) BlocksAboveText.text = Block.GetBlocksAbove().Count.ToString();
            if (BreakingPointText) BreakingPointText.text = _breakingPoint.ToString();
        }

        private void SetBreakingPoint(int value)
        {
            if (_breakingPoint == value) return;

            _breakingPoint = value;
            if (BreakingPointText) BreakingPointText.text = _breakingPoint.ToString();
        }
    }
}