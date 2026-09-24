using System;

namespace RockhopperGames.StackCats.Data
{
    public interface IPlacedObjectData
    {
        string PlaceableObjectItemId { get; set; }

        DateTime PlacedDateTime { get; set; }

        DateTime ActivatedDateTime { get; set; }
    }
}
