using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class JigsawBlockComponentModel
    {
        public string JigsawPuzzleObjectId { get { return _jigsawPuzzleObjectId; } set { _jigsawPuzzleObjectId = value; } }
        public int JigsawIndex { get { return _jigsawIndex; } set { _jigsawIndex = value; } }

        [SerializeField]
        private int _jigsawIndex;

        [SerializeField]
        private string _jigsawPuzzleObjectId;
    }
}