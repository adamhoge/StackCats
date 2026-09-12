using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.CompilerServices;

namespace Tofuwu.StackCats
{
    public enum PuzzleSceneState
    {
        Playing,
        Completing,
        Complete
    }

    public delegate void PuzzleSceneCompleting();
    public delegate void PuzzleSceneCompleted(Puzzle puzzle, PuzzleCompletionType completionType);

    [RequireComponent(typeof(PuzzleLoader))]
    public abstract class PuzzleScene : SceneBehaviour
    {
        protected class StackCatAvatar
        {
            public CatAvatar CatAvatar;
            public Stack OnStack;
            public bool IsExiting;
        }

        public event PuzzleSceneCompleting onPuzzleSceneCompleting;
        public event PuzzleSceneCompleted onPuzzleSceneCompleted;

        public float PuzzleBorder = 0.15f;
        public CatAvatar CatAvatarPrefab;
        public float PuzzleCompletionDuration = 1.5f;
        public PuzzleFlasher PuzzleFlasher;
        public AudioEvent PuzzleCompleteAudioEvent;

        /// <summary>
        /// The puzzle loader used to load the puzzle.
        /// </summary>
        public PuzzleLoader PuzzleLoader { get { return _puzzleLoader; } }

        /// <summary>
        /// The subject puzzle area of the puzzle scene.
        /// </summary>
        public PuzzleArea PuzzleArea { get { return _puzzleArea; } }

        /// <summary>
        /// Any currency found while playing the puzzle.
        /// </summary>
        public Dictionary<Currency, int> CurrencyFound { get { return _currencyFound; } }

        /// <summary>
        /// A list of cats seen while playing the puzzle.
        /// </summary>
        public List<Cat> CatsSeen { get { return _catsSeen; } set { _catsSeen = value; } }

        /// <summary>
        /// A list of new cats seen while playing the puzzle.
        /// </summary>
        public List<Cat> NewCatsSeen { get { return _newCatsSeen; } set { _newCatsSeen = value; } }

        /// <summary>
        /// The camera used for rendering.
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// The sprite renderer for the top boudnary.
        /// </summary>
        public SpriteRenderer TopBoundarySprite;

        /// <summary>
        /// The sprite renderer for the puzzle backdrop.
        /// </summary>
        public SpriteRenderer PuzzleBackdropSpriteRenderer;

        protected PuzzleLoader _puzzleLoader;
        protected AudioManager _audioManager;
        protected SceneTransitioner _sceneTransitioner;
        protected PuzzleManager _puzzleManager;
        protected CatManager _catManager;
        protected TutorialManager _tutorialManager;
        protected PuzzleArea _puzzleArea;
        protected Dictionary<Currency, int> _currencyFound = new Dictionary<Currency, int>();
        protected List<Cat> _catsSeen = new List<Cat>();
        protected List<Cat> _newCatsSeen = new List<Cat>();
        protected readonly List<StackCatAvatar> _stackCatAvatars = new List<StackCatAvatar>();
        private PuzzleSceneState _puzzleSceneState;
        private float _stateTimeElapsed;
        private Puzzle _completedPuzzle;
        private PuzzleCompletionType _completionType;

        protected override void Awake()
        {
            base.Awake();

            _puzzleLoader = GetComponent<PuzzleLoader>();
            _audioManager = _gameManager.Audio;
            _sceneTransitioner = _gameManager.GameScenes.SceneTransitioner;
            _puzzleManager = _gameManager.Puzzles;
            _catManager = _gameManager.Cats;
            _tutorialManager = _gameManager.TutorialManager;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _puzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            _puzzleLoader.onPuzzleLoaded += OnPuzzleLoaded;
            _puzzleLoader.onPuzzleBeginUnload += OnPuzzleUnloaded;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _puzzleLoader.onPuzzleBeginLoad -= OnPuzzleBeginLoad;
            _puzzleLoader.onPuzzleLoaded -= OnPuzzleLoaded;
            _puzzleLoader.onPuzzleBeginUnload -= OnPuzzleUnloaded;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected virtual void Update()
        {
            foreach (StackCatAvatar stackCatAvatar in _stackCatAvatars)
            {
                if (!stackCatAvatar.IsExiting)
                {
                    Block topBlock = stackCatAvatar.OnStack.TopBlock;
                    stackCatAvatar.CatAvatar.transform.position = topBlock ? topBlock.transform.position + Vector3.up : stackCatAvatar.OnStack.transform.position;
                }
            }

            if (_sceneState != SceneState.EXITING)
            {
                _stateTimeElapsed += Time.deltaTime;

                if (_puzzleSceneState == PuzzleSceneState.Completing)
                {
                    if (_stateTimeElapsed >= PuzzleCompletionDuration)
                    {
                        ChangeState(PuzzleSceneState.Complete);
                    }
                }
            }
        }

        protected void ShowPuzzleBackdrop(Puzzle puzzle)
        {
            LeanTween.cancel(PuzzleBackdropSpriteRenderer.gameObject);
            float backdropWidth = puzzle.Stacks.Count * (1 + puzzle.StackSpacing) + puzzle.StackSpacing + PuzzleBorder * 2;
            float backdropHeight = puzzle.MaxStackHeight + puzzle.StackSpacing * 4 + PuzzleBorder * 2;
            PuzzleBackdropSpriteRenderer.size = new Vector2(backdropWidth, backdropHeight);
            PuzzleBackdropSpriteRenderer.transform.localPosition = new Vector2(0.0f, backdropHeight / 2 - puzzle.StackSpacing - PuzzleBorder);
            LeanTween.alpha(PuzzleBackdropSpriteRenderer.gameObject, 1.0f, 1.0f).setEase(LeanTweenType.easeInOutSine);
        }

        protected void HidePuzzleBackdrop()
        {
            LeanTween.cancel(PuzzleBackdropSpriteRenderer.gameObject);
            LeanTween.alpha(PuzzleBackdropSpriteRenderer.gameObject, 0.0f, 0.5f).setEase(LeanTweenType.easeInOutSine);
        }

        protected virtual void OnPuzzleCompleted(Puzzle puzzle, PuzzleCompletionType completionType)
        {
            ChangeState(PuzzleSceneState.Completing);
            _completedPuzzle = puzzle;
            _completionType = completionType;

            if (PuzzleFlasher && completionType == PuzzleCompletionType.PuzzleSolved)
            {
                PuzzleFlasher.FlashPuzzle(puzzle);

                if (PuzzleCompleteAudioEvent)
                {
                    _audioManager.PlaySoundEffect(PuzzleCompleteAudioEvent);
                }
            }
        }

        protected virtual void OnPuzzleSceneCompleted(Puzzle puzzle, PuzzleCompletionType puzzleCompletionType)
        {
            if (onPuzzleSceneCompleted != null) onPuzzleSceneCompleted(puzzle, puzzleCompletionType);
        }

        private void ExitCatAvatar(CatAvatar catAvatar)
        {
            _stackCatAvatars.First(c => c.CatAvatar == catAvatar).IsExiting = true;
            float jumpDelay = UnityEngine.Random.Range(0.0f, 0.1f);
            LeanTween.moveY(catAvatar.gameObject, -5.0f, 0.5f + catAvatar.transform.position.y / 32.0f).setEase(LeanTweenType.easeInCubic).setDelay(jumpDelay + 0.15f);
            catAvatar.Animator.Jump();
            Destroy(catAvatar.gameObject, 1.0f);
        }

        private void ExitAllCatAvatars()
        {
            foreach (StackCatAvatar stackCatAvatar in _stackCatAvatars)
            {
                ExitCatAvatar(stackCatAvatar.CatAvatar);
            }
            _stackCatAvatars.Clear();
        }

        private void ChangeState(PuzzleSceneState state)
        {
            if (_puzzleSceneState == state) return;

            _puzzleSceneState = state;
            _stateTimeElapsed = 0.0f;

            switch (_puzzleSceneState)
            {
                case PuzzleSceneState.Playing:
                    break;
                case PuzzleSceneState.Completing:
                    if (onPuzzleSceneCompleting != null) onPuzzleSceneCompleting();
                    break;
                case PuzzleSceneState.Complete:
                    OnPuzzleSceneCompleted(_completedPuzzle, _completionType);
                    break;
            }
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            if (!wasRestarted)
            {
                LeanTween.cancel(TopBoundarySprite.gameObject);
                LeanTween.moveY(TopBoundarySprite.gameObject, puzzle.MaxStackHeight - 5.0f + 0.25f, 1.0f).setEase(LeanTweenType.easeInOutSine);
            }
        }

        private void OnPuzzleLoaded(Puzzle puzzle)
        {
            if (_sceneState != SceneState.EXITING)
            {
                PointerPuzzleController controller = puzzle.gameObject.AddComponent<PointerPuzzleController>();
                controller.Camera = Camera;

                puzzle.onCatSighting += OnCatSighting;
                puzzle.onCurrencyFound += OnCurrencyFound;
                puzzle.onBlockMoved += OnBlockMoved;
                puzzle.onPuzzleCompleted += OnPuzzleCompleted;
                puzzle.onBlockMoveResolved += OnAfterBlockResolved;
                if (controller) controller.onBlocksSelected += OnBlocksSelected;

                _tutorialManager.BeginTutorialsForPuzzle(puzzle);
            }
        }

        private void OnCurrencyFound(CatBlock source, Currency currency, int amount)
        {
            if (!_currencyFound.ContainsKey(currency))
            {
                _currencyFound.Add(currency, amount);
            }
            else
            {
                _currencyFound[currency] += amount;
            }
        }

        private void OnCatSighting(Cat cat, Puzzle puzzle, int stackIndex, int blockIndex)
        {
            if (!_catsSeen.Contains(cat)) _catsSeen.Add(cat);
            if (!_catManager.WasCatSeen(cat) && !_newCatsSeen.Contains(cat)) _newCatsSeen.Add(cat);

            Stack stack = puzzle.Stacks[stackIndex];
            CatAvatar catAvatar = Instantiate(CatAvatarPrefab, transform);
            catAvatar.Cat = cat;
            catAvatar.IsShadow = !_catManager.IsBonded(cat);
            catAvatar.Initialize();
            catAvatar.transform.position = stack.transform.position + Vector3.up * blockIndex;
            catAvatar.Animator.Jump();
            catAvatar.transform.localScale = Vector3.one * 0.25f;
            LeanTween.scale(catAvatar.gameObject, Vector3.one * 0.9f, 0.5f).setEase(LeanTweenType.easeOutSine);
            _stackCatAvatars.Add(new StackCatAvatar { CatAvatar = catAvatar, OnStack = stack });
            _gameManager.Audio.PlaySoundEffect(cat.Meow);
            switch (cat.Rarity)
            {
                case Rarity.Common:
                    catAvatar.Flash(Constants.ColorCommon, 1.0f);
                    break;
                case Rarity.Uncommon:
                    catAvatar.Flash(Constants.ColorUncommon, 1.25f);
                    break;
                case Rarity.Rare:
                    catAvatar.Flash(Constants.ColorRare, 1.5f);
                    break;
            }
        }

        private void OnBlocksSelected(PuzzleMarker marker)
        {
            List<StackCatAvatar> affectedCatAvatars = _stackCatAvatars.Where(a => a.OnStack == marker.Stack).ToList();

            while (affectedCatAvatars.Count > 0)
            {
                ExitCatAvatar(affectedCatAvatars[0].CatAvatar);
                _stackCatAvatars.Remove(affectedCatAvatars[0]);
                affectedCatAvatars.RemoveAt(0);
            }
        }

        private void OnBlockMoved(Puzzle puzzle, Stack source, Block block, Stack destination)
        {
            List<StackCatAvatar> affectedCatAvatars = _stackCatAvatars.Where(a => a.OnStack == destination).ToList();

            while (affectedCatAvatars.Count > 0)
            {
                ExitCatAvatar(affectedCatAvatars[0].CatAvatar);
                _stackCatAvatars.Remove(affectedCatAvatars[0]);
                affectedCatAvatars.RemoveAt(0);
            }
        }

        private void OnAfterBlockResolved(Puzzle puzzle, Stack source, Block block, Stack destination)
        {
            List<StackCatAvatar> affectedCatAvatars = _stackCatAvatars.Where(a => a.OnStack.Blocks.Count == puzzle.MaxMovableStackHeight).ToList();

            while (affectedCatAvatars.Count > 0)
            {
                ExitCatAvatar(affectedCatAvatars[0].CatAvatar);
                _stackCatAvatars.Remove(affectedCatAvatars[0]);
                affectedCatAvatars.RemoveAt(0);
            }
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            ExitAllCatAvatars();

            LeanTween.cancel(TopBoundarySprite.gameObject);
            LeanTween.moveY(TopBoundarySprite.gameObject, 6.25f, 1.0f).setEase(LeanTweenType.easeInOutSine);
        }
    }
}