using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class CatReward
    {
        public int AtSightingCount = 1;
        public CurrencyAmountDictionary Currency;
        public ItemAmountDictionary Items;
    }

    public enum CatPersonality
    {
        Timid,
        Rambunctious,
        Curious,
        Aloof,
        Playful,
        Sleepy,
        MildMannered,
        Lazy,
        Hungry,
        Coy,
        Mysterious,
        Territorial
    }

    [CreateAssetMenu(fileName = "Cat", menuName = "Stack Cats/Cat")]
    public class Cat : ScriptableObject, IIdentifiable
    {
        public string Name;
        public int Number;
        public CatPersonality Personality;
        public Sprite Portrait;
        public CatAvatarSettings AvatarSettings;
        public CatAccessory HeadAccessory;
        public AudioEvent Meow;
        public Rarity Rarity;
        public int BondedAt = 10;
        public List<CatReward> Rewards = new List<CatReward>();

        [SerializeField]
        [HideInInspector]
        private string _id;

        public string GetId()
        {
            if (string.IsNullOrEmpty(_id)) { _id = Guid.NewGuid().ToString(); }

            return _id;
        }
    }
}