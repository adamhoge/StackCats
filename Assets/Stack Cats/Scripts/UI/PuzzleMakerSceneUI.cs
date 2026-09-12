using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleMakerSceneUI : MonoBehaviour
    {
        public PuzzleMakerScene PuzzleMakerScene;
        public PuzzleCameraAdjuster PuzzleCameraAdjuster;
        public Dropdown AreaEditorSelectDropdown;
        public RectTransform PuzzleMakerActionsRectTransform;
        public Image HeaderImage;
        public Image FooterImage;
        public Image TopBoundaryImage;
        public Image BottomBoundaryImage;
        public List<Button> UIButtons = new List<Button>();
        public PuzzleMakerFarmFlavoredActionsUI PuzzleMakerFarmFlavoredActionsPrefab;
        public PuzzleMakerJungleFlavoredActionsUI PuzzleMakerJungleFlavoredActionsPrefab;
        public PuzzleMakerNightFlavoredActionsUI PuzzleMakerNightFlavoredActionsPrefab;
        public PuzzleMakerDesertFlavoredActionsUI PuzzleMakerDesertFlavoredActionsPrefab;
        public Button DataButton;
        public Button PlaytestButton;

        private PuzzleMakerActionsUI _currentPuzzleMakerActionsUI;

        protected virtual void OnEnable()
        {
            AreaEditorSelectDropdown.onValueChanged.AddListener(OnPuzzleAreaEditorSelected);
            PuzzleMakerScene.onAreaEditorChanged += OnPuzzleAreaEditorChanged;
            PuzzleMakerScene.onActionsEditorStarted += OnActionEditorStarted;
            PuzzleMakerScene.onActionsEditorStopped += OnActionEditorStopped;
            PuzzleMakerScene.onPuzzleMakerPuzzleLoaded += OnPuzzleLoaded;
            PuzzleMakerScene.onPuzzleMakerPuzzleUnloaded += OnPuzzleUnloaded;
        }

        protected virtual void OnDisable()
        {
            AreaEditorSelectDropdown.onValueChanged.RemoveListener(OnPuzzleAreaEditorSelected);
            PuzzleMakerScene.onAreaEditorChanged -= OnPuzzleAreaEditorChanged;
            PuzzleMakerScene.onActionsEditorStarted -= OnActionEditorStarted;
            PuzzleMakerScene.onActionsEditorStopped -= OnActionEditorStopped;
            PuzzleMakerScene.onPuzzleMakerPuzzleLoaded -= OnPuzzleLoaded;
            PuzzleMakerScene.onPuzzleMakerPuzzleUnloaded -= OnPuzzleUnloaded;
        }

        protected virtual void Start()
        {
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (PuzzleMakerAreaEditor areaEditor in PuzzleMakerScene.AreaEditors)
            {
                Dropdown.OptionData optionData = new Dropdown.OptionData(areaEditor.PuzzleArea.AreaTitle);
                options.Add(optionData);
            }
            AreaEditorSelectDropdown.AddOptions(options);
        }

        protected virtual void Update()
        {
            AreaEditorSelectDropdown.interactable = !PuzzleMakerScene.IsTesting;
            DataButton.interactable = !PuzzleMakerScene.IsTesting;
        }

        private void OnPuzzleAreaEditorSelected(int selection)
        {
            PuzzleMakerScene.CurrentAreaEditor = PuzzleMakerScene.AreaEditors[selection];
        }

        private void OnPuzzleAreaEditorChanged(PuzzleMakerAreaEditor areaEditor)
        {
            AreaEditorSelectDropdown.value = PuzzleMakerScene.AreaEditors.IndexOf(areaEditor);

            PuzzleTheme puzzleTheme = areaEditor.PuzzleArea.PuzzleTheme;
            HeaderImage.color = puzzleTheme.UIColor;
            FooterImage.color = puzzleTheme.UIColor;
            TopBoundaryImage.sprite = puzzleTheme.TopBoundarySprite;
            BottomBoundaryImage.sprite = puzzleTheme.BottomBoundarySprite;
            foreach (Button button in UIButtons)
            {
                button.image.color = puzzleTheme.UIColor;
            }
        }

        private void OnActionEditorStarted(PuzzleMakerActions actionsEditor, PuzzleMakerActionsController actionsController)
        {
            if (actionsEditor.GetType() == typeof(PuzzleMakerNightFlavoredActions))
            {
                _currentPuzzleMakerActionsUI = Instantiate(PuzzleMakerNightFlavoredActionsPrefab, PuzzleMakerActionsRectTransform);
            }
            else if (actionsEditor.GetType() == typeof(PuzzleMakerDesertFlavoredActions))
            {
                _currentPuzzleMakerActionsUI = Instantiate(PuzzleMakerDesertFlavoredActionsPrefab, PuzzleMakerActionsRectTransform);
            }
            else if (actionsEditor.GetType() == typeof(PuzzleMakerJungleFlavoredActions))
            {
                _currentPuzzleMakerActionsUI = Instantiate(PuzzleMakerJungleFlavoredActionsPrefab, PuzzleMakerActionsRectTransform);
            }
            else if (actionsEditor.GetType() == typeof(PuzzleMakerFarmFlavoredActions))
            {
                _currentPuzzleMakerActionsUI = Instantiate(PuzzleMakerFarmFlavoredActionsPrefab, PuzzleMakerActionsRectTransform);
            }
            if(_currentPuzzleMakerActionsUI != null)
            {
                _currentPuzzleMakerActionsUI.PuzzleMakerActions = actionsEditor;
                _currentPuzzleMakerActionsUI.PuzzleMakerActionsController = actionsController;
            }
        }

        private void OnActionEditorStopped()
        {
            Destroy(_currentPuzzleMakerActionsUI.gameObject);
        }

        private void OnPuzzleLoaded(Puzzle puzzle)
        {
            PuzzleCameraAdjuster.FitCameraAndUIToPuzzleAspect(puzzle);
        }

        private void OnPuzzleUnloaded()
        {
            PuzzleCameraAdjuster.FitCameraAndUIToMenu();
        }
    }
}