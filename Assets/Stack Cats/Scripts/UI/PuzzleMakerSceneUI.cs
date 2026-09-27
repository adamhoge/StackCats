using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockhopperGames.StackCats.UI
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
        private PuzzleManager _puzzleManager;
        private string _currentNumberInput = "";
        private float _numberInputStartTime = -1f;
        private const float NUMBER_INPUT_TIMEOUT = 0.5f;

        protected virtual void OnEnable()
        {
            AreaEditorSelectDropdown.onValueChanged.AddListener(OnPuzzleAreaEditorSelected);
            PuzzleMakerScene.onAreaEditorChanged += OnPuzzleAreaEditorChanged;
            PuzzleMakerScene.onActionsEditorStarted += OnActionEditorStarted;
            PuzzleMakerScene.onActionsEditorStopped += OnActionEditorStopped;
            PuzzleMakerScene.onPuzzleMakerPuzzleLoaded += OnPuzzleLoaded;
            PuzzleMakerScene.onPuzzleMakerPuzzleUnloaded += OnPuzzleUnloaded;
            _puzzleManager = GameManager.Instance.Puzzles;
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
                Dropdown.OptionData optionData = new Dropdown.OptionData(
                    areaEditor.PuzzleArea.AreaTitle
                );
                options.Add(optionData);
            }
            AreaEditorSelectDropdown.AddOptions(options);
        }

        protected virtual void Update()
        {
            AreaEditorSelectDropdown.interactable = !PuzzleMakerScene.IsTesting;
            DataButton.interactable = !PuzzleMakerScene.IsTesting;

            // Handle number input for quick story puzzle loading (when Shift is held)
            if (
                !PuzzleMakerScene.IsTesting
                && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            )
            {
                for (int i = 0; i <= 9; i++)
                {
                    KeyCode keyCode = KeyCode.Alpha0 + i;
                    if (Input.GetKeyDown(keyCode))
                    {
                        HandleNumberInput(i);
                        break;
                    }
                }

                // Check if timeout has elapsed and we should attempt to load
                if (!string.IsNullOrEmpty(_currentNumberInput) && _numberInputStartTime >= 0f)
                {
                    if (Time.time - _numberInputStartTime >= NUMBER_INPUT_TIMEOUT)
                    {
                        TryLoadStoryPuzzle();
                    }
                }
            }
            else
            {
                // Shift is not held, re-enable controller if it was disabled
                if (!string.IsNullOrEmpty(_currentNumberInput))
                {
                    _currentNumberInput = "";
                    _numberInputStartTime = -1f;
                    PuzzleMakerScene.SetControllerEnabled(true);
                }
            }
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

        private void OnActionEditorStarted(
            PuzzleMakerActions actionsEditor,
            PuzzleMakerActionsController actionsController
        )
        {
            if (actionsEditor.GetType() == typeof(PuzzleMakerNightFlavoredActions))
            {
                _currentPuzzleMakerActionsUI = Instantiate(
                    PuzzleMakerNightFlavoredActionsPrefab,
                    PuzzleMakerActionsRectTransform
                );
            }
            else if (actionsEditor.GetType() == typeof(PuzzleMakerDesertFlavoredActions))
            {
                _currentPuzzleMakerActionsUI = Instantiate(
                    PuzzleMakerDesertFlavoredActionsPrefab,
                    PuzzleMakerActionsRectTransform
                );
            }
            else if (actionsEditor.GetType() == typeof(PuzzleMakerJungleFlavoredActions))
            {
                _currentPuzzleMakerActionsUI = Instantiate(
                    PuzzleMakerJungleFlavoredActionsPrefab,
                    PuzzleMakerActionsRectTransform
                );
            }
            else if (actionsEditor.GetType() == typeof(PuzzleMakerFarmFlavoredActions))
            {
                _currentPuzzleMakerActionsUI = Instantiate(
                    PuzzleMakerFarmFlavoredActionsPrefab,
                    PuzzleMakerActionsRectTransform
                );
            }
            if (_currentPuzzleMakerActionsUI != null)
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

        private void HandleNumberInput(int digit)
        {
            if (_currentNumberInput.Length == 0)
            {
                // First digit pressed - disable puzzle actions to prevent them from receiving this input
                _currentNumberInput = digit.ToString();
                _numberInputStartTime = Time.time;
                PuzzleMakerScene.SetControllerEnabled(false);
            }
            else if (_currentNumberInput.Length == 1)
            {
                // Second digit pressed, immediately attempt to load
                _currentNumberInput += digit.ToString();
                TryLoadStoryPuzzle();
            }
        }

        private void TryLoadStoryPuzzle()
        {
            if (string.IsNullOrEmpty(_currentNumberInput))
            {
                PuzzleMakerScene.SetControllerEnabled(true);
                return;
            }

            if (!int.TryParse(_currentNumberInput, out int puzzleIndex))
            {
                _currentNumberInput = "";
                _numberInputStartTime = -1f;
                PuzzleMakerScene.SetControllerEnabled(true);
                return;
            }

            // Adjust for 1-based indexing (user types 1-based, but list is 0-based)
            puzzleIndex--;

            List<StoryPuzzle> storyPuzzles = _puzzleManager.GetStoryPuzzles(
                PuzzleMakerScene.CurrentAreaEditor.PuzzleArea
            );

            if (puzzleIndex >= 0 && puzzleIndex < storyPuzzles.Count)
            {
                StoryPuzzle selectedPuzzle = storyPuzzles[puzzleIndex];
                if (selectedPuzzle != null)
                {
                    PuzzleMakerScene.LoadPuzzle(selectedPuzzle.JsonData);
                }
            }

            _currentNumberInput = "";
            _numberInputStartTime = -1f;
            PuzzleMakerScene.SetControllerEnabled(true);
        }
    }
}
