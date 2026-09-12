using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tofuwu.StackCats.Data;

namespace Tofuwu.StackCats
{
    /// <summary>
    /// Manages the player's home, including cat visitors, invited cats, decor, etc.
    /// </summary>
    public class HomeManager : MonoBehaviour
    {
        /// <summary>
        /// The maximum number of cats that can be invited into the player's home.
        /// </summary>
        public int MaxInvitedCats = 10;

        /// <summary>
        /// List of cats invited into the player's home.
        /// </summary>
        public List<Cat> InvitedCats { get { return _invitedCats; } }

        /// <summary>
        /// Flag indicating whether or not more cats can be invited into the player's home.
        /// </summary>
        public bool CanInviteCats { get { return _invitedCats.Count < MaxInvitedCats; } }

        private IHomeData _homeData;
        private StuffManager _stuffManager;
        private CatManager _catManager;
        private List<Cat> _invitedCats;

        public void Awake()
        {
            GameManager gameManager = GameManager.Instance;
            _homeData = gameManager.Data.HomeData;
            _stuffManager = gameManager.Stuff;
            _catManager = gameManager.Cats;

            LoadInvitedCats();
        }

        /// <summary>
        /// Invite a cat into the player's home.
        /// </summary>
        /// <param name="cat">The cat to invite.</param>
        public void InviteCat(Cat cat)
        {
            if (!CanInviteCats)
            {
                Debug.LogError("Tried to invite more than max invitable cats.");
                return;
            }

            _homeData.AddInvitedCatId(cat.GetId());
            _invitedCats.Add(cat);
        }

        /// <summary>
        /// Uninvite a cat from the player's home.
        /// </summary>
        /// <param name="cat">The cat to uninvite.</param>
        public void UninviteCat(Cat cat)
        {
            _homeData.RemoveInvitedCatId(cat.GetId());
            _invitedCats.Remove(cat);
        }

        private void LoadInvitedCats()
        {
            List<string> invitedCatIds = _homeData.GetInvitedCatIds();

            _invitedCats = new List<Cat>();
            foreach (string catId in invitedCatIds)
            {
                _invitedCats.Add(_catManager.CatCollection.GetById(catId));
            }
        }
    }
}