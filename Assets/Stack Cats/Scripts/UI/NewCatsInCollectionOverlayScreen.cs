using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class NewCatsInCollectionOverlayScreen : OverlayScreen
    {
        private enum State
        {
            NotStarted,
            NavigatingToCatPortrait,
            RevealingCatPortrait,
            DisplayingCatPortrait,
            Complete
        }

        public List<Cat> NewCats = new List<Cat>();
        public float StartDelay = 0.5f;
        public float NavigationDuration = 1.5f;
        public float RevealDuration = 1.5f;
        public float DisplayDuration = 1.5f;
        public RectTransform CatPortraitsRectTransform;
        public GridLayoutGroup CatPortraitsGrid;
        public CatPortrait CatPortraitPrefab;
        public int NumColumns = 4;

        private CatManager _catManager;
        private State _state;
        private float _stateTimeElapsed;
        private List<CatPortrait> _allCatPortraits = new List<CatPortrait>();
        private List<CatPortrait> _catPortraitsToReveal = new List<CatPortrait>();
        private CatPortrait _currentPortrait;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            _state = State.NotStarted;
            LoadCatPortraits();
        }

        protected void Awake()
        {
            _catManager = GameManager.Instance.Cats;
        }

        protected override void Update()
        {
            base.Update();

            if (_isActive)
            {
                _stateTimeElapsed += Time.deltaTime;

                if (Input.GetMouseButtonDown(0))
                {
                    switch (_state)
                    {
                        case State.NavigatingToCatPortrait:
                            SkipNavigation();
                            break;
                        case State.RevealingCatPortrait:
                            SkipReveal();
                            break;
                        case State.DisplayingCatPortrait:
                            SkipDisplayPortrait();
                            break;
                        case State.Complete:
                            Dismiss();
                            break;
                    }
                }

                switch (_state)
                {
                    case State.NotStarted:
                        UpdateNotStarted();
                        break;
                    case State.NavigatingToCatPortrait:
                        UpdateNavigation();
                        break;
                    case State.RevealingCatPortrait:
                        UpdateRevealCat();
                        break;
                    case State.DisplayingCatPortrait:
                        UpdateDisplayCat();
                        break;
                }
            }
        }

        private void LoadCatPortraits()
        {
            foreach(CatPortrait catPortrait in _allCatPortraits)
            {
                Destroy(catPortrait.gameObject);
            }
            _allCatPortraits.Clear();

            foreach (Cat cat in _catManager.CatCollection.List)
            {
                CatPortrait catPortrait = Instantiate(CatPortraitPrefab, CatPortraitsGrid.transform);
                catPortrait.Cat = cat;
                catPortrait.WasSeen = _catManager.WasCatSeen(cat);
                catPortrait.IsBonded = _catManager.IsBonded(cat);
                if (NewCats.Contains(cat))
                {
                    catPortrait.WasSeen = false;
                    _catPortraitsToReveal.Add(catPortrait);
                }
                else
                {
                    catPortrait.WasSeen = _catManager.WasCatSeen(cat);
                }
                _allCatPortraits.Add(catPortrait);
            }

            NewCats.Clear();
        }

        private void ShowNextCat()
        {
            if (_catPortraitsToReveal.Count > 0)
            {
                _currentPortrait = _catPortraitsToReveal[0];
                _catPortraitsToReveal.RemoveAt(0);
                ChangeState(State.NavigatingToCatPortrait);
            }
            else
            {
                Dismiss();
            }
        }

        private void NavigateToCatPortrait()
        {
            float yOffset = GetCatYOffset(_currentPortrait.Cat);
            LeanTween.moveLocalY(CatPortraitsGrid.gameObject, yOffset, NavigationDuration - 0.1f).setEase(LeanTweenType.easeInOutQuad);
        }

        private float GetCatYOffset(Cat cat)
        {
            int catPortraitRow = _allCatPortraits.IndexOf(_currentPortrait) / NumColumns;
            float gridOffset = (catPortraitRow + 0.5f) * (CatPortraitsGrid.cellSize.y + CatPortraitsGrid.spacing.y);
            float minOffset = -CatPortraitsRectTransform.rect.y / 2;
            float maxOffset = minOffset + (GetGridTotalHeight() - minOffset * 2);
            if (maxOffset < minOffset) maxOffset = minOffset;
            float gridOffsetClamped = Mathf.Clamp(gridOffset, minOffset, maxOffset);
            return gridOffsetClamped;
        }

        private float GetGridTotalHeight()
        {
            float gridHeight = (CatPortraitsGrid.cellSize.y + CatPortraitsGrid.spacing.y) * Mathf.CeilToInt(_allCatPortraits.Count / (float)NumColumns);
            float gridHeightPadding = (CatPortraitsGrid.padding.top + CatPortraitsGrid.padding.bottom);
            return gridHeight + gridHeightPadding;
        }

        private void SkipNavigation()
        {
            LeanTween.cancel(CatPortraitsGrid.gameObject);
            CatPortraitsGrid.transform.localPosition = Vector2.up * GetCatYOffset(_currentPortrait.Cat);
            ChangeState(State.RevealingCatPortrait);
        }

        private void SkipReveal()
        {
            ChangeState(State.DisplayingCatPortrait);
        }

        private void SkipDisplayPortrait()
        {
            ChangeState(State.Complete);
        }

        private void UpdateNavigation()
        {
            if (_stateTimeElapsed >= NavigationDuration)
            {
                ChangeState(State.RevealingCatPortrait);
            }
        }

        private void UpdateNotStarted()
        {
            if(_stateTimeElapsed >= StartDelay)
            {
                ShowNextCat();
            }
        }

        private void UpdateRevealCat()
        {
            if (_stateTimeElapsed >= RevealDuration)
            {
                ChangeState(State.DisplayingCatPortrait);
            }
        }

        private void UpdateDisplayCat()
        {
            if (_stateTimeElapsed >= DisplayDuration)
            {
                ChangeState(State.Complete);
            }
        }

        private void ChangeState(State state)
        {
            if (_state == state) return;

            _state = state;
            _stateTimeElapsed = 0.0f;

            switch (_state)
            {
                case State.NotStarted:
                    break;
                case State.NavigatingToCatPortrait:
                    NavigateToCatPortrait();
                    break;
                case State.RevealingCatPortrait:
                    _currentPortrait.SetWasSeen();
                    break;
                case State.DisplayingCatPortrait:
                    break;
                case State.Complete:
                    ShowNextCat();
                    break;
            }
        }
    }
}
