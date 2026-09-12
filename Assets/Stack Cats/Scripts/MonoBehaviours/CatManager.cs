using System.Linq;
using UnityEngine;
using Tofuwu.StackCats.Data;

namespace Tofuwu.StackCats
{
    public class CatManager : MonoBehaviour
    {
        /// <summary>
        /// The cat collection associated with the cat manager.
        /// </summary>
        public CatCollection CatCollection;

        private ICatCollectionData _data;

        /// <summary>
        /// Add a cat sighting.
        /// </summary>
        /// <param name="cat">The cat seen.</param>
        /// <returns>The reward (if any) received for the sighting.</returns>
        public CatReward AddCatSighting(Cat cat)
        {
            string catId = cat.GetId();

            _data.AddCatSighting(catId);
            int sightingCount = _data.GetSightingsCount(catId);
            return cat.Rewards.FirstOrDefault(r => r.AtSightingCount == sightingCount);
        }

        /// <summary>
        /// Check whether or not a cat has been seen.
        /// </summary>
        /// <param name="cat">The cat to check.</param>
        /// <returns>A flag indicating whether or not the cat has been seen.</returns>
        public bool WasCatSeen(Cat cat)
        {
            return _data.HasCat(cat.GetId());
        }

        /// <summary>
        /// Get the number of times the specified cat has been seen.
        /// </summary>
        /// <param name="cat">The cat to check.</param>
        /// <returns>The number of times the cat has been seen.</returns>
        public int GetSightingsCount(Cat cat)
        {
            return _data.GetSightingsCount(cat.GetId());
        }

        /// <summary>
        /// Check whether or not a cat has established a bond.
        /// </summary>
        /// <param name="cat">The cat to check.</param>
        /// <returns>A flag indicating whether or not the cat is bonded.</returns>
        public bool IsBonded(Cat cat)
        {
            return _data.GetSightingsCount(cat.GetId()) >= cat.BondedAt;
        }

        protected void Awake()
        {
            _data = GameManager.Instance.Data.CatCollectionData;
        }
    }
}