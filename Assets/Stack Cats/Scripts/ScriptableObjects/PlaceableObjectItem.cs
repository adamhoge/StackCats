using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Placeable Object Item", menuName = "Stack Cats/Items/Decor/Placeable Object Item")]
    public class PlaceableObjectItem : Item
    {
        public PlaceableObject PlaceableObjectPrefab;
    }
}