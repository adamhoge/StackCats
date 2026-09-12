using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class GalaxyFlavoredPuzzleModel : PuzzleModel
    {
        public int RaiseStacksInterval { get { return _raiseStacksInterval; } set { _raiseStacksInterval = value; } }

        [SerializeField]
        private int _raiseStacksInterval;
    }
}