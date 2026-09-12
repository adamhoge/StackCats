using UnityEngine;

namespace Tofuwu.StackCats
{
    [RequireComponent(typeof(CatAvatar))]
    public abstract class CatAvatarMannerisms : MonoBehaviour
    {
        public float CheckNewBehaviourMinInterval = 1.0f;
        public float CheckNewBehaviourMaxInterval = 3.0f;

        protected CatAvatar _catAvatar;

        private float _lastCheckNewBehaviourTime;

        public abstract void CheckNewBehaviour();

        public abstract void SetMannerisms(Cat cat);

        protected virtual void Awake()
        {
            _catAvatar = GetComponent<CatAvatar>();
        }

        protected virtual void Update()
        {
            if (Time.time >= _lastCheckNewBehaviourTime + Random.Range(CheckNewBehaviourMinInterval, CheckNewBehaviourMaxInterval))
            {
                CheckNewBehaviour();
                _lastCheckNewBehaviourTime = Time.time;
            }
        }
    }
}