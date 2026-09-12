using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class CatBlockComponentModel
    {
        public string CatBlockId { get { return _catBlockId; } set { _catBlockId = value; } }

        public string CatId { get { return _catId; } set { _catId = value; } }

        public int Yarn { get { return _yarn; } set { _yarn = value; } }

        [SerializeField]
        private string _catBlockId;

        [SerializeField]
        private string _catId;

        [SerializeField]
        private int _yarn;
    }
}