using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void Focused(PuzzleMarker marker, bool isMovable);
    public delegate void Checked(PuzzleMarker source, PuzzleMarker destination, int numBlocks, bool isValid);
    public delegate void BlocksSelected(PuzzleMarker selection);
    public delegate void Cancelled(PuzzleMarker selection);

    public class PuzzleMarker
    {
        public Stack Stack { get { return _stack; } }

        public Block Block { get { return _block; } }

        private readonly Stack _stack;
        private readonly Block _block;

        public PuzzleMarker(Stack stack, Block block)
        {
            _stack = stack;
            _block = block;
        }
    }

    [RequireComponent(typeof(Puzzle))]
    public class PuzzleController : MonoBehaviour
    {
        /// <summary>
        /// Invoked whenever a block is focused within the puzzle.
        /// </summary>
        public event Focused onFocused;

        /// <summary>
        /// Invoked whenever a move is checked.
        /// </summary>
        public event Checked onChecked;

        /// <summary>
        /// Invoked whenever a block is selected.
        /// </summary>
        public event BlocksSelected onBlocksSelected;

        /// <summary>
        /// Invoked whenever a move is cancelled.
        /// </summary>
        public event Cancelled onCancelled;

        /// <summary>
        /// The puzzle being controlled.
        /// </summary>
        public Puzzle Puzzle { get { return _puzzle; } }

        /// <summary>
        /// The current block selection (if any).
        /// </summary>
        public PuzzleMarker Selection { get { return _selection; } }

        /// <summary>
        /// A readonly list of selected blocks.
        /// </summary>
        public ReadOnlyCollection<Block> SelectedBlocks { get { return _selectedBlocks.AsReadOnly(); } }

        protected Puzzle _puzzle;
        protected PuzzleMarker _focus;
        protected PuzzleMarker _selection;
        protected List<Block> _selectedBlocks = new List<Block>();

        private AudioManager _audioManager;

        /// <summary>
        /// Focus a specific stack/block location.
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="block"></param>
        public void FocusAt(Stack stack, Block block)
        {
            if (_focus != null && _focus.Stack == stack && _focus.Block == block) return;

            if (enabled && _puzzle.IsInteractable)
            {
                _focus = !stack || (_selection == null && !block) ? null : new PuzzleMarker(stack, block);
                bool isMovable = _puzzle.IsMovable(stack, block);
                OnFocused(_focus, isMovable);
                if (onFocused != null) onFocused(_focus, isMovable);

                if (!(_selection == null || _focus == null))
                {
                    bool isValid = _puzzle.CanMoveBlock(_selection.Stack, _selection.Block, _focus.Stack);
                    int numBlocks = _selection.Stack.Blocks.Count - _selection.Stack.Blocks.IndexOf(_selection.Block);
                    OnChecked(_focus, numBlocks, isValid);
                    if (onChecked != null) onChecked(_selection, _focus, numBlocks, isValid);
                }
            }
        }

        /// <summary>
        /// Select the block located at the marker.
        /// </summary>
        public virtual void Select()
        {
            if (enabled && _puzzle.IsInteractable)
            {
                if (_selection != null)
                {
                    if (_focus != null)
                    {
                        _puzzle.MoveBlock(_selection.Stack, _selection.Block, _focus.Stack);
                        Cancel();
                    }
                    else
                    {
                        Cancel();
                    }
                }
                else if (_focus != null)
                {
                    if (_puzzle.IsMovable(_focus.Stack, _focus.Block))
                    {
                        PuzzleMarker focusMarker = _focus;
                        FocusAt(null, null);
                        _selection = focusMarker;
                        _selectedBlocks = _selection.Stack.GetBlocksAt(_selection.Block);
                        _selection.Block.SelectBlock();
                        OnBlocksSelected(_selection);
                        if (onBlocksSelected != null) onBlocksSelected(_selection);
                    }
                    else
                    {
                        _focus.Block.KnockBlock();
                    }
                }
            }
        }

        /// <summary>
        /// Cancel selection.
        /// </summary>
        public virtual void Cancel()
        {
            if (_selection != null)
            {
                PuzzleMarker cancelledSelection = _selection;
                _selection = null;
                _selectedBlocks.Clear();
                FocusAt(null, null);
                OnCancelled(cancelledSelection);
                if (onCancelled != null) onCancelled(cancelledSelection);
            }
        }

        protected virtual void Awake()
        {
            _audioManager = GameManager.Instance.Audio;
            _puzzle = GetComponent<Puzzle>();
        }

        protected void OnDisable()
        {
            Cancel();
        }

        protected virtual void OnFocused(PuzzleMarker marker, bool isMovable) { }

        protected virtual void OnChecked(PuzzleMarker marker, int numBlocks, bool isValid) { }

        protected virtual void OnBlocksSelected(PuzzleMarker marker) { }

        protected virtual void OnCancelled(PuzzleMarker marker) { }
    }
}