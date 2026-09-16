using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public enum TutorialSceneState
    {
        NotStarted,
        HearingNoise,
        FreeingCat,
        FreeingCatWithHelper,
        MeetingCat,
        Zoom1,
        Zoom2,
        Zoom3,
        BondingWithCat,
        ChatIntroductions,
        NoticingHouse,
        ChatAboutHouse,
        MovingToBlockStackingPuzzle,
        UnloadingPreviousPuzzle,
        ChatAboutStairs,
        BuildingStairs,
        BuildingStairsWithHelper,
        HeadingHome,
        Completed,
    }

    public delegate void BoxThumped(Vector3 boxPosition, int direction);

    [RequireComponent(typeof(PuzzleLoader))]
    public class TutorialScene : SceneBehaviour
    {
        public delegate void EnterState(TutorialSceneState state);

        public event EnterState onEnterState;
        public event BoxThumped onBoxThumped;

        public Camera Camera;
        public AudioLoop TutorialBackgroundMusic;
        public TutorialCatAvatar TutorialCatAvatarPrefab;
        public Cat ChatCat;
        public FarmFlavoredPuzzle FreeCatPuzzle;
        public FarmFlavoredPuzzle StairsPuzzle;
        public List<TutorialSceneState> PuzzleInteractableStates;
        public AudioEvent ThumpSoundEffect;
        public AudioEvent FreeChatCatSoundEffect;
        public AudioEvent OpenBondingUISoundEffect;
        public AudioEvent ZoomInOnChatCatSoundEffect;
        public CutScene EnterChatCatCutScene;
        public GameObject Clouds;
        public AudioLoop TutorialAmbience;
        public PuzzleFlasher PuzzleFlasher;

        private TutorialCatAvatar _catAvatar;
        private PuzzleLoader _puzzleLoader;
        private TutorialSceneState _state = TutorialSceneState.NotStarted;
        private float _stateTimeElapsed;
        private Block _chatCatBlock;
        private float _nextRustleTime;
        private bool _isPuzzleInteractable;
        private bool _areHeartsEmitted;
        private Guid _ambienceVolumeGuid;

        public void SetPuzzleInteractable(bool isInteractable)
        {
            _isPuzzleInteractable = isInteractable && PuzzleInteractableStates.Contains(_state);
            _puzzleLoader.SetInteractable(_isPuzzleInteractable);
        }

        public void ConfirmChat()
        {
            switch (_state)
            {
                case TutorialSceneState.ChatIntroductions:
                    ChangeState(TutorialSceneState.NoticingHouse);
                    break;
                case TutorialSceneState.ChatAboutHouse:
                    ChangeState(TutorialSceneState.MovingToBlockStackingPuzzle);
                    break;
                case TutorialSceneState.ChatAboutStairs:
                    ChangeState(TutorialSceneState.UnloadingPreviousPuzzle);
                    break;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _puzzleLoader = GetComponent<PuzzleLoader>();

            _chatCatBlock = FreeCatPuzzle.Stacks[1].Blocks[0];
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _puzzleLoader.onPuzzleLoaded += OnTutorialPuzzleLoaded;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _puzzleLoader.onPuzzleLoaded -= OnTutorialPuzzleLoaded;
        }

        protected override void Start()
        {
            base.Start();

            _state = _gameManager.Data.TutorialData.TutorialState;
            LeanTween.moveLocalX(Clouds, 0.25f, 8.0f);

            _ambienceVolumeGuid = _gameManager.Audio.AddBackgroundMusicVolumeModifier(1.0f);
            _gameManager.Audio.PlayBackgroundMusic(TutorialAmbience);
            LeanTween
                .value(0.0f, 1.0f, 1.0f)
                .setOnUpdate(
                    (float value) =>
                    {
                        _gameManager.Audio.SetBackgroundMusicVolumeModifier(
                            _ambienceVolumeGuid,
                            value
                        );
                    }
                );

            ChangeState(TutorialSceneState.HearingNoise);
        }

        protected void Update()
        {
            _stateTimeElapsed += Time.deltaTime;

            switch (_state)
            {
                case TutorialSceneState.HearingNoise:
                    if (_stateTimeElapsed > 6.0f)
                        ChangeState(TutorialSceneState.FreeingCat);
                    UpdateCatBlockRustling();
                    break;
                case TutorialSceneState.FreeingCat:
                    if (_stateTimeElapsed > 5.0f)
                        ChangeState(TutorialSceneState.FreeingCatWithHelper);
                    UpdateCatBlockRustling();
                    break;
                case TutorialSceneState.FreeingCatWithHelper:
                    UpdateCatBlockRustling();
                    break;
                case TutorialSceneState.MeetingCat:
                    if (_stateTimeElapsed > 3.0f)
                        ChangeState(TutorialSceneState.Zoom1);
                    // PlayAnimation
                    break;
                case TutorialSceneState.Zoom1:
                    if (_stateTimeElapsed > 1.0f)
                        ChangeState(TutorialSceneState.Zoom2);
                    break;
                case TutorialSceneState.Zoom2:
                    if (_stateTimeElapsed > 1.0f)
                        ChangeState(TutorialSceneState.Zoom3);
                    break;
                case TutorialSceneState.Zoom3:
                    if (_stateTimeElapsed > 2.0f)
                        ChangeState(TutorialSceneState.BondingWithCat);
                    break;
                case TutorialSceneState.BondingWithCat:
                    if (!_areHeartsEmitted && _stateTimeElapsed > 6.0f)
                    {
                        _catAvatar.Animator.HeartsParticleSystem.Play();
                        _areHeartsEmitted = true;
                    }
                    if (_stateTimeElapsed > 9.0f)
                        ChangeState(TutorialSceneState.ChatIntroductions);
                    break;
                case TutorialSceneState.NoticingHouse:
                    if (_stateTimeElapsed > 2.0f)
                        ChangeState(TutorialSceneState.ChatAboutHouse);
                    break;
                case TutorialSceneState.MovingToBlockStackingPuzzle:
                    if (_stateTimeElapsed > 2.0f)
                        ChangeState(TutorialSceneState.ChatAboutStairs);
                    break;
                case TutorialSceneState.BuildingStairs:
                    if (_stateTimeElapsed > 5.0f)
                        ChangeState(TutorialSceneState.BuildingStairsWithHelper);
                    break;
                case TutorialSceneState.HeadingHome:
                    if (_stateTimeElapsed > 3.0f)
                        ChangeState(TutorialSceneState.Completed);
                    break;
            }
        }

        public void SkipTutorial()
        {
            _gameManager.ConfirmAction(
                "Are you sure you want to skip the tutorial?",
                ConfirmSkipTutorial
            );
        }

        private void ConfirmSkipTutorial()
        {
            if (!_gameManager.Cats.WasCatSeen(ChatCat))
            {
                CatReward reward = _gameManager.Cats.AddCatSighting(ChatCat);
                _gameManager.Stuff.AddPresent(ChatCat, reward.Currency, reward.Items);
            }

            _gameManager.Data.TutorialData.TutorialState = TutorialSceneState.Completed;

            _gameManager.GoHome();
        }

        private void ChangeState(TutorialSceneState state)
        {
            if (_state == state)
                return;

            _state = state;
            _stateTimeElapsed = 0.0f;

            // Enter state handler
            switch (_state)
            {
                case TutorialSceneState.HearingNoise:
                    FreeCatPuzzle.gameObject.SetActive(true);
                    _puzzleLoader.LoadPuzzle(FreeCatPuzzle, false, false, false);
                    SetPuzzleInteractable(false);
                    Camera.transform.position = new Vector3(0.0f, 8.0f, -8.0f);
                    Camera.orthographicSize = 5.0f;
                    LeanTween
                        .moveLocalY(Camera.gameObject, 1.0f, 2.0f)
                        .setEase(LeanTweenType.easeInOutSine)
                        .setDelay(4.0f);
                    break;
                case TutorialSceneState.FreeingCat:
                    SetPuzzleInteractable(true);
                    FreeCatPuzzle.onBlockMoved += OnFreeCatPuzzleBlockMoved;
                    break;
                case TutorialSceneState.FreeingCatWithHelper:
                    break;
                case TutorialSceneState.MeetingCat:
                    SetPuzzleInteractable(false);
                    // TODO: Maybe some hyped up bright light animation stuff.
                    _gameManager.Audio.PlaySoundEffect(FreeChatCatSoundEffect);
                    _gameManager.Audio.PlayBackgroundMusic(null);
                    _catAvatar = Instantiate(TutorialCatAvatarPrefab, transform);
                    _catAvatar.transform.localScale = Vector3.one * 0.5f;
                    LeanTween
                        .scale(_catAvatar.gameObject, Vector3.one, 1.0f)
                        .setEase(LeanTweenType.easeOutQuint);
                    RarityEffect rarityEffect = Instantiate(
                        FreeCatPuzzle.RarityEffectPrefab,
                        transform
                    );
                    rarityEffect.Cat = ChatCat;
                    rarityEffect.transform.position = FreeCatPuzzle.transform.position;
                    _catAvatar.Initialize();
                    _catAvatar.TutorialAnimator.PlayMeetingCat();
                    _catAvatar.Cat = ChatCat;
                    _catAvatar.Flash(Color.white, 3.0f);
                    break;
                case TutorialSceneState.Zoom1:
                    _gameManager.Audio.PlaySoundEffect(ZoomInOnChatCatSoundEffect);
                    Camera.orthographicSize = 3.5f;
                    break;
                case TutorialSceneState.Zoom2:
                    _gameManager.Audio.PlaySoundEffect(ZoomInOnChatCatSoundEffect);
                    Camera.transform.position = new Vector3(0.0f, 0.9f, -8.0f);
                    Camera.orthographicSize = 2.0f;
                    break;
                case TutorialSceneState.Zoom3:
                    _gameManager.Audio.PlaySoundEffect(ZoomInOnChatCatSoundEffect);
                    Camera.transform.position = new Vector3(0.0f, 0.8f, -8.0f);
                    Camera.orthographicSize = 0.75f;
                    break;
                case TutorialSceneState.BondingWithCat:
                    _gameManager.Audio.PlaySoundEffect(OpenBondingUISoundEffect);
                    if (!_gameManager.Cats.WasCatSeen(ChatCat))
                    {
                        CatReward reward = _gameManager.Cats.AddCatSighting(ChatCat);
                        _gameManager.Stuff.AddPresent(ChatCat, reward.Currency, reward.Items);
                    }
                    SetPuzzleInteractable(false);
                    break;
                case TutorialSceneState.ChatIntroductions:
                    // TODO?: _catAvatar.TutorialAnimator.PlayChatIntroductions();
                    LeanTween
                        .value(Camera.gameObject, Camera.orthographicSize, 2.0f, 2.0f)
                        .setEase(LeanTweenType.easeInOutSine)
                        .setOnUpdate(
                            (float newOrthoSize) =>
                            {
                                Camera.orthographicSize = newOrthoSize;
                            }
                        );
                    _gameManager.Audio.PlayBackgroundMusic(TutorialBackgroundMusic);
                    _gameManager.ChatCat.Say("You must be my new human!", ChatCatEmote.Surprised);
                    _gameManager.ChatCat.Say(
                        "Wow that's a lot of responsibility...",
                        ChatCatEmote.Thinking
                    );
                    _gameManager.ChatCat.Say("...", ChatCatEmote.Thinking, 0.5f);
                    _gameManager.ChatCat.Say("But I think I can handle it!", ChatCatEmote.Heart);
                    break;
                case TutorialSceneState.NoticingHouse:
                    _catAvatar.TutorialAnimator.PlayNoticingHouse();
                    LeanTween
                        .value(Camera.gameObject, Camera.orthographicSize, 3.0f, 2.0f)
                        .setEase(LeanTweenType.easeInOutSine)
                        .setOnUpdate(
                            (float newOrthoSize) =>
                            {
                                Camera.orthographicSize = newOrthoSize;
                            }
                        );
                    break;
                case TutorialSceneState.ChatAboutHouse:
                    _gameManager.ChatCat.Say("Oh!", ChatCatEmote.Surprised);
                    _gameManager.ChatCat.Say("Is that your house?", ChatCatEmote.Surprised);
                    break;
                case TutorialSceneState.MovingToBlockStackingPuzzle:
                    // TODO?: _catAvatar.TutorialAnimator.PlayMovingToBlockStackingPuzzle();
                    StairsPuzzle.gameObject.SetActive(true);
                    Camera.transform.position = new Vector3(0.0f, 0.8f, -8.0f);
                    LeanTween
                        .moveLocal(Camera.gameObject, new Vector3(10.0f, 1.5f, -8.0f), 2.0f)
                        .setEase(LeanTweenType.easeInOutSine);
                    LeanTween
                        .value(Camera.gameObject, Camera.orthographicSize, 6.5f, 2.0f)
                        .setEase(LeanTweenType.easeInOutSine)
                        .setOnUpdate(
                            (float newOrthoSize) =>
                            {
                                Camera.orthographicSize = newOrthoSize;
                            }
                        );
                    break;
                case TutorialSceneState.ChatAboutStairs:
                    _gameManager.ChatCat.Say(
                        "I didn't know you came with one of those!",
                        ChatCatEmote.Surprised
                    );
                    _gameManager.ChatCat.Say("Help me inside, would you?");
                    _gameManager.ChatCat.Say(
                        "Maybe you could make a staircase for me with those blocks!",
                        ChatCatEmote.Thinking
                    );
                    break;
                case TutorialSceneState.UnloadingPreviousPuzzle:
                    _puzzleLoader.onPuzzleUnloaded += OnFirstPuzzleUnloaded;
                    _puzzleLoader.UnloadPuzzle();
                    break;
                case TutorialSceneState.BuildingStairs:
                    SetPuzzleInteractable(true);
                    StairsPuzzle.onBlockMoved += OnStairsPuzzleBlockMoved;
                    break;
                case TutorialSceneState.HeadingHome:
                    _catAvatar.TutorialAnimator.PlayHeadingHome();
                    SetPuzzleInteractable(false);
                    break;
                case TutorialSceneState.Completed:
                    _gameManager.Data.TutorialData.TutorialState = TutorialSceneState.Completed;
                    _gameManager.GoHome();
                    break;
            }

            if (onEnterState != null)
                onEnterState(_state);
        }

        private void UpdateCatBlockRustling()
        {
            if (Time.time > _nextRustleTime)
            {
                int i;
                for (i = 0; i < UnityEngine.Random.Range(1, 4); i++)
                {
                    int direction = UnityEngine.Random.value >= 0.5f ? -1 : 1;
                    LeanTween
                        .moveLocalX(_chatCatBlock.gameObject, 0.05f * direction, 0.3f)
                        .setEase(LeanTweenType.punch)
                        .setDelay(i * 0.3f)
                        .setOnStart(
                            delegate()
                            {
                                _gameManager.Audio.PlaySoundEffect(ThumpSoundEffect);
                                if (onBoxThumped != null)
                                    onBoxThumped(_chatCatBlock.transform.position, direction);
                            }
                        );
                }
                _nextRustleTime = Time.time + i * 0.3f + UnityEngine.Random.Range(0.5f, 2.0f);
            }
        }

        private void OnFreeCatPuzzleBlockMoved(
            Puzzle puzzle,
            Stack source,
            Block block,
            Stack destination
        )
        {
            FreeCatPuzzle.onBlockMoved -= OnFreeCatPuzzleBlockMoved;
            ChangeState(TutorialSceneState.MeetingCat);
        }

        private void OnStairsPuzzleBlockMoved(
            Puzzle puzzle,
            Stack source,
            Block block,
            Stack destination
        )
        {
            if (
                puzzle.Stacks[0].Blocks.Count == 1
                && puzzle.Stacks[1].Blocks.Count == 2
                && puzzle.Stacks[2].Blocks.Count == 3
            )
            {
                puzzle.CompletePuzzle(PuzzleCompletionType.PuzzleSolved);
                PuzzleFlasher.FlashPuzzle(puzzle);
                StairsPuzzle.onBlockMoved -= OnFreeCatPuzzleBlockMoved;
                _gameManager.CutScenes.EnqueueCutScene(EnterChatCatCutScene);
                ChangeState(TutorialSceneState.HeadingHome);
            }
        }

        private void OnTutorialPuzzleLoaded(Puzzle puzzle)
        {
            PointerPuzzleController controller =
                puzzle.gameObject.AddComponent<PointerPuzzleController>();
            controller.Camera = Camera;
            controller.enabled = _isPuzzleInteractable;
        }

        private void OnFirstPuzzleUnloaded(Puzzle puzzle)
        {
            _puzzleLoader.onPuzzleUnloaded -= OnFirstPuzzleUnloaded;
            _puzzleLoader.LoadPuzzle(StairsPuzzle, false, false, false);
            ChangeState(TutorialSceneState.BuildingStairs);
        }
    }
}
