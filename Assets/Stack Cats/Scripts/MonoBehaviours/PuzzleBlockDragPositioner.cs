using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tofuwu.StackCats
{
    public class PuzzleBlockDragPositioner : MonoBehaviour
    {
        public Vector3 DragPosition;

        private PuzzleMarker _selection;
        private List<Block> _selectedBlocks;

        public void SelectBlocks(PuzzleMarker selection)
        {
            int selectionSortingLayerID = SortingLayer.NameToID("Selection");

            _selection = selection;
            _selectedBlocks = _selection.Stack.GetBlocksAt(_selection.Block);

            foreach (Block block in _selectedBlocks)
            {
                SortingGroup blockSortingGroup = block.GetComponent<SortingGroup>();
                if (blockSortingGroup)
                {
                    blockSortingGroup.sortingLayerID = selectionSortingLayerID;
                }

                LeanTween.cancel(block.gameObject);
                block.transform.Translate(Vector3.back * 10.0f);
                block.transform.localScale = Vector3.one * 1.1f;
                LeanTween.scale(block.gameObject, Vector3.one, 0.1f);
            }
        }

        public void ReleaseBlocks()
        {
            List<Block> blocks = _selection.Stack.GetBlocksAt(_selection.Block);
            List<Vector2> blockPositions = new List<Vector2>();
            for (int i = 0; i < blocks.Count; i++)
            {
                Block block = blocks[i];
                Action<object> updateBlockPositions = UpdateBlockPositions;
                LeanTween
                    .moveLocal(block.gameObject, _selection.Stack.GetBlockLocalPosition(block), 0.1f)
                    .setEase(LeanTweenType.easeInOutQuint)
                    .setOnComplete(UpdateBlockPositions, block);
            }

            _selection = null;
            _selectedBlocks = null;
        }

        protected void Update()
        {
            if (_selection == null) return;

            for (int i = 0; i < _selectedBlocks.Count; i++)
            {
                Block selectedBlock = _selectedBlocks[i];
                Vector3 blockPointerPosition = (DragPosition + Vector3.up * transform.lossyScale.y * (_selection.Stack.BlockHeight * i - _selection.Stack.BlockHeight / 2) + Vector3.back * 8.0f);

                selectedBlock.transform.position = Vector3.Lerp(selectedBlock.transform.position, blockPointerPosition, 0.5f);
            }
        }

        private void UpdateBlockPositions(object blockObject)
        {
            Block block = (Block)blockObject;
            block.ParentStack.UpdateBlockPositions(block.ParentStack.Blocks.IndexOf(block));
        }
    }
}