using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class PuzzleBlockComponentModel
    {
        public int PrimaryNumber { get { return _primaryNumber; } set { _primaryNumber = value; } }

        public int SecondaryNumber { get { return _secondaryNumber; } set { _secondaryNumber = value; } }

        [SerializeField]
        private int _primaryNumber;

        [SerializeField]
        private int _secondaryNumber;
    }
}