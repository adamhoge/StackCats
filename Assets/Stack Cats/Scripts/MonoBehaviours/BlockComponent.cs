using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    /// <summary>
    /// Base MonoBehaviour for all block components.
    /// </summary>
    [RequireComponent(typeof(Block))]
    public abstract class BlockComponent : MonoBehaviour, IBlockComponent
    {
        protected Block _block;

        /// <summary>
        /// The block with which the component is associated.
        /// </summary>
        public Block Block { get { return _block; } }

        public virtual bool IsMovable { get { return true; } }

        public virtual bool IsPlaceableOn(Block block) { return true; }

        public virtual List<string> GetIsPlaceableOnRuleExceptions(Block block) { return new List<string>(); }

        public virtual void OnMove() { }

        public virtual void OnDestroyed() { }

        public virtual void OnStackChanged() { }

        protected virtual void Awake()
        {
            _block = GetComponent<Block>();
        }
    }
}