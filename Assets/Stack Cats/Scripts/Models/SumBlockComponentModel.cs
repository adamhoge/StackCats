using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [System.Serializable]
    public class SumBlockComponentModel
    {
        public int SumValue { get { return _sumValue; } set { _sumValue = value; } }

        [SerializeField]
        private int _sumValue;
    }
}