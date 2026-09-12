using UnityEngine;

namespace Tofuwu.StackCats
{
    public class TerrainBlock : BlockComponent
    {
        /// <summary>
        /// The visual effect shown when the terrain block is destroyed.
        /// </summary>
        public TerrainBlockDestructionEffect DestructionEffect;

        public override bool IsMovable { get { return false; } }

        public override void OnDestroyed()
        {
            base.OnDestroyed();

            TerrainBlockDestructionEffect destructionEffect = Instantiate(DestructionEffect);
            destructionEffect.transform.position = transform.position;
        }
    }
}