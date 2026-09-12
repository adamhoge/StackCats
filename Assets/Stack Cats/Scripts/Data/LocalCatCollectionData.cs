using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalCatCollectionData : LocalData<LocalCatCollectionData>, ICatCollectionData
    {
        private readonly string _dataPath;
        private readonly Dictionary<string, ICatData> _catsSeen = new Dictionary<string, ICatData>();

        public LocalCatCollectionData(string dataPath) : base(dataPath) { }

        /// <summary>
        /// Check if a cat was seen.
        /// </summary>
        /// <param name="catID">The ID of the seen cat.</param>
        /// <returns>A flag indicating whether or not the puzzle is locked.</returns>
        public bool HasCat(string catID)
        {
            return _catsSeen.ContainsKey(catID);
        }

        /// <summary>
        /// Get the number of times a cat has been seen.
        /// </summary>
        /// <param name="catID">The ID of the cat.</param>
        /// <returns>The number of times the specified cat has been seen.</returns>
        public int GetSightingsCount(string catID)
        {
            return _catsSeen.ContainsKey(catID) ? _catsSeen[catID].SightingsCount : 0;
        }

        /// <summary>
        /// Add a cat to the list of seen cats.
        /// </summary>
        /// <param name="catID">The ID of the seen cat.</param>
        public void AddCatSighting(string catID)
        {
            if (!_catsSeen.ContainsKey(catID))
            {
                _catsSeen.Add(catID, new LocalCatData(catID));
            }

            _catsSeen[catID].SightingsCount++;

            Save();
        }
    }
}