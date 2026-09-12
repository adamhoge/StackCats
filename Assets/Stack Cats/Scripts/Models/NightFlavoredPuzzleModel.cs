using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class NightFlavoredPuzzleModel : PuzzleModel
    {
        public int CurtainDropInterval { get { return _curtainDropInterval; } set { _curtainDropInterval = value; } }

        public int CurtainHeight { get { return _curtainHeight; } set { _curtainHeight = value; } }

        [SerializeField]
        private int _curtainDropInterval;

        [SerializeField]
        private int _curtainHeight;
    }
}