using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Gift Item", menuName = "Stack Cats/Items/Gift Item")]
    public class GiftItem : Item
    {
        public int BondingBonus = 1;
    }
}