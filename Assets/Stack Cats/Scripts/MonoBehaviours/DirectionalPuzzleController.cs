using System;
using UnityEngine;

namespace Tofuwu.StackCats
{
    // TODO: It may be cleaner to rework as an "axis" controller, supporting holding both axes
    [RequireComponent(typeof(Puzzle))]
    [RequireComponent(typeof(PuzzleController))]
    public class DirectionalPuzzleController : MonoBehaviour
    {
        private class DirectionHold
        {
            public Direction Direction { get { return _direction; } }

            public DirectionHold(Direction direction, float heldAt, float directionHoldDelay, float directionHoldInterval)
            {
                _direction = direction;
                _heldAt = heldAt;
                _directionHoldDelay = directionHoldDelay;
                _directionHoldInterval = directionHoldInterval;
            }

            public bool GetInterval()
            {
                float time = Time.time;

                if (_heldAt + _directionHoldDelay < time && _lastIntervalAt + _directionHoldInterval < time)
                {
                    _lastIntervalAt = time;
                    return true;
                }

                return false;
            }

            private readonly Direction _direction;
            private readonly float _heldAt;
            private readonly float _directionHoldDelay;
            private readonly float _directionHoldInterval;
            private float _lastIntervalAt;
        }

        public float DirectionHoldDelay = 0.25f;
        public float DirectionHoldInterval = 0.125f;
        public bool IgnoreImmovableBlocks = false;
        public AudioEvent MarkerFocusSound;

        protected Puzzle _puzzle;
        protected PuzzleController _puzzleController;
        private DirectionHold _directionHold;
        private int _currentStackIndex = -1;
        private int _currentBlockIndex = -1;

        protected void Awake()
        {
            _puzzle = GetComponent<Puzzle>();
            _puzzleController = GetComponent<PuzzleController>();
        }

        protected void Start()
        {
            FocusInitialBlock();
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) HoldDirection(Direction.Up);
            else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) ReleaseDirection(Direction.Up);

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) HoldDirection(Direction.Right);
            else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) ReleaseDirection(Direction.Right);

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) HoldDirection(Direction.Down);
            else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) ReleaseDirection(Direction.Down);

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) HoldDirection(Direction.Left);
            else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow)) ReleaseDirection(Direction.Left);

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl)) _puzzleController.Select();

            if (Input.GetKeyDown(KeyCode.Escape)) _puzzleController.Cancel();

            if (_directionHold != null && _directionHold.GetInterval())
            {
                if (_directionHold.Direction == Direction.Up) FocusNextBlock();
                if (_directionHold.Direction == Direction.Right) FocusNextStack();
                if (_directionHold.Direction == Direction.Down) FocusPreviousBlock();
                if (_directionHold.Direction == Direction.Left) FocusPreviousStack();
            }
        }

        protected void HoldDirection(Direction direction)
        {
            _directionHold = new DirectionHold(direction, Time.time, DirectionHoldDelay, DirectionHoldInterval);

            switch (direction)
            {
                case Direction.Up:
                    FocusNextBlock();
                    break;
                case Direction.Right:
                    FocusNextStack();
                    break;
                case Direction.Down:
                    FocusPreviousBlock();
                    break;
                case Direction.Left:
                    FocusPreviousStack();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        protected void ReleaseDirection(Direction direction)
        {
            if (_directionHold != null && _directionHold.Direction == direction)
            {
                _directionHold = null;
            }
        }

        private void FocusPreviousStack()
        {
            if (_puzzleController.Selection != null || _currentStackIndex <= 0) return;

            Stack stack = null;
            for (int i = _currentStackIndex - 1; i >= 0; i--)
            {
                if (_puzzle.Stacks[i].Blocks.Count > 0)
                {
                    stack = _puzzle.Stacks[i];
                    _currentStackIndex = i;
                    break;
                }
            }

            if (!stack) return;

            _currentBlockIndex = _currentBlockIndex < stack.Blocks.Count ? _currentBlockIndex : stack.Blocks.Count - 1;
            if (IgnoreImmovableBlocks)
            {
                int lowestIndex = GetLowestMovableBlockIndex(stack);
                if (_currentBlockIndex < lowestIndex) _currentBlockIndex = lowestIndex;
            }
            Block block = stack.Blocks[_currentBlockIndex];

            _puzzleController.FocusAt(stack, block);
            if (MarkerFocusSound) GameManager.Instance.Audio.PlaySoundEffect(MarkerFocusSound);
        }

        private void FocusNextStack()
        {
            if (_puzzleController.Selection != null || _currentStackIndex == _puzzle.Stacks.Count - 1) return;

            Stack stack = null;
            for (int i = _currentStackIndex + 1; i < _puzzle.Stacks.Count; i++)
            {
                if (_puzzle.Stacks[i].Blocks.Count > 0)
                {
                    stack = _puzzle.Stacks[i];
                    _currentStackIndex = i;
                    break;
                }
            }

            if (!stack) return;

            _currentBlockIndex = _currentBlockIndex < stack.Blocks.Count ? _currentBlockIndex : stack.Blocks.Count - 1;
            if (IgnoreImmovableBlocks)
            {
                int lowestIndex = GetLowestMovableBlockIndex(stack);
                if (_currentBlockIndex < lowestIndex) _currentBlockIndex = lowestIndex;
            }
            Block block = stack.Blocks[_currentBlockIndex];

            _puzzleController.FocusAt(stack, block);
            if (MarkerFocusSound) GameManager.Instance.Audio.PlaySoundEffect(MarkerFocusSound);
        }

        private void FocusPreviousBlock()
        {
            if (_puzzleController.Selection != null) return;

            if (_currentBlockIndex > 0)
            {
                --_currentBlockIndex;
                Stack stack = _puzzle.Stacks[_currentStackIndex];
                Block block = stack.Blocks[_currentBlockIndex];
                if (IgnoreImmovableBlocks && !_puzzle.IsMovable(stack, block))
                {
                    ++_currentBlockIndex;
                    return;
                }
                _puzzleController.FocusAt(stack, block);
                if (MarkerFocusSound) GameManager.Instance.Audio.PlaySoundEffect(MarkerFocusSound);
            }
        }

        private void FocusNextBlock()
        {
            if (_puzzleController.Selection != null) return;

            if (_currentBlockIndex < _puzzle.Stacks[_currentStackIndex].Blocks.Count - 1)
            {
                ++_currentBlockIndex;
                Stack stack = _puzzle.Stacks[_currentStackIndex];
                Block block = stack.Blocks[_currentBlockIndex];
                if (IgnoreImmovableBlocks && !_puzzle.IsMovable(stack, block))
                {
                    --_currentBlockIndex;
                    return;
                }
                _puzzleController.FocusAt(stack, block);
                if (MarkerFocusSound) GameManager.Instance.Audio.PlaySoundEffect(MarkerFocusSound);
            }
        }

        private void FocusInitialBlock()
        {
            for (int i = 0; i < _puzzle.Stacks.Count; i++)
            {
                if (_puzzle.Stacks[i].Blocks.Count > 0)
                {
                    Stack stack = _puzzle.Stacks[i];
                    Block block = stack.TopBlock;

                    _currentStackIndex = i;
                    _currentBlockIndex = stack.Blocks.IndexOf(block);

                    _puzzleController.FocusAt(stack, block);
                    break;
                }
            }
        }

        private int GetLowestMovableBlockIndex(Stack stack)
        {
            int lowestIndex = stack.Blocks.Count - 1;
            for (int i = lowestIndex; i >= 0; i--)
            {
                Block block = stack.Blocks[i];
                if (_puzzle.IsMovable(stack, block))
                {
                    lowestIndex = i;
                }
                else
                {
                    break;
                }
            }

            return lowestIndex;
        }
    }
}