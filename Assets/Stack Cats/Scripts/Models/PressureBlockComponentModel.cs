using System;
using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [Serializable]
    public class PressureBlockComponentModel
    {
        public int BreakingPoint { get { return _numSupportableBlocks; } set { _numSupportableBlocks = value; } }

        [SerializeField]
        private int _numSupportableBlocks;
    }
}