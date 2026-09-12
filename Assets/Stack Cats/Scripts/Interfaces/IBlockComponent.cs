namespace Tofuwu.StackCats
{
    public interface IBlockComponent
    {
        bool IsPlaceableOn(Block otherBlock);
        void OnMove();
    }
}
