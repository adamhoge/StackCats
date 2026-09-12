using UnityEngine;

namespace Tofuwu.StackCats
{
    public class CatBondingGuage : MonoBehaviour
    {
        public Cat Cat;
        public int BondingLevel;
        public FillTransform FillMask;

        public void IncrementBondingLevel()
        {
            ++BondingLevel;
            if (FillMask) FillMask.Amount = Mathf.Clamp(BondingLevel / (float)Cat.BondedAt, 0.0f, 1.0f);
        }

        protected void OnEnable()
        {
            if (FillMask)
            {
                FillMask.onFull += OnFull;
                FillMask.SetAmountImmediate(Mathf.Clamp(BondingLevel / (float)Cat.BondedAt, 0.0f, 1.0f));
            }
        }

        protected void OnDisable()
        {
            if (FillMask) FillMask.onFull -= OnFull;
        }

        private void OnFull()
        {
            // Bonded event!
        }
    }
}