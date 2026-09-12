using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class ObjectPooler : MonoBehaviour
    {
        /// <summary>
        /// The prefab used for the object pool.
        /// </summary>
        public PoolObject Prefab;

        /// <summary>
        /// The initial size of the object pool.
        /// </summary>
        public int InitialSize;

        /// <summary>
        /// Flag for whether or not the pool should automatically expand.
        /// </summary>
        public bool IsExandable = true;

        private readonly List<PoolObject> _objectPool = new List<PoolObject>();

        /// <summary>
        /// Borrow an instance of the pool prefab.
        /// </summary>
        /// <returns>An instance of the pool prefab.</returns>
        public PoolObject BorrowInstance()
        {
            foreach (PoolObject poolObject in _objectPool)
            {
                if (!poolObject.IsBorrowed)
                {
                    return LendObject(poolObject);
                }
            }

            if (IsExandable)
            {
                return LendObject(AddObjectToPool());
            }

            return null;
        }

        /// <summary>
        /// Return an instance of the pool prefab.
        /// </summary>
        public void ReturnInstance(PoolObject poolObject)
        {
            poolObject.IsBorrowed = false;
            poolObject.gameObject.SetActive(false);
            poolObject.transform.SetParent(transform);
        }

        protected void Awake()
        {
            for (int i = 0; i < InitialSize; i++)
            {
                AddObjectToPool();
            }
        }

        private PoolObject AddObjectToPool()
        {
            PoolObject newObject = Instantiate(Prefab, transform);
            newObject.name = Prefab.name + " " + _objectPool.Count + " (Pool Object)";
            newObject.gameObject.SetActive(false);
            newObject.Parent = this;
            _objectPool.Add(newObject);
            return newObject;
        }

        private PoolObject LendObject(PoolObject poolObject)
        {
            poolObject.IsBorrowed = true;
            poolObject.gameObject.SetActive(true);
            return poolObject;
        }
    }
}