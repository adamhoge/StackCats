using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class PuzzleMakerAreaEditor
    {
        public PuzzleArea PuzzleArea;
    }

    public delegate void AreaEditorChanged(PuzzleMakerAreaEditor areaEditor);
    public delegate void ActionEditorStarted(PuzzleMakerActions actionsEditor, PuzzleMakerActionsController actionsEditorController);
    public delegate void ActionEditorStopped();
    public delegate void PuzzleMakerPuzzleLoaded(Puzzle puzzle);
    public delegate void PuzzleMakerPuzzleUnloaded();

    [RequireComponent(typeof(PuzzleLoader))]
    public class PuzzleMakerScene : SceneBehaviour
    {
        /// <summary>
        /// Invoked whenever a puzzle is loaded.
        /// </summary>
        public event PuzzleMakerPuzzleLoaded onPuzzleMakerPuzzleLoaded;

        /// <summary>
        /// Invoked whenever a puzzle is unloaded.
        /// </summary>
        public event PuzzleMakerPuzzleUnloaded onPuzzleMakerPuzzleUnloaded;

        /// <summary>
        /// Invoked whenever the area editor in use changes.
        /// </summary>
        public event AreaEditorChanged onAreaEditorChanged;

        /// <summary>
        /// Invoked whenever a puzzle editor is started.
        /// </summary>
        public event ActionEditorStarted onActionsEditorStarted;

        /// <summary>
        /// Invoked whenever a puzzle editor is stopped.
        /// </summary>
        public event ActionEditorStopped onActionsEditorStopped;

        public Camera Camera;

        /// <summary>
        /// Editors used to create, save and load puzzles for a given area.
        /// </summary>
        public List<PuzzleMakerAreaEditor> AreaEditors = new List<PuzzleMakerAreaEditor>();

        /// <summary>
        /// The puzzle area for which puzzles will be made.
        /// </summary>
        public PuzzleMakerAreaEditor CurrentAreaEditor { get { return _currentAreaEditor; } set { SetCurrentAreaEditor(value); } }

        /// <summary>
        /// Indicates whether or not a puzzle is currently being tested.
        /// </summary>
        public bool IsTesting { get { return _isTesting; } }


        private PuzzleLoader _puzzleLoader;
        private PuzzleLoader _playtestPuzzleLoader;
        private PuzzleMakerAreaEditor _currentAreaEditor;
        private PuzzleMakerActions _currentPuzzleMakerActions;
        private PuzzleMakerActionsController _currentPuzzleMakerActionsController;
        private GameObject _puzzleAreaScenary;
        private string _puzzleJsonData;
        private bool _isTesting;

        /// <summary>
        /// Go home.
        /// </summary>
        public void GoHome()
        {
            _gameManager.ConfirmAction("Are you sure you want to exit the puzzle maker?", ConfirmGoHome);
        }

        /// <summary>
        /// Create a new puzzle.
        /// </summary>
        public void NewPuzzle()
        {
            if (_isTesting) return;

            Puzzle puzzle = Instantiate(_currentAreaEditor.PuzzleArea.PuzzlePrefab, transform);
            puzzle.MaxStackHeight = 8;
            for (int i = 0; i < 5; i++)
            {
                puzzle.AddNewStack();
            }
            puzzle.IsEditMode = true;
            _puzzleLoader.LoadPuzzle(puzzle);
            if (onPuzzleMakerPuzzleLoaded != null) onPuzzleMakerPuzzleLoaded(puzzle);
        }

        /// <summary>
        /// Load an existing puzzle.
        /// </summary>
        public void LoadPuzzle(string puzzleJsonData)
        {
            _puzzleLoader.LoadPuzzle(puzzleJsonData, _currentAreaEditor.PuzzleArea, false, false, false);
            _puzzleLoader.Puzzle.IsEditMode = true;
            if (onPuzzleMakerPuzzleLoaded != null) onPuzzleMakerPuzzleLoaded(_puzzleLoader.Puzzle);
        }

        public void LoadPuzzleFromClipboard()
        {
            LoadPuzzle(GUIUtility.systemCopyBuffer);
        }

        /// <summary>
        /// Save current puzzle.
        /// </summary>
        public void SavePuzzle() { }

        /// <summary>
        /// Begin playtesting the puzzle.
        /// </summary>
        public void BeginPlaytest()
        {
            Puzzle currentPuzzle = _puzzleLoader.Puzzle;

            if (!currentPuzzle) return;

            _puzzleJsonData = PuzzleBuilder.GetPuzzleJsonData(currentPuzzle);
            _puzzleLoader.UnloadPuzzle();
            _playtestPuzzleLoader.LoadPuzzle(_puzzleJsonData, _currentAreaEditor.PuzzleArea, false, false, false);
            _isTesting = true;
            if (onPuzzleMakerPuzzleLoaded != null) onPuzzleMakerPuzzleLoaded(_playtestPuzzleLoader.Puzzle);
        }

        /// <summary>
        /// End playtesting the puzzle.
        /// </summary>
        public void EndPlaytest()
        {
            if (!_isTesting) return;

            _playtestPuzzleLoader.UnloadPuzzle();
            _isTesting = false;
            LoadPuzzle(_puzzleJsonData);
        }

        /// <summary>
        /// Begin/end playtesting.
        /// </summary>
        public void TogglePlaytest()
        {
            if (_isTesting)
            {
                EndPlaytest();
            }
            else
            {
                BeginPlaytest();
            }
        }

        /// <summary>
        /// Unload the current puzzle.
        /// </summary>
        public void UnloadPuzzle()
        {
            _puzzleLoader.UnloadPuzzle();
            if (onPuzzleMakerPuzzleUnloaded != null) onPuzzleMakerPuzzleUnloaded();
        }

        /// <summary>
        /// Copy the current puzzle json data to the clipboard.
        /// </summary>
        public void CopyPuzzleDataToClipboard()
        {
            Puzzle currentPuzzle = _puzzleLoader.Puzzle;

            if (!currentPuzzle) return;

            GUIUtility.systemCopyBuffer = PuzzleBuilder.GetPuzzleJsonData(currentPuzzle);
        }

        public void SetControllerEnabled(bool isEnabled)
        {
            if (_currentPuzzleMakerActionsController) _currentPuzzleMakerActionsController.IsEnabled = isEnabled;
        }

        protected override void Awake()
        {
            base.Awake();

            _puzzleLoader = GetComponent<PuzzleLoader>();
            _playtestPuzzleLoader = new GameObject().AddComponent<PuzzleLoader>();
            _playtestPuzzleLoader.transform.SetParent(transform);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _puzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            _puzzleLoader.onPuzzleBeginUnload += OnPuzzleBeginUnload;
            _playtestPuzzleLoader.onPuzzleBeginLoad += OnPlaytestPuzzleBeginLoad;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _puzzleLoader.onPuzzleBeginLoad -= OnPuzzleBeginLoad;
            _puzzleLoader.onPuzzleBeginUnload -= OnPuzzleBeginUnload;
            _playtestPuzzleLoader.onPuzzleBeginLoad -= OnPlaytestPuzzleBeginLoad;
        }

        protected override void Start()
        {
            base.Start();

            SetCurrentAreaEditor(AreaEditors[0]);
        }

        protected void Update()
        {
            if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    LoadPuzzleFromClipboard();
                }
            }
        }

        private void ConfirmGoHome()
        {
            _gameManager.GoHome();
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            PuzzleMakerActionsController controller = null;
            if (puzzle.GetType() == typeof(NightFlavoredPuzzle))
            {
                // TODO: Add actions specific to Night Flavored Puzzles.
                _currentPuzzleMakerActions = gameObject.AddComponent<PuzzleMakerNightFlavoredActions>();
                controller = puzzle.gameObject.AddComponent<PuzzleMakerNightFlavoredActionsController>();
                controller.Camera = Camera;
                controller.PuzzleMakerActions = _currentPuzzleMakerActions;
            }
            else if (puzzle.GetType() == typeof(DesertFlavoredPuzzle))
            {
                // TODO: Add actions specific to Desert Flavored Puzzles.
                _currentPuzzleMakerActions = gameObject.AddComponent<PuzzleMakerDesertFlavoredActions>();
                controller = puzzle.gameObject.AddComponent<PuzzleMakerDesertFlavoredActionsController>();
                controller.Camera = Camera;
                controller.PuzzleMakerActions = _currentPuzzleMakerActions;
            }
            else if (puzzle.GetType() == typeof(JungleFlavoredPuzzle))
            {
                // TODO: Add actions specific to Jungle Flavored Puzzles.
                _currentPuzzleMakerActions = gameObject.AddComponent<PuzzleMakerJungleFlavoredActions>();
                controller = puzzle.gameObject.AddComponent<PuzzleMakerJungleFlavoredActionsController>();
                controller.Camera = Camera;
                controller.PuzzleMakerActions = _currentPuzzleMakerActions;
            }
            else if (puzzle.GetType() == typeof(FarmFlavoredPuzzle))
            {
                // TODO: Add actions specific to Farm Flavored Puzzles.
                _currentPuzzleMakerActions = gameObject.AddComponent<PuzzleMakerFarmFlavoredActions>();
                controller = puzzle.gameObject.AddComponent<PuzzleMakerFarmFlavoredActionsController>();
                controller.Camera = Camera;
                controller.PuzzleMakerActions = _currentPuzzleMakerActions;
            }

            if (_currentPuzzleMakerActions != null)
            {
                _currentPuzzleMakerActions.Puzzle = puzzle;
                _currentPuzzleMakerActions.PuzzleArea = _currentAreaEditor.PuzzleArea;
                _currentPuzzleMakerActionsController = controller;
                if (onActionsEditorStarted != null) onActionsEditorStarted(_currentPuzzleMakerActions, controller);
            }
        }

        private void OnPuzzleBeginUnload(Puzzle puzzle)
        {
            DestroyImmediate(_currentPuzzleMakerActions);
            if (onActionsEditorStopped != null) onActionsEditorStopped();
        }

        private void OnPlaytestPuzzleBeginLoad(Puzzle puzzle, bool isRestarted, bool isUndo)
        {
            PointerPuzzleController controller = puzzle.gameObject.AddComponent<PointerPuzzleController>();
            controller.Camera = Camera;
        }

        private void SetCurrentAreaEditor(PuzzleMakerAreaEditor areaEditor)
        {
            if (_currentAreaEditor == areaEditor) return;

            if (_currentAreaEditor != null)
            {
                Destroy(_puzzleAreaScenary.gameObject);
                UnloadPuzzle();
            }

            _currentAreaEditor = areaEditor;
            _puzzleAreaScenary = Instantiate(_currentAreaEditor.PuzzleArea.PuzzleTheme.PuzzleScenaryPrefab, transform);
            Camera.backgroundColor = _currentAreaEditor.PuzzleArea.PuzzleTheme.BackgroundColor;
            if (onAreaEditorChanged != null) onAreaEditorChanged(_currentAreaEditor);
        }
    }
}