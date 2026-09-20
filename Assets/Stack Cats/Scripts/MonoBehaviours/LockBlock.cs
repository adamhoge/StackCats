using UnityEngine;

namespace Tofuwu.StackCats
{
    public class LockBlock : BlockComponent
    {
        /// <summary>
        /// The visual effect shown when the terrain block is destroyed.
        /// </summary>
        public LockBlockDestructionEffect DestructionEffect;

        public override bool IsMovable
        {
            get { return false; }
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();

            LockBlockDestructionEffect destructionEffect = Instantiate(DestructionEffect);
            destructionEffect.transform.position = transform.position;
        }
    }
}
