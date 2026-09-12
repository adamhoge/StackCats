using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tofuwu.StackCats
{
    public delegate void BlockInserted(Stack sender, Block block, int atIndex);
    public delegate void BlocksRemoved(Stack sender, List<Block> blocks, int fromIndex, int toIndex);

    public class Stack : MonoBehaviour, IComparable<Stack>
    {
        public event BlockInserted onBlockInserted;
        public event BlocksRemoved onBlocksRemoved;

        /// <summary>
        /// All blocks belonging to the stack.
        /// </summary>
        public ReadOnlyCollection<Block> Blocks { get { return _blocks.AsReadOnly(); } }

        /// <summary>
        /// The maximum number of blocks which may be placed on the stack.
        /// </summary>
        public int MaxBlocks { get { return _maxBlocks; } set { SetMaxBlocks(value); } }

        /// <summary>
        /// The height of blocks contained on the stack.
        /// </summary>
        public float BlockHeight { get { return _blockHeight; } set { SetBlockHeight(value); } }

        /// <summary>
        /// The block currently at the top of the stack.
        /// </summary>
        public Block TopBlock { get { return _blocks.Count > 0 ? _blocks[_blocks.Count - 1] : null; } }

        /// <summary>
        /// A flag indicating whether or not the stack is empty.
        /// </summary>
        public bool IsEmpty { get { return _blocks.Count == 0; } }

        /// <summary>
        /// A flag indicating whether or not the stack is a maximum block capacity.
        /// </summary>
        public bool IsAtMaxCapacity { get { return _blocks.Count == MaxBlocks; } }

        [SerializeField]
        [HideInInspector]
        private List<Block> _blocks = new List<Block>();

        [SerializeField]
        [HideInInspector]
        private int _maxBlocks = 10;

        [SerializeField]
        [HideInInspector]
        private float _blockHeight = 1.0f;

        /// <summary>
        /// Add a block to the stack.
        /// </summary>
        /// <param name="block">The block to be added.</param>
        /// <returns>A flag indicating whether or not the block was successfully added.</returns>
        public bool AddBlock(Block block)
        {
            return InsertBlock(block, _blocks.Count);
        }

        /// <summary>
        /// Add a set of blocks to the stack.
        /// </summary>
        /// <param name="block">The blocks to be added.</param>
        /// <returns>A flag indicating whether or not the blocks were successfully added.</returns>
        public bool AddBlocks(List<Block> blocks)
        {
            if (MaxBlocks - _blocks.Count < blocks.Count) return false;

            foreach (Block block in blocks)
            {
                AddBlock(block);
            }

            return true;
        }

        /// <summary>
        /// Insert a block into the stack at a given index.
        /// </summary>
        /// <param name="block">The block inserted.</param>
        /// <param name="atIndex">The index at which to insert the block.</param>
        /// <returns></returns>
        public bool InsertBlock(Block block, int atIndex)
        {
            if (IsAtMaxCapacity) return false;

            if (!block || _blocks.Contains(block) || atIndex > _blocks.Count) return false;

            _blocks.Insert(atIndex, block);
            block.ParentStack = this;
            block.transform.SetParent(transform);

            UpdateBlockPositions(atIndex);
            if (onBlockInserted != null) onBlockInserted(this, block, atIndex);
            return true;
        }

        /// <summary>
        /// Remove a block from the stack.
        /// </summary>
        /// <param name="block">The block to be removed.</param>
        /// <returns>A flag indicating whether or not the block was succesfully removed.</returns>
        public bool RemoveBlock(Block block, bool destroy = false)
        {
            return RemoveBlocks(block, block, destroy);
        }

        /// <summary>
        /// Remove all blocks at and above the specified block.
        /// </summary>
        /// <param name="from">The first block removed.</param>
        /// <param name="destroy">Flag indicating whether or not the removed blocks should be destroyed.</param>
        public bool RemoveBlocks(Block from, bool destroy = false)
        {
            return RemoveBlocks(from, TopBlock, destroy);
        }

        /// <summary>
        /// Remove a range of blocks from the stack.
        /// </summary>
        /// <param name="from">The first block removed.</param>
        /// <param name="to">The last block removed.</param>
        /// <param name="destroy">Flag indicating whether or not the removed blocks should be destroyed.</param>
        public bool RemoveBlocks(Block from, Block to, bool destroy = false)
        {
            // If the block doesn't exist in the stack, don't attempt to remove it.
            if (!_blocks.Contains(from) || !_blocks.Contains(to)) return false;

            // Get the indices of the blocks.
            int fromBlockIndex = _blocks.IndexOf(from);
            int toBlockIndex = _blocks.IndexOf(to);

            // Get the list of removed blocks.
            List<Block> blocksRemoved = GetBlocksBetween(fromBlockIndex, toBlockIndex);

            // Remove them.
            foreach (Block block in blocksRemoved)
            {
                _blocks.Remove(block);

                if (destroy)
                {
                    foreach (BlockComponent blockComponent in block.GetComponents<BlockComponent>())
                    {
                        blockComponent.OnDestroyed();
                    }
                    Destroy(block.gameObject);
                }
                else
                {
                    block.ParentStack = null;
                    block.transform.SetParent(null);
                }
            }
            UpdateBlockPositions(fromBlockIndex);

            if (onBlocksRemoved != null) onBlocksRemoved(this, blocksRemoved, fromBlockIndex, toBlockIndex);

            return true;
        }

        /// <summary>
        /// Get a list consisting of the provided block and all blocks above it.
        /// </summary>
        /// <param name="block">The starting block.</param>
        /// <returns>A list consisting of the provided block and all blocks above it.</returns>
        public List<Block> GetBlocksAt(Block block)
        {
            List<Block> blocks = new List<Block>();

            int blockIndex = _blocks.IndexOf(block);
            if (blockIndex == -1) return blocks;

            for (int i = blockIndex; i < _blocks.Count; i++)
            {
                blocks.Add(_blocks[i]);
            }

            return blocks;
        }

        /// <summary>
        /// Get the local position of a block.
        /// </summary>
        /// <param name="block">The block checked.</param>
        /// <returns></returns>
        public Vector3 GetBlockLocalPosition(Block block)
        {
            if (!_blocks.Contains(block)) return Vector3.zero;

            return GetBlockLocalPosition(_blocks.IndexOf(block));
        }

        /// <summary>
        /// Get the local position of a (hypothetical) block by index.
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <returns></returns>
        public Vector3 GetBlockLocalPosition(int blockIndex)
        {
            return new Vector3(0, _blockHeight * blockIndex, 0);
        }

        /// <summary>
        /// Get all blocks between the provided indices.
        /// </summary>
        /// <param name="minIndex">The minimum block index.</param>
        /// <param name="maxIndex">The maximum block index.</param>
        /// <returns></returns>
        public List<Block> GetBlocksBetween(int minIndex, int maxIndex)
        {
            List<Block> blocks = new List<Block>();

            if (minIndex < 0) minIndex = 0;
            if (maxIndex >= _maxBlocks) maxIndex = _maxBlocks - 1;

            while (minIndex <= maxIndex && minIndex < _blocks.Count)
            {
                blocks.Add(_blocks[minIndex]);
                minIndex++;
            }

            return blocks;
        }

        public void UpdateBlockPosition(int i)
        {
            int blockSortingLayer = SortingLayer.NameToID("Block");

            Block block = _blocks[i];
            block.transform.localPosition = GetBlockLocalPosition(block);
            block.transform.localScale = new Vector3(1.0f, _blockHeight, 1.0f);

            SortingGroup blockSortingGroup = block.GetComponent<SortingGroup>();
            if (blockSortingGroup)
            {
                blockSortingGroup.sortingLayerID = blockSortingLayer;
                blockSortingGroup.sortingOrder = i;
            }
        }

        /// <summary>
        /// Update the block positions from the specified index.
        /// </summary>
        /// <param name="startingAt">The index from which to update the block positions.</param>
        public void UpdateBlockPositions(int startingAt = 0)
        {
            int numBlocks = _blocks.Count;
            for (int i = startingAt; i < numBlocks; i++)
            {
                UpdateBlockPosition(i);
            }
        }

        private void SetMaxBlocks(int value)
        {
            if (value == _maxBlocks) return;

            _maxBlocks = value;
            while (_blocks.Count > MaxBlocks)
            {
                Block topBlock = TopBlock;
                _blocks.Remove(topBlock);
                Destroy(topBlock.gameObject);
            }
        }

        private void SetBlockHeight(float value)
        {
            if (value == _blockHeight) return;

            _blockHeight = value;
            UpdateBlockPositions();
        }

        public int CompareTo(Stack otherStack)
        {
            return Blocks.Count.CompareTo(otherStack.Blocks.Count);
        }
    }
}