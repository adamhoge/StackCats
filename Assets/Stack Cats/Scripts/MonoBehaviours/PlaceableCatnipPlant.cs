using System;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class PlaceableCatnipPlant : PlaceableObject
    {
        private enum GrowthStage
        {
            Sprout,
            Juvenile,
            Mature
        }

        public SpriteRenderer PlantSpriteRenderer;
        public Sprite SproutSprite;
        public Sprite JuvenileSprite;
        public Sprite MatureSprite;
        public GiftItem CatnipGiftItem;
        public AudioEvent HarvestSoundEffect;

        private GameManager _gameManager;
        private const int JUVENILE_AT_SECONDS = 42750;
        private const int MATURE_AT_SECONDS = 85500;
        private GrowthStage _growthStage;

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
        }

        protected override void Start()
        {
            base.Start();

            LeanTween.scaleY(PlantSpriteRenderer.gameObject, 1.05f, 1.0f).setEase(LeanTweenType.easeOutSine).setLoopPingPong();

            int ageInMinutes = GetLastActivationInSeconds();
            if (ageInMinutes >= MATURE_AT_SECONDS)
            {
                SetGrowthStage(GrowthStage.Mature);
            }
            else if (ageInMinutes >= JUVENILE_AT_SECONDS)
            {
                SetGrowthStage(GrowthStage.Juvenile);
            }
            else
            {
                SetGrowthStage(GrowthStage.Sprout);
            }
        }

        protected void Update()
        {
            switch (_growthStage)
            {
                case GrowthStage.Sprout:
                    if(GetLastActivationInSeconds() >= JUVENILE_AT_SECONDS)
                    {
                        SetGrowthStage(GrowthStage.Juvenile);
                    }
                    break;
                case GrowthStage.Juvenile:
                    if (GetLastActivationInSeconds() >= MATURE_AT_SECONDS)
                    {
                        SetGrowthStage(GrowthStage.Mature);
                    }
                    break;
                case GrowthStage.Mature:
                    break;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            _gameManager.Audio.PlaySoundEffect(HarvestSoundEffect);
            _gameManager.Stuff.AddItem(CatnipGiftItem, 3);
            SetGrowthStage(GrowthStage.Sprout);
        }

        private void SetGrowthStage(GrowthStage growthStage)
        {
            switch (growthStage)
            {
                case GrowthStage.Sprout:
                    PlantSpriteRenderer.sprite = SproutSprite;
                    break;
                case GrowthStage.Juvenile:
                    PlantSpriteRenderer.sprite = JuvenileSprite;
                    break;
                case GrowthStage.Mature:
                    PlantSpriteRenderer.sprite = MatureSprite;
                    break;
            }

            _growthStage = growthStage;
        }
    }
}