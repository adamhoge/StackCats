namespace Tofuwu.StackCats.Data
{
    public interface ICatCollectionData
    {
        bool HasCat(string catId);
        int GetSightingsCount(string catId);
        void AddCatSighting(string catId);
    }
}