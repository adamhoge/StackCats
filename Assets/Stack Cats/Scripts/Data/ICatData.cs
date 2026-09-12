namespace Tofuwu.StackCats.Data
{
    public interface ICatData
    {
        string CatID { get; }
        int SightingsCount { get; set; }
    }
}