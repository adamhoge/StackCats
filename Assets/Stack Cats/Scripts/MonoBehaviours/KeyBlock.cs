using UnityEngine;

namespace RockhopperGames.StackCats
{
    public class KeyBlock : BlockComponent
    {
        /// <summary>
        /// The visual effect shown when the removal block is destroyed.
        /// </summary>
        public KeyBlockDestructionEffect DestructionEffect;

        public override bool IsPlaceableOn(Block otherBlock)
        {
            return base.IsPlaceableOn(otherBlock);
        }

        public override void OnMove()
        {
            // Check stack for top block and remove it if one exists
            Block blockBelow = Block.GetBlockBelow();
            if (!blockBelow)
                return;

            LockBlock terrainBlockBelow = blockBelow.GetComponent<LockBlock>();
            if (!terrainBlockBelow)
                return;

            Block.ParentStack.RemoveBlocks(blockBelow, Block, true);
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();

            KeyBlockDestructionEffect destructionEffect = Instantiate(DestructionEffect);
            destructionEffect.transform.position = transform.position;
        }
    }
}
