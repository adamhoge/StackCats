using System;

namespace Tofuwu.StackCats
{
    public delegate void CurtainDropMoveCountChanged(int moveCount);

    public class PuzzleMakerNightFlavoredActions : PuzzleMakerActions
    {
        public event CurtainDropMoveCountChanged onCurtainDropMoveCountChanged;

        public bool IsAutoLiftCurtainEnabled = true;

        public NightFlavoredPuzzle NightFlavoredPuzzle { get { return _nightFlavoredPuzzle; } }

        private NightFlavoredPuzzle _nightFlavoredPuzzle;
        private int _curtainDropMoveCount;

        // Set Curtain Drop Move Count
        public void SetCurtainDropMoveCount(int moveCount)
        {
            _curtainDropMoveCount = moveCount;
            _nightFlavoredPuzzle.CurtainTurnsRemaining = _nightFlavoredPuzzle.CurtainDropInterval - moveCount;
            if (onCurtainDropMoveCountChanged != null) onCurtainDropMoveCountChanged(moveCount);
        }

        // Set Curtain Drop Interval
        public void SetCurtainDropInterval(int interval)
        {
            _nightFlavoredPuzzle.CurtainDropInterval = interval;
        }

        // Lift Curtain
        public void LiftCurtain()
        {
            _nightFlavoredPuzzle.Curtain.Raise(false);
        }

        // Drop Curtain
        public void DropCurtain()
        {
            _nightFlavoredPuzzle.Curtain.Drop(false);
        }

        protected void Start()
        {
            _nightFlavoredPuzzle = (NightFlavoredPuzzle)Puzzle;
            _nightFlavoredPuzzle.onBlockMoveResolved += OnBlockMoveResolved;
        }

        private void OnBlockMoveResolved(Puzzle puzzle, Stack source, Block block, Stack destination)
        {
            if (IsAutoLiftCurtainEnabled)
            {
                SetCurtainDropMoveCount(_curtainDropMoveCount + 1);
                if (_curtainDropMoveCount >= _nightFlavoredPuzzle.CurtainDropInterval)
                {
                    LiftCurtain();
                    SetCurtainDropMoveCount(0);
                }
            }
        }
    }
}
