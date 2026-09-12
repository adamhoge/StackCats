using UnityEngine;

namespace Tofuwu.StackCats
{
    public class ParticleEffect : PoolObject
    {
        public ParticleSystem ParticleSystem;

        protected virtual void Update()
        {
            if (isActiveAndEnabled)
            {
                if(!ParticleSystem.IsAlive(true)) Parent.ReturnInstance(this);
            }
        }
    }
}
