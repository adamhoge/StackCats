using System;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleDemoUI : MonoBehaviour
    {
        public PointerGestureUI PointerGesture;
        public PuzzleDemo PuzzleDemo;
        public Vector3 StartingPosition = Vector3.zero;
        public float DragYOffset;

        protected void Start()
        {
            PuzzleDemo.onShowingPuzzle += OnShowingPuzzle;
            PuzzleDemo.onHoveringBlocks += OnHoveringBlocks;
            PuzzleDemo.onSelectedBlocks += OnSelectedBlocks;
            PuzzleDemo.onDraggingBlocks += OnDraggingBlocks;
            PuzzleDemo.onReleasedBlocks += OnReleasedBlocks;
            PuzzleDemo.onDemoComplete += OnDemoComplete;

            PointerGesture.SetPressed(false, false);
            PointerGesture.SetVisible(false, false);
            PointerGesture.SetPosition(StartingPosition, false);
        }

        private void OnShowingPuzzle()
        {
            PointerGesture.SetPressed(false, false);
            PointerGesture.SetPosition(StartingPosition, false);
        }

        private void OnHoveringBlocks(Vector3 hoverPosition)
        {
            PointerGesture.SetVisible(true);
            PointerGesture.SetPosition(hoverPosition + Vector3.up * DragYOffset);
        }

        private void OnSelectedBlocks()
        {
            PointerGesture.SetPressed(true);
        }

        private void OnDraggingBlocks(Vector3 dragPosition)
        {
            PointerGesture.transform.position = dragPosition + Vector3.up * DragYOffset;
        }

        private void OnReleasedBlocks()
        {
            PointerGesture.SetPressed(false);
            PointerGesture.SetPosition(PointerGesture.transform.position + Vector3.up * 0.25f);
        }

        private void OnDemoComplete()
        {
            PointerGesture.SetVisible(false);
        }
    }
}