using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class PoolObject : MonoBehaviour
    {
        public ObjectPooler Parent = null;
        public bool IsBorrowed;

        public virtual void ParamStart() { }
    }
}