using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalPlacedObjectData : IPlacedObjectData
    {
        public string PlaceableObjectItemId { get; set; }

        public DateTime PlacedDateTime { get; set; }
        
        public DateTime ActivatedDateTime { get; set; }
    }
}