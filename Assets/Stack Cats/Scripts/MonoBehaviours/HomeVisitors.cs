using Tofuwu.StackCats.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class HomeVisitors : MonoBehaviour
    {
        public int MaxNumVisitors = 4;
        public int MinVisitInSeconds = 120;
        public int MaxVisitInSeconds = 1200;
        public int MinMinigameBreakInSeconds = 300;
        public float AvatarScale = 1.0f;
        public float ZOffset = 1;
        public float YRotationInDegrees = 30;
        public Transform VisitorAreaRectTransform;
        public CatAvatar CatAvatarPrefab;

        public MinigameInformation VisitorsMinigame { get { return _visitorsMinigame; } }

        private IHomeData _homeData;
        private HomeManager _homeManager;
        private CatManager _catManager;
        private MinigameManager _minigameManager;
        private List<VisitorData> _visitorsData = new List<VisitorData>();
        private List<CatAvatar> _visitorAvatars = new List<CatAvatar>();
        private MinigameInformation _visitorsMinigame;

        protected void Awake()
        {
            GameManager gameManager = GameManager.Instance;

            _homeData = gameManager.Data.HomeData;
            _homeManager = gameManager.Home;
            _catManager = gameManager.Cats;
            _minigameManager = gameManager.Minigames;
        }

        protected void Start()
        {
            LoadVisitors();
            UpdateMinigame();
        }

        /// <summary>
        /// Get a list of cats visiting the player's house.
        /// </summary>
        /// <returns>The list of cats.</returns>
        public List<Cat> GetVisitors()
        {
            List<Cat> visitors = new List<Cat>();

            foreach (VisitorData visitorData in _visitorsData)
            {
                if (visitorData.CatId != "")
                {
                    visitors.Add(_catManager.CatCollection.GetById(visitorData.CatId));
                }
            }

            return visitors;
        }

        /// <summary>
        /// Pet visitor at the given index (if pettable)
        /// </summary>
        /// <param name="index">Index of the visitor to pet.</param>
        public void InteractWithVisitor(int index)
        {
            if (index < 0 || index >= _visitorsData.Count || string.IsNullOrEmpty(_visitorsData[index].CatId)) return;

            CatAvatar visitorAvatar = _visitorAvatars.First(a => a.Cat.GetId() == _visitorsData[index].CatId);

            if (!visitorAvatar) return;

            visitorAvatar.Animator.Chuffing();
        }

        private void LoadVisitors()
        {
            UnloadVisitors();

            _visitorsData = new List<VisitorData>(_homeData.GetVisitors());
            DateTime now = DateTime.Now;
            while (_visitorsData.Count > MaxNumVisitors)
            {
                _homeData.RemoveVisitor(_visitorsData[MaxNumVisitors].VisitorGuid);
                _visitorsData.RemoveAt(MaxNumVisitors);
            }

            List<VisitorData> visitorsToRemove = new List<VisitorData>();
            foreach (VisitorData visitor in _visitorsData)
            {
                if (visitor.LeaveDateTime <= now)
                {
                    _homeData.RemoveVisitor(visitor.VisitorGuid);
                    visitorsToRemove.Add(visitor);
                }
            }
            _visitorsData = _visitorsData.Except(visitorsToRemove).ToList();

            while (_visitorsData.Count < MaxNumVisitors)
            {
                VisitorData newVisitor = GetNewVisitor();
                _visitorsData.Add(newVisitor);
                _homeData.AddVisitor(newVisitor);
            }

            for (int i = 0; i < _visitorsData.Count; i++)
            {
                VisitorData visitor = _visitorsData[i];

                if (!string.IsNullOrEmpty(visitor.CatId))
                {
                    CatAvatar visitorAvatar = Instantiate(CatAvatarPrefab, VisitorAreaRectTransform);
                    visitorAvatar.transform.localPosition = GetVisitorLocalPositionByIndex(i);
                    visitorAvatar.transform.localRotation = Quaternion.Euler(GetVisitorLocalRotationEulerByIndex(i));
                    visitorAvatar.transform.localScale = Vector3.one * AvatarScale;
                    visitorAvatar.Cat = _catManager.CatCollection.GetById(visitor.CatId);
                    _visitorAvatars.Add(visitorAvatar);
                }
            }
        }

        private void UnloadVisitors()
        {
            _visitorsData.Clear();

            foreach (CatAvatar visitor in _visitorAvatars)
            {
                Destroy(visitor.gameObject);
            }
        }

        private void UpdateMinigame()
        {
            if ((DateTime.Now - _minigameManager.LastMinigamePlayedTime).TotalSeconds >= MinMinigameBreakInSeconds)
            {
                _visitorsMinigame = _minigameManager.GetMinigamesForCats(GetVisitors()).SelectRandom();
            }
            else
            {
                _visitorsMinigame = null;
            }
        }

        private VisitorData GetNewVisitor()
        {
            Cat newVisitor = GetRandomCatVisitor();

            return new VisitorData
            {
                CatId = newVisitor ? newVisitor.GetId() : "",
                LeaveDateTime = DateTime.Now.AddSeconds(UnityEngine.Random.Range(MinVisitInSeconds, MaxVisitInSeconds)),
                VisitorGuid = Guid.NewGuid()
            };
        }

        // TODO: Implement invitations, add odds of getting one of invited cats.
        private Cat GetRandomCatVisitor()
        {
            List<Cat> availableCats = _catManager.CatCollection.List.Where(c => _catManager.IsBonded(c) && !IsCatVisiting(c)).ToList();
            List<Cat> alreadyVisitingCats = new List<Cat>();
            foreach (VisitorData visitorData in _visitorsData)
            {
                alreadyVisitingCats.Add(_catManager.CatCollection.GetById(visitorData.CatId));
            }
            List<Cat> invitedCats = new List<Cat>(_homeManager.InvitedCats).Except(alreadyVisitingCats).ToList();

            float catRng = UnityEngine.Random.value;

            if (catRng < 0.2f)
            {
                return null;
            }
            else if (/*catRng < 0.6f || */invitedCats.Count == 0)
            {
                return availableCats.Count == 0 ? null : availableCats.SelectRandom();
            }
            else
            {
                Cat catVisitor = invitedCats.SelectRandom();
                invitedCats.Remove(catVisitor);
                return catVisitor;
            }
        }

        bool IsCatVisiting(Cat cat)
        {
            return _visitorsData.Exists(v => v.CatId == cat.GetId());
        }

        private Vector3 GetVisitorLocalPositionByIndex(int index)
        {
            float xPosition = -(MaxNumVisitors / 2.0f) + index + 0.5f;
            float zPosition = ZOffset * (0.5f - Math.Abs((-(MaxNumVisitors - 1) / 2.0f) + index));

            return new Vector3(xPosition * AvatarScale, 0.0f, zPosition * AvatarScale);
        }

        private Vector3 GetVisitorLocalRotationEulerByIndex(int index)
        {
            float yRotation = -(YRotationInDegrees / ((MaxNumVisitors + 1) / 2.0f)) + YRotationInDegrees * (index / (float)MaxNumVisitors);

            return new Vector3(0.0f, yRotation, 0.0f);
        }
    }
}