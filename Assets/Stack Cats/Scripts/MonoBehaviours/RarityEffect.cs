using UnityEngine;

namespace Tofuwu.StackCats
{
    public class RarityEffect : MonoBehaviour
    {
        public Cat Cat;
        public ParticleSystem MainParticleSystem;
        public ParticleSystem RareParticleSystem;
        public Color CommonParticleColor;
        public Color UncommonParticleColor;
        public Color RareParticleColor;

        protected void Start()
        {
            Color mainColor = Color.white;
            bool isRareParticleEffectEnabled = false;
            switch (Cat.Rarity)
            {
                case Rarity.Common:
                    mainColor = CommonParticleColor;
                    break;
                case Rarity.Uncommon:
                    mainColor = UncommonParticleColor;
                    break;
                case Rarity.Rare:
                    mainColor = RareParticleColor;
                    isRareParticleEffectEnabled = true;
                    break;
            }
            ParticleSystem.MainModule main = MainParticleSystem.main;
            main.startColor = mainColor;
            RareParticleSystem.gameObject.SetActive(isRareParticleEffectEnabled);
        }
    }
}