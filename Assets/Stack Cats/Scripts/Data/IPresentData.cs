using System.Collections.Generic;

namespace RockhopperGames.StackCats
{
    public interface IPresentData
    {
        string FromCatID { get; }
        Dictionary<Currency, int> Currency { get; }
        Dictionary<string, int> Items { get; }
    }
}
