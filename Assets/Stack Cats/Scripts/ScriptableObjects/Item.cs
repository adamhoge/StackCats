using UnityEngine;
using System;

namespace Tofuwu.StackCats
{
    public enum ItemRarity
    {
        VeryCommon,
        Common,
        Uncommon,
        Rare,
        VeryRare
    }

    [Serializable]
    public class ItemColorShift
    {
        [Range(-180, 180)]
        public float Hue = 0.0f;

        [Range(-1, 1)]
        public float Brightness = 0.0f;

        [Range(0, 2)]
        public float Contrast = 1.0f;

        [Range(0, 2)]
        public float Saturation = 1.0f;
    }

    [CreateAssetMenu(fileName = "Item", menuName = "Stack Cats/Items/Item")]
    public class Item : ScriptableObject, IIdentifiable
    {
        public string Name;
        public Sprite Icon;
        public ItemColorShift ColorShift;
        public bool IsUnique = true;
        public ItemRarity Rarity = ItemRarity.Common;

        [SerializeField]
        [HideInInspector]
        private string _id;

        public void RegenerateId()
        {
            _id = Guid.NewGuid().ToString();
        }

        public string GetId()
        {
            if (string.IsNullOrEmpty(_id)) { _id = Guid.NewGuid().ToString(); }

            return _id;
        }
    }
}