using UnityEngine;
using System.Collections.Generic;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class DesertFlavoredPuzzleModel : PuzzleModel
    {
        public List<int> StackHeightRequirements { get { return _stackHeightRequirements; } set { _stackHeightRequirements = value; } }

        [SerializeField]
        private List<int> _stackHeightRequirements;
    }
}