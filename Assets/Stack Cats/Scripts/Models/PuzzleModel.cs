using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class PuzzleModel
    {
        public List<StackModel> Stacks { get { return _stacks; } }

        public int MaxStackHeight { get { return _maxStackHeight; } set { _maxStackHeight = value; } }

        public int NumMovesMade { get { return _numMovesMade; } set { _numMovesMade = value; } }

        public bool IsSpecial { get { return _isSpecial; } set { _isSpecial = value; } }

        [SerializeField]
        private List<StackModel> _stacks = new List<StackModel>();

        [SerializeField]
        private int _maxStackHeight = 10;

        [SerializeField]
        private int _numMovesMade = 0;

        [SerializeField]
        private bool _isSpecial = false;
    }
}