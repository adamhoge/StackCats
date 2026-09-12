using System;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void CurtainTurnsChanged(int curtainTurnsRemaining);

    public class NightFlavoredPuzzle : FarmFlavoredPuzzle
    {
        /// <summary>
        /// Invoked whenever the turns remaining changes for curtain drops.
        /// </summary>
        public event CurtainTurnsChanged onCurtainTurnsChanged;

        public PuzzleCurtain Curtain;
        public int CurtainDropInterval;

        /// <summary>
        /// The turns remaining until the curtain drops.
        /// </summary>
        public int CurtainTurnsRemaining { get { return _curtainTurnsRemaining; } set { SetCurtainTurnsRemaining(value); } }

        private void SetCurtainTurnsRemaining(int value)
        {
            if (value == _curtainTurnsRemaining) return;

            _curtainTurnsRemaining = value;

            Curtain.IsShaking = _curtainTurnsRemaining == 1;
        }

        public override int MaxMovableStackHeight { get { return GetMaxMovableStackHeight(); } }

        private int _curtainTurnsRemaining;

        public override bool CanMoveBlock(Stack source, Block block, Stack destination)
        {
            if (!destination || destination == source) return false;

            int numBlocks = source.Blocks.Count - source.Blocks.IndexOf(block);
            int curtainHeight = Curtain.Height;
            if (IsEditMode && _curtainTurnsRemaining == 1) --curtainHeight;
            bool canFit = destination.Blocks.Count + numBlocks <= destination.MaxBlocks - curtainHeight;
            return !(!IsMovable(source, block) || !IsPlaceable(block, destination.TopBlock) || !canFit);
        }

        protected new void Awake()
        {
            base.Awake();

            Curtain.Puzzle = this;
        }

        protected new void Start()
        {
            base.Start();

            _curtainTurnsRemaining = CurtainDropInterval;
        }

        protected override void OnAfterBlockMoved(Stack source, Block block, Stack destination)
        {
            base.OnAfterBlockMoved(source, block, destination);

            if (!IsEditMode && !_isComplete)
            {
                CurtainTurnsRemaining = CurtainDropInterval - _numMovesMade % CurtainDropInterval;
                if (onCurtainTurnsChanged != null) onCurtainTurnsChanged(_curtainTurnsRemaining);

                if (CurtainTurnsRemaining == CurtainDropInterval)
                {
                    Curtain.Drop();
                }

                if (Curtain.GetCoveredBlocks().Count > 0)
                {
                    CompletePuzzle(PuzzleCompletionType.PuzzleFailed);
                }
            }
        }

        private int GetMaxMovableStackHeight()
        {
            int curtainHeight = Curtain.Height;
            if (IsEditMode && _curtainTurnsRemaining == 1) --curtainHeight;
            return MaxStackHeight - curtainHeight;
        }
    }
}
