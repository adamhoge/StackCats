using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class DemoMove
    {
        public int FromStackIndex;
        public int FromBlockIndex;
        public int ToStackIndex;
        public int ToBlockIndex;
    }

    public delegate void ShowingPuzzle();
    public delegate void SelectedBlocks();
    public delegate void HoveringBlocks(Vector3 hoverPosition);
    public delegate void DraggingBlocks(Vector3 dragPosition);
    public delegate void ReleasedBlocks();
    public delegate void DemoComplete();

    [RequireComponent(typeof(PuzzleBlockDragPositioner))]
    public class PuzzleDemo : MonoBehaviour
    {
        private enum DemoState
        {
            NotStarted,
            ShowingPuzzle,
            ReadyingMove,
            BeginningMove,
            Moving,
            EndingMove,
            Complete,
            Restarting
        }

        public event ShowingPuzzle onShowingPuzzle;
        public event HoveringBlocks onHoveringBlocks;
        public event SelectedBlocks onSelectedBlocks;
        public event DraggingBlocks onDraggingBlocks;
        public event ReleasedBlocks onReleasedBlocks;
        public event DemoComplete onDemoComplete;

        public PuzzleLoader PuzzleLoader;
        public PuzzleArea PuzzleArea;
        public float ShowPuzzleDuration = 1.0f;
        public float ReadyMoveDuration = 0.5f;
        public float BeginMoveDuration = 0.5f;
        public float MoveDuration = 0.5f;
        public float EndMoveDuration = 0.5f;
        public float CompletePuzzleDuration = 1.0f;
        [TextArea]
        public string PuzzleJsonData;
        public List<DemoMove> DemoMoves = new List<DemoMove>();

        private PuzzleBlockDragPositioner _dragPositioner;
        private DemoState _demoState;
        private float _demoStateTimeElapsed;
        private Queue<DemoMove> _demoMovesRemaining;

        protected void Awake()
        {
            _dragPositioner = GetComponent<PuzzleBlockDragPositioner>();
        }

        protected void Start()
        {
            ShowPuzzle();
        }

        protected void Update()
        {
            _demoStateTimeElapsed += Time.deltaTime;

            switch (_demoState)
            {
                case DemoState.ShowingPuzzle:
                    if (_demoStateTimeElapsed >= ShowPuzzleDuration)
                    {
                        PerformNextMove();
                    }
                    break;
                case DemoState.ReadyingMove:
                    if (_demoStateTimeElapsed >= ReadyMoveDuration)
                    {
                        BeginMove();
                    }
                    break;
                case DemoState.BeginningMove:
                    if (_demoStateTimeElapsed >= BeginMoveDuration)
                    {
                        Move();
                    }
                    break;
                case DemoState.Moving:
                    if (_demoStateTimeElapsed >= MoveDuration)
                    {
                        EndMove();
                    }
                    break;
                case DemoState.EndingMove:
                    if (_demoStateTimeElapsed >= EndMoveDuration)
                    {
                        PerformNextMove();
                    }
                    break;
                case DemoState.Complete:
                    if (_demoStateTimeElapsed >= CompletePuzzleDuration)
                    {
                        RestartDemo();
                    }
                    break;
                case DemoState.Restarting:
                    ShowPuzzle();
                    break;
            }
        }

        private void ShowPuzzle()
        {
            PuzzleLoader.LoadPuzzle(PuzzleJsonData, PuzzleArea, false, false, false);
            _demoMovesRemaining = new Queue<DemoMove>(DemoMoves);

            if (onShowingPuzzle != null) onShowingPuzzle();
            ChangeDemoState(DemoState.ShowingPuzzle);
        }

        private void PerformNextMove()
        {
            if (_demoMovesRemaining.Count == 0)
            {
                CompleteDemo();
                return;
            }

            DemoMove currentDemoMove = _demoMovesRemaining.Peek();
            HoverBlocks(currentDemoMove);
            if (onHoveringBlocks != null) onHoveringBlocks(_dragPositioner.DragPosition);
            ChangeDemoState(DemoState.ReadyingMove);
        }

        private void BeginMove()
        {
            SelectBlocks(_demoMovesRemaining.Peek());
            if (onSelectedBlocks != null) onSelectedBlocks();
            ChangeDemoState(DemoState.BeginningMove);
        }

        private void Move()
        {
            DemoMove demoMove = _demoMovesRemaining.Peek();
            Stack toStack = PuzzleLoader.Puzzle.Stacks[demoMove.ToStackIndex];
            Vector3 finalDragPosition = toStack.GetBlockLocalPosition(demoMove.ToBlockIndex) + Vector3.up * 0.5f;
            finalDragPosition.Scale(toStack.transform.lossyScale);
            finalDragPosition = finalDragPosition + toStack.transform.position + Vector3.up * 0.1f;
            LeanTween
                .value(0.0f, 1.0f, MoveDuration)
                .setEase(LeanTweenType.easeInSine)
                .setOnUpdate((float stateDelta) =>
                {
                    UpdateDragPosition(stateDelta, _dragPositioner.DragPosition, finalDragPosition);
                });
            ChangeDemoState(DemoState.Moving);
        }

        private void EndMove()
        {
            DemoMove demoMove = _demoMovesRemaining.Peek();

            _dragPositioner.ReleaseBlocks();
            if (onReleasedBlocks != null) onReleasedBlocks();

            Stack fromStack = PuzzleLoader.Puzzle.Stacks[demoMove.FromStackIndex];
            Block fromBlock = fromStack.Blocks[demoMove.FromBlockIndex];
            Stack toStack = PuzzleLoader.Puzzle.Stacks[demoMove.ToStackIndex];
            PuzzleLoader.Puzzle.MoveBlock(fromStack, fromBlock, toStack);

            _demoMovesRemaining.Dequeue();
            ChangeDemoState(DemoState.EndingMove);
        }

        private void CompleteDemo()
        {
            if (onDemoComplete != null) onDemoComplete();
            ChangeDemoState(DemoState.Complete);
        }

        private void RestartDemo()
        {
            ChangeDemoState(DemoState.Restarting);
        }

        private void HoverBlocks(DemoMove demoMove)
        {
            Stack fromStack = PuzzleLoader.Puzzle.Stacks[demoMove.FromStackIndex];
            Block fromBlock = fromStack.Blocks[demoMove.FromBlockIndex];
            _dragPositioner.DragPosition = fromBlock.transform.position + (Vector3.up * 0.6f);
        }

        private void SelectBlocks(DemoMove demoMove)
        {
            Stack fromStack = PuzzleLoader.Puzzle.Stacks[demoMove.FromStackIndex];
            Block fromBlock = fromStack.Blocks[demoMove.FromBlockIndex];
            _dragPositioner.SelectBlocks(new PuzzleMarker(fromStack, fromBlock));
        }

        private void UpdateDragPosition(float stateDelta, Vector3 fromPosition, Vector3 toPosition)
        {
            _dragPositioner.DragPosition = fromPosition + (toPosition - fromPosition) * stateDelta;
            if (onDraggingBlocks != null) onDraggingBlocks(_dragPositioner.DragPosition);
        }

        private void ChangeDemoState(DemoState demoState)
        {
            _demoState = demoState;
            _demoStateTimeElapsed = 0.0f;
        }
    }
}