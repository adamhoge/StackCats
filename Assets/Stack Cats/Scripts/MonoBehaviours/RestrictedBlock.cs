using UnityEngine;

namespace Tofuwu.StackCats
{
    public class RestrictedBlock : BlockComponent
    {
        public override bool IsMovable
        {
            get { return false; }
        }

        public override bool IsPlaceableOn(Block block)
        {
            return false;
        }
    }
}
