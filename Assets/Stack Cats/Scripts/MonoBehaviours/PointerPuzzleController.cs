using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [RequireComponent(typeof(Puzzle))]
    [RequireComponent(typeof(PuzzleController))]
    [RequireComponent(typeof(PuzzleBlockDragPositioner))]
    public class PointerPuzzleController : PuzzleController
    {
        public Camera Camera;

        private PuzzleBlockDragPositioner _dragPositioner;
        private Vector3 _currentMousePosition;
        private Vector3 _currentPointerPosition;
        protected int _currentStackIndex = -1;
        protected int _currentBlockIndex = -1;

        protected override void Awake()
        {
            base.Awake();

            _dragPositioner = GetComponent<PuzzleBlockDragPositioner>();
        }

        protected virtual void Update()
        {
            UpdateMousePosition(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                Select();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_selection != null)
                {
                    Select();
                }

                Cancel();
                UpdateFocus(_currentStackIndex, _currentBlockIndex);
            }
            else if (Input.GetMouseButtonDown(1)) Cancel();
        }

        private void UpdateMousePosition(Vector3 mousePosition)
        {
            if (mousePosition == _currentMousePosition) return;

            _currentMousePosition = Input.mousePosition;
            Vector3 pointerPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
            UpdatePointerPosition(pointerPosition);
        }

        private void UpdatePointerPosition(Vector3 pointerPosition)
        {
            if (pointerPosition == _currentPointerPosition) return;

            _currentPointerPosition = pointerPosition;
            _dragPositioner.DragPosition = pointerPosition;
            int stackIndex = (int)Mathf.Floor((_currentPointerPosition.x - _puzzle.transform.position.x) / (1.0f + _puzzle.StackSpacing) + (float)_puzzle.Stacks.Count / 2);
            int blockIndex;
            if (stackIndex < 0 || stackIndex >= _puzzle.Stacks.Count)
            {
                blockIndex = -1;
            }
            else
            {
                blockIndex = (int)Mathf.Floor((_currentPointerPosition.y - _puzzle.transform.position.y) / _puzzle.Stacks[stackIndex].BlockHeight);
            }

            UpdateFocus(stackIndex, blockIndex);
        }


        private void UpdateFocus(int stackIndex, int blockIndex)
        {
            if (_puzzle.MaxBlocks == 0 || _currentStackIndex == stackIndex && _currentBlockIndex == blockIndex) return;

            _currentStackIndex = stackIndex;
            _currentBlockIndex = blockIndex;

            if (_currentStackIndex < 0) _currentStackIndex = 0;
            if (_currentStackIndex >= _puzzle.Stacks.Count) _currentStackIndex = _puzzle.Stacks.Count - 1;

            if (_currentBlockIndex < 0 || _currentBlockIndex >= _puzzle.Stacks[_currentStackIndex].Blocks.Count)
            {
                FocusAt(_puzzle.Stacks[_currentStackIndex], null);
            }
            else
            {
                FocusAt(_puzzle.Stacks[_currentStackIndex], _puzzle.Stacks[_currentStackIndex].Blocks[_currentBlockIndex]);
            }
        }

        protected override void OnBlocksSelected(PuzzleMarker selection)
        {
            base.OnBlocksSelected(selection);

            _dragPositioner.SelectBlocks(selection);
        }

        protected override void OnCancelled(PuzzleMarker selection)
        {
            base.OnCancelled(selection);

            _dragPositioner.ReleaseBlocks();

            if (_currentStackIndex < 0 || _currentStackIndex >= _puzzle.Stacks.Count)
            {
                FocusAt(null, null);
            }
            else if (_currentBlockIndex < 0 || _currentBlockIndex >= _puzzle.Stacks[_currentStackIndex].Blocks.Count)
            {
                FocusAt(_puzzle.Stacks[_currentStackIndex], null);
            }
            else
            {
                FocusAt(_puzzle.Stacks[_currentStackIndex], _puzzle.Stacks[_currentStackIndex].Blocks[_currentBlockIndex]);
            }
        }
    }
}