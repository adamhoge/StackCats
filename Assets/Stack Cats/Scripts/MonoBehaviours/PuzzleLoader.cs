using System;
using System.Security.Cryptography;
using UnityEngine;
using Tofuwu.StackCats.Models;
using Random = UnityEngine.Random;


namespace Tofuwu.StackCats
{
    public delegate void PuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo);
    public delegate void PuzzleLoaded(Puzzle puzzle);
    public delegate void PuzzleBeginUnload(Puzzle puzzle);
    public delegate void PuzzleUnloaded(Puzzle puzzle);
    public delegate void PuzzleRestarted(Puzzle puzzle);

    public enum PuzzleLoaderState
    {
        Idle,
        LoadingPuzzle,
        UnloadingPuzzle
    }

    /// <summary>
    /// Responsible for loading, reloading or unloading puzzles and assigning specified controlller.
    /// </summary>
    public class PuzzleLoader : MonoBehaviour
    {
        /// <summary>
        /// Invoked at the beginning of loading a puzzle.
        /// </summary>
        public event PuzzleBeginLoad onPuzzleBeginLoad;

        /// <summary>
        /// Invoked when a puzzle has been completely loaded.
        /// </summary>
        public event PuzzleLoaded onPuzzleLoaded;

        /// <summary>
        /// Invoked at the beginning of loading a puzzle.
        /// </summary>
        public event PuzzleBeginUnload onPuzzleBeginUnload;

        /// <summary>
        /// Invoked when a puzzle has been completed unloaded.
        /// </summary>
        public event PuzzleUnloaded onPuzzleUnloaded;

        /// <summary>
        /// Invoked when a puzzle has been restarted.
        /// </summary>
        public event PuzzleRestarted onPuzzleRestarted;

        /// <summary>
        /// The sound effect used to move blocks (mimics block placement event).
        /// </summary>
        [Tooltip("The sound effect used to move blocks (mimics block placement event).")]
        public AudioEvent BlockMovedSoundEffect;

        /// <summary>
        /// The puzzle currently loaded.
        /// </summary>
        public Puzzle Puzzle { get { return _puzzle; } }

        private PuzzleLoaderState _state;
        private AudioManager _audio;
        private float _stateTimeElapsed;
        private Puzzle _puzzle;
        private Puzzle _puzzleCopy;
        private Puzzle _puzzleToLoad;
        private bool _puzzleToLoadWasRestarted;
        private bool _puzzleToLoadIsUndo;
        private bool _puzzleToLoadIsAnimated;
        private bool _reloadPuzzle;
        private float _loadTimer;
        private bool _isInteractable = true;

        /// <summary>
        /// Set whether or not the currently loaded puzzle (if any) is interactable.
        /// </summary>
        /// <param name="isInteractable"></param>
        public void SetInteractable(bool isInteractable)
        {
            if (_isInteractable == isInteractable) return;

            _isInteractable = isInteractable;

            if (_puzzle)
            {
                PointerPuzzleController pointerPuzzleController = _puzzle.GetComponent<PointerPuzzleController>();
                if (pointerPuzzleController) pointerPuzzleController.enabled = _isInteractable;
            }
        }

        /// <summary>
        /// Load a puzzle from puzzle data.
        /// </summary>
        /// <param name="puzzleData">The puzzle data from which to load the puzzle.</param>
        /// <param name="puzzleArea">The area used to build the puzzle.</param>
        public void LoadPuzzle(string puzzleJsonData, PuzzleArea puzzleArea, bool wasRestarted = false, bool isUndo = false, bool isAnimated = true)
        {
            if (_state != PuzzleLoaderState.Idle) return;

            Puzzle puzzle = PuzzleBuilder.BuildFromModel(puzzleJsonData, puzzleArea, GameManager.Instance.Cats);

            if (puzzle) LoadPuzzle(puzzle, wasRestarted, isUndo, isAnimated);
        }

        /// <summary>
        /// Load a puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle to load.</param>
        public void LoadPuzzle(Puzzle puzzle, bool wasRestarted = false, bool isUndo = false, bool animated = true)
        {
            if (_state == PuzzleLoaderState.Idle && _puzzle)
            {
                UnloadPuzzle();
            }

            if (_state == PuzzleLoaderState.UnloadingPuzzle)
            {
                if (_puzzleToLoad) Destroy(_puzzleToLoad.gameObject);

                _puzzleToLoad = puzzle;
                _puzzleToLoadWasRestarted = wasRestarted;
                _puzzleToLoadIsUndo = isUndo;
                _puzzleToLoadIsAnimated = animated;

                return;
            }

            if (_state == PuzzleLoaderState.LoadingPuzzle)
            {
                Destroy(puzzle.gameObject);
            }

            if (_state != PuzzleLoaderState.Idle || _puzzle) return;

            _puzzle = puzzle;
            _puzzle.name = "Puzzle";
            _puzzleCopy = Instantiate(puzzle, transform);
            _puzzleCopy.gameObject.SetActive(false);
            _puzzle.transform.SetParent(transform, false);

            ChangeState(PuzzleLoaderState.LoadingPuzzle);

            if (animated)
            {
                AnimateFallingBlocks();
            }
            else
            {
                _loadTimer = 0.0f;
            }

            if (onPuzzleBeginLoad != null) onPuzzleBeginLoad(_puzzle, wasRestarted, isUndo);
        }

        /// <summary>
        /// Unload the current puzzle.
        /// </summary>
        public void UnloadPuzzle()
        {
            if (_state != PuzzleLoaderState.Idle || !_puzzle) return;

            _state = PuzzleLoaderState.UnloadingPuzzle;
            if (onPuzzleBeginUnload != null) onPuzzleBeginUnload(_puzzle);
        }

        /// <summary>
        /// Reload the current puzzle in its initial state.
        /// </summary>
        public void ReloadPuzzle()
        {
            if (_state != PuzzleLoaderState.Idle || !_puzzle) return;

            UnloadPuzzle();
            _reloadPuzzle = true;
        }

        protected void Awake()
        {
            _audio = GameManager.Instance.Audio;
        }

        protected void Update()
        {
            _stateTimeElapsed += Time.deltaTime;

            switch (_state)
            {
                case PuzzleLoaderState.Idle:
                    break;
                case PuzzleLoaderState.LoadingPuzzle:
                    if (_stateTimeElapsed > _loadTimer)
                    {
                        CompleteLoadPuzzle();
                    }
                    break;
                case PuzzleLoaderState.UnloadingPuzzle:
                    // TODO: After blocks are cleaned up, dispatch onPuzzleUnloaded and set to idle.
                    CompleteUnloadPuzzle();
                    break;
            }
        }

        private void ChangeState(PuzzleLoaderState state)
        {
            _state = state;
            _stateTimeElapsed = 0.0f;
        }

        private void CompleteLoadPuzzle()
        {
            if (onPuzzleLoaded != null) onPuzzleLoaded(_puzzle);

            ChangeState(PuzzleLoaderState.Idle);
        }

        private void CompleteUnloadPuzzle()
        {
            ChangeState(PuzzleLoaderState.Idle);

            if (_reloadPuzzle)
            {
                Destroy(_puzzle.gameObject);
                _puzzle = null;
                _puzzleCopy.gameObject.SetActive(true);
                LoadPuzzle(_puzzleCopy, true);
                _reloadPuzzle = false;
                if (onPuzzleRestarted != null) onPuzzleRestarted(_puzzle);
            }
            else
            {
                Destroy(_puzzle.gameObject);
                _puzzle = null;
                Destroy(_puzzleCopy.gameObject);
                _puzzleCopy = null;
                if (onPuzzleUnloaded != null) onPuzzleUnloaded(_puzzle);
            }

            if (_puzzleToLoad != null)
            {
                LoadPuzzle(_puzzleToLoad, _puzzleToLoadWasRestarted, _puzzleToLoadIsUndo, _puzzleToLoadIsAnimated);
                _puzzleToLoad = null;
            }
        }

        private void AnimateFallingBlocks()
        {
            _loadTimer = 0.0f;
            float fallDuration = 0.6f;

            foreach (Stack stack in _puzzle.Stacks)
            {
                float delay = 0.0f;
                for (int i = 0; i < stack.Blocks.Count; i++)
                {
                    float blockY = stack.Blocks[i].transform.position.y;
                    stack.Blocks[i].transform.Translate(Vector2.up * 13);
                    delay += 0.05f - i * 0.005f + Random.Range(0.0f, 0.1f);
                    Action<object> playSound = PlaySound;
                    LeanTween
                        .moveLocalY(stack.Blocks[i].gameObject, blockY, fallDuration)
                        .setDelay(delay)
                        .setEase(LeanTweenType.easeInSine)
                        .setOnComplete(playSound, stack);
                }
                if (delay > _loadTimer) _loadTimer = delay;
            }

            _loadTimer += fallDuration;
        }

        private void PlaySound(object stackObject)
        {
            Stack stack = (Stack)stackObject;
            int stackIndex = _puzzle.Stacks.IndexOf(stack);
            float panning = (float)stackIndex / (_puzzle.Stacks.Count - 1) * 0.5f - 0.25f;
            if (BlockMovedSoundEffect) _audio.PlaySoundEffect(BlockMovedSoundEffect, panning);
        }
    }
}