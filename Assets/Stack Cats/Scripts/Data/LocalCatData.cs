using System;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalCatData : ICatData
    {
        public string CatID { get { return _catId; } }

        public int SightingsCount { get { return _sightingsCount; } set { SetSightingsCount(value); } }

        public LocalCatData(string catID)
        {
            _catId = catID;
        }

        private readonly string _catId;
        private int _sightingsCount;

        private void SetSightingsCount(int value)
        {
            _sightingsCount = value;
        }
    }
}