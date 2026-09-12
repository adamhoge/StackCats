using System.Collections.Generic;

namespace Tofuwu.StackCats
{
    public interface IPresentData
    {
        string FromCatID { get; }
        Dictionary<Currency, int> Currency { get; }
        Dictionary<string, int> Items { get; }
    }
}