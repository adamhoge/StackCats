using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class Lifespan : MonoBehaviour
    {
        public float Duration = 1.0f;

        private float _aliveDuration;

        protected void Update()
        {
            _aliveDuration += Time.deltaTime;

            if (_aliveDuration >= Duration) Destroy(gameObject);
        }
    }
}