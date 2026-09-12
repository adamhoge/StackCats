using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class StackModel
    {
        public List<BlockModel> Blocks { get { return _blocks; } }

        [SerializeField]
        private List<BlockModel> _blocks = new List<BlockModel>();
    }
}