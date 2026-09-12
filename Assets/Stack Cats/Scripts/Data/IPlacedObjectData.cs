using System;

namespace Tofuwu.StackCats.Data
{
    public interface IPlacedObjectData
    {
        string PlaceableObjectItemId { get; set; }

        DateTime PlacedDateTime { get; set; }

        DateTime ActivatedDateTime { get; set; }
    }
}