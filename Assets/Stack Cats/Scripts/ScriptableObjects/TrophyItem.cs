using UnityEngine;

namespace Tofuwu.StackCats
{
    public enum TrophyMaterial
    {
        Bronze,
        Silver,
        Gold,
        Platinum,
        Diamond
    }

    [CreateAssetMenu(fileName = "Trophy Item", menuName = "Stack Cats/Items/Trophy Item")]
    public class TrophyItem : Item
    {
        public TrophyMaterial Material;
        public GameObject TrophyPrefab;
    }
}
