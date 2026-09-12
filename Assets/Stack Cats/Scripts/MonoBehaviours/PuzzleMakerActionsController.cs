using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tofuwu.StackCats
{
    [System.Serializable]
    public class PointerAction
    {
        public string Label;
        public UnityAction<int, int, bool> Action;
    }

    public delegate void BlockTypeChanged(BlockType blockType);

    public abstract class PuzzleMakerActionsController : PointerPuzzleController
    {
        /// <summary>
        /// Invoked whenever the puzzle maker action controller's "add block" type changes.
        /// </summary>
        public event BlockTypeChanged onBlockTypeChanged;

        /// <summary>
        /// The puzzle maker actions object responsible for acting upon the puzzle.
        /// </summary>
        public PuzzleMakerActions PuzzleMakerActions;

        /// <summary>
        /// All pointer actions correlating with the actions controller.
        /// </summary>
        public List<PointerAction> PointerActions
        {
            get { return _pointerActions; }
        }

        /// <summary>
        /// Flag indicating whether or not the current action should be inverted.
        /// </summary>
        public bool InvertAction;

        public bool IsEnabled = true;

        protected PointerAction _currentPointerAction;
        protected List<PointerAction> _pointerActions = new List<PointerAction>();
        protected BlockType _currentBlockType;

        /// <summary>
        /// Set the pointer action.
        /// </summary>
        /// <param name="pointerAction">The pointer action that should be used. If null, pointer will be used to move blocks.</param>
        public void SetPointerAction(PointerAction pointerAction)
        {
            _currentPointerAction = pointerAction;
        }

        public void SetPointerAction(string pointerActionLabel)
        {
            PointerAction pointerAction = _pointerActions.Find(pa =>
                pa.Label == pointerActionLabel
            );

            if (pointerAction != null)
                SetPointerAction(pointerAction);
        }

        public void ChangeBlockType(BlockType blockType)
        {
            _currentBlockType = blockType;

            if (onBlockTypeChanged != null)
                onBlockTypeChanged(_currentBlockType);
        }

        public override void Select()
        {
            if (_currentPointerAction.Action == null)
                base.Select();
        }

        protected virtual void Start()
        {
            _pointerActions.Add(new PointerAction { Label = "None", Action = DoNothing });
            _pointerActions.Add(
                new PointerAction { Label = "Add/Remove Blocks", Action = AddBlocks }
            );
            _pointerActions.Add(new PointerAction { Label = "Move Blocks", Action = null });
            _pointerActions.Add(
                new PointerAction { Label = "Shift Blocks Up/Down", Action = ShiftBlockUpDown }
            );
            _pointerActions.Add(
                new PointerAction { Label = "Inc/Dec Stacks", Action = IncDecStack }
            );
            _pointerActions.Add(
                new PointerAction { Label = "Inc/Dec Blocks", Action = IncDecBlock }
            );
        }

        protected override void Update()
        {
            base.Update();

            if (!IsEnabled || _currentPointerAction.Action == null)
                return;

            if (_currentBlockIndex >= 0 && _currentBlockIndex < Puzzle.MaxStackHeight)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _currentPointerAction.Action.Invoke(
                        _currentStackIndex,
                        _currentBlockIndex,
                        InvertAction
                    );
                }
            }
        }

        private void DoNothing(int stackIndex, int blockIndex, bool invertAction) { }

        protected virtual void AddBlocks(int stackIndex, int blockIndex, bool invertAction)
        {
            if (stackIndex < 0 || stackIndex >= Puzzle.Stacks.Count)
                return;
            Stack stack = PuzzleMakerActions.Puzzle.Stacks[stackIndex];

            if (!invertAction)
            {
                switch (_currentBlockType)
                {
                    case BlockType.CatBlock:
                        PuzzleMakerActions.AddCatBlock(stack);
                        break;
                    case BlockType.PuzzleBlock:
                        PuzzleMakerActions.AddPuzzleBlock(stack);
                        break;
                    case BlockType.WildBlock:
                        PuzzleMakerActions.AddWildBlock(stack);
                        break;
                    case BlockType.SumBlock:
                        PuzzleMakerActions.AddSumBlock(stack, 1);
                        break;
                    case BlockType.TerrainBlock:
                        PuzzleMakerActions.AddTerrainBlock(stack);
                        break;
                    case BlockType.RemovalBlock:
                        PuzzleMakerActions.AddRemovalBlock(stack);
                        break;
                    case BlockType.PressureBlock:
                        PuzzleMakerActions.AddPressureBlock(stack);
                        break;
                }
            }
            else
            {
                PuzzleMakerActions.RemoveBlocksAt(stack, blockIndex);
            }
        }

        private void IncDecStack(int stackIndex, int blockIndex, bool invertAction)
        {
            if (stackIndex < 0 || stackIndex >= Puzzle.Stacks.Count)
                return;
            Stack stack = PuzzleMakerActions.Puzzle.Stacks[stackIndex];

            if (!invertAction)
            {
                PuzzleMakerActions.SumTopPuzzleBlocks(stack, 1);
            }
            else
            {
                PuzzleMakerActions.SumTopPuzzleBlocks(stack, -1);
            }
        }

        private void IncDecBlock(int stackIndex, int blockIndex, bool invertAction)
        {
            if (stackIndex < 0 || stackIndex >= Puzzle.Stacks.Count)
                return;
            Stack stack = PuzzleMakerActions.Puzzle.Stacks[stackIndex];

            if (blockIndex < 0 || blockIndex >= stack.Blocks.Count)
                return;
            Block block = stack.Blocks[blockIndex];

            if (!invertAction)
            {
                PuzzleMakerActions.SumBlock(stack, block, 1);
            }
            else
            {
                PuzzleMakerActions.SumBlock(stack, block, -1);
            }
        }

        private void ShiftBlockUpDown(int stackIndex, int blockIndex, bool invertAction)
        {
            if (stackIndex < 0 || stackIndex >= Puzzle.Stacks.Count)
                return;
            Stack stack = PuzzleMakerActions.Puzzle.Stacks[stackIndex];

            if (blockIndex < 0 || blockIndex >= stack.Blocks.Count)
                return;
            Block block = stack.Blocks[blockIndex];

            if (!invertAction)
            {
                PuzzleMakerActions.ShiftBlock(stack, block, 1);
            }
            else
            {
                PuzzleMakerActions.ShiftBlock(stack, block, -1);
            }
        }
    }
}
