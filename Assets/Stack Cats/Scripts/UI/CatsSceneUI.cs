using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class CatsSceneUI : MonoBehaviour
    {
        public CatsScene CatsScene;
        public RectTransform PortraitsRect;
        public CatPortraitButton CatPortraitButtonPrefab;
        public OverlayScreenManager OverlayScreenManager;
        public CatInfoOverlayScreen CatInfoOverlayScreen;
        public TextMeshProUGUI CatsSeenCount;
        public TextMeshProUGUI CatsBondedCount;

        private CatManager _catManager;
        private List<Cat> _cats;
        private Cat _currentCatDisplayed;
        private List<CatPortraitButton> _catPortraitButtons = new List<CatPortraitButton>();

        public void SortBy(Dropdown dropdown)
        {
            switch (dropdown.value)
            {
                case 0: DisplayListByNumber(); break;
                case 1: DisplayListByName(); break;
                case 2: DisplayListByRarity(); break;
            }
        }

        public void DisplayCatInfo(Cat cat)
        {
            if (!_currentCatDisplayed)
            {
                OverlayScreenManager.EnqueueScreen(CatInfoOverlayScreen);
            }

            _currentCatDisplayed = cat;
            CatInfoOverlayScreen.Cat = cat;
        }

        public void DisplayPreviousCatInfo()
        {
            DisplayCatInfo(GetPreviousCat(_currentCatDisplayed));
        }

        public void DisplayNextCatInfo()
        {
            DisplayCatInfo(GetNextCat(_currentCatDisplayed));
        }

        public Cat GetPreviousCat(Cat fromCat)
        {
            List<Cat> catsSeen = _cats.Where(c => _catManager.WasCatSeen(c)).ToList();
            int previousCatIndex = catsSeen.IndexOf(_currentCatDisplayed) - 1;
            if (previousCatIndex < 0) previousCatIndex = catsSeen.Count - 1;
            return catsSeen[previousCatIndex];
        }

        public Cat GetNextCat(Cat fromCat)
        {
            List<Cat> catsSeen = _cats.Where(c => _catManager.WasCatSeen(c)).ToList();
            int nextCatIndex = catsSeen.IndexOf(fromCat) + 1;
            if (nextCatIndex >= catsSeen.Count) nextCatIndex = 0;
            return (catsSeen[nextCatIndex]);
        }

        protected void Awake()
        {
            _catManager = GameManager.Instance.Cats;
            _cats = new List<Cat>(GameManager.Instance.Cats.CatCollection.List);
        }

        protected void OnEnable()
        {
            OverlayScreenManager.OnHidden.AddListener(OnOverlayScreenHidden);
        }

        protected void OnDisable()
        {
            OverlayScreenManager.OnHidden.RemoveListener(OnOverlayScreenHidden);
        }

        protected void Start()
        {
            if (!PortraitsRect || !CatPortraitButtonPrefab) return;

            int catsCount = _cats.Count;
            int catsSeenCount = _cats.Count(c => _catManager.WasCatSeen(c));
            int catsBondedCount = _cats.Count(c => _catManager.IsBonded(c));

            CatsSeenCount.text = catsSeenCount + "/" + catsCount;
            CatsBondedCount.text = catsBondedCount + "/" + catsCount;

            DisplayListByNumber();
        }

        protected void Update()
        {
            if (!_currentCatDisplayed && (CatsScene.SceneState == SceneState.ENTERING || CatsScene.SceneState == SceneState.ACTIVE) && Input.GetButtonDown("Cancel"))
            {
                CatsScene.GoHome();
            }
        }

        private void ClearCurrentList()
        {
            foreach (CatPortraitButton catPortraitButton in _catPortraitButtons)
            {
                Destroy(catPortraitButton.gameObject);
            }
            _catPortraitButtons.Clear();
        }

        private void DisplayList()
        {
            foreach (Cat cat in _cats)
            {
                CatPortraitButton catPortraitButton = Instantiate(CatPortraitButtonPrefab, PortraitsRect);
                catPortraitButton.Cat = cat;
                catPortraitButton.Button.onClick.AddListener(delegate { DisplayCatInfo(cat); });
                Random.InitState(_cats.IndexOf(cat));
                float randomRotation = (0.5f - Random.value) * 2.0f;
                catPortraitButton.transform.Rotate(new Vector3(0.0f, 0.0f, 10.0f * randomRotation));
                _catPortraitButtons.Add(catPortraitButton);
            }
        }

        private void DisplayListByNumber()
        {
            ClearCurrentList();
            _cats = new List<Cat>(GameManager.Instance.Cats.CatCollection.List);
            DisplayList();
        }

        private void DisplayListByName()
        {
            ClearCurrentList();
            _cats = _cats.OrderByDescending(c => _catManager.WasCatSeen(c)).ThenBy(c => c.Name).ToList();
            DisplayList();
        }

        private void DisplayListByRarity()
        {
            ClearCurrentList();
            _cats = _cats.OrderBy(c => c.Rarity).ThenBy(c => c.Name).ToList();
            DisplayList();
        }

        private void OnOverlayScreenHidden()
        {
            _currentCatDisplayed = null;
        }
    }
}