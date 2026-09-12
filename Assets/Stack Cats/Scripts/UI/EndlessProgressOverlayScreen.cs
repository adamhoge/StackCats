using System.Collections.Generic;
using Tofuwu.StackCats.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class EndlessProgressOverlayScreen : OverlayScreen
    {
        private enum State
        {
            NotStarted,
            DisplayingScoreChange,
            DisplayingCurrentScore
        }

        public EndlessRunScene EndlessRunScene;
        public Image BackgroundImage;
        public RectTransform BestScoreRectTransform;
        public Image BestScoreBackgroundImage;
        public Button PlayPuzzleButton;
        public Button EndRunButton;
        public MedalUI Medal;
        public MedalFlashEffectUI MedalFlashEffect;
        public AudioEvent MedalFlashAudioEvent;
        public TextMeshProUGUI ScoreText;
        public MedalUI BestMedal;
        public TextMeshProUGUI BestScoreText;
        public float DisplayScoreIncrementInterval = 0.05f;
        public float DisplayScoreChangeDelayInSeconds = 1.0f;

        private State _currentState;
        private float _stateEnterTime;
        private PuzzleManager _puzzleManager;
        private EndlessRunModel _currentEndlessRun;
        private int _currentScore;
        private int _displayedScore;
        private bool _isHighScore;
        private int _previousHighScore;
        private float _nextScoreIncrementTime;
        private bool _shouldIncrementMedal;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            _currentEndlessRun = EndlessRunScene.CurrentEndlessRun;
            _currentState = State.NotStarted;

            bool isStarted = _currentEndlessRun.CurrentPuzzle.MovesMade > 0;

            PlayPuzzleButton.GetComponentInChildren<TextMeshProUGUI>().text = isStarted ? "Resume Puzzle" : "Start Puzzle";
            if (EndlessRunScene.PuzzleArea)
            {
                var puzzleTheme = EndlessRunScene.PuzzleArea.PuzzleTheme;
                BackgroundImage.gameObject.SetActive(true);
                BackgroundImage.color = puzzleTheme.UIColor;
                var hsbColor = HSBColor.FromColor(puzzleTheme.UIColor);
                hsbColor.b = 0.66f;
                BestMedal.MedalOutlineImage.color = hsbColor.ToColor();
                BestScoreBackgroundImage.color = puzzleTheme.UIColor;
                PlayPuzzleButton.image.color = puzzleTheme.UIColor;
            }
            else
            {
                BackgroundImage.gameObject.SetActive(false);
                PlayPuzzleButton.image.color = Color.white;
            }
            UpdatePuzzleCompletion();

            _puzzleManager.OneOffEndlessPuzzleCompleted.ConsumeAll(OnEndlessPuzzleCompleted);
        }

        public override void OnActive()
        {
            base.OnActive();
        }

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void OnEnable()
        {
            EndRunButton.onClick.AddListener(OnEndRunButtonClicked);
            EndlessRunScene.onStateChanged += OnEndlessRunSceneStateChanged;
        }

        protected void OnDisable()
        {
            EndRunButton.onClick.RemoveListener(OnEndRunButtonClicked);
            EndlessRunScene.onStateChanged -= OnEndlessRunSceneStateChanged;
        }

        protected override void Update()
        {
            base.Update();

            if (_currentState == State.DisplayingScoreChange && _stateEnterTime + DisplayScoreChangeDelayInSeconds < Time.time)
            {
                if (_nextScoreIncrementTime <= Time.time)
                {
                    if (_displayedScore < _currentScore)
                    {
                        ++_displayedScore;
                        ScoreText.text = _displayedScore.ToString();
                        _nextScoreIncrementTime = Time.time + DisplayScoreIncrementInterval;

                        if (_isHighScore && _displayedScore > _previousHighScore)
                        {
                            BestScoreText.text = _displayedScore.ToString();
                        }
                    }
                    else
                    {
                        ChangeState(State.DisplayingCurrentScore);
                    }
                }

                if (_shouldIncrementMedal)
                {
                    Medal.NumPuzzlesCompleted += 1;
                    if (Medal.NumPuzzlesCompleted == 1) Medal.gameObject.SetActive(true);
                    MedalFlashEffect.NumPuzzlesCompleted = Medal.NumPuzzlesCompleted;
                    MedalFlashEffect.Flash();
                    GameManager.Instance.Audio.PlaySoundEffect(MedalFlashAudioEvent);

                    _shouldIncrementMedal = false;
                }
            }
        }

        private void UpdatePuzzleCompletion()
        {
            List<EndlessPuzzleCompletionModel> completedPuzzles = _currentEndlessRun.CompletedPuzzles;
            int numCompletedPuzzles = completedPuzzles.Count;
            if (numCompletedPuzzles == 0)
            {
                Medal.gameObject.SetActive(false);
                ScoreText.gameObject.SetActive(false);
            }
            else
            {
                Medal.gameObject.SetActive(true);
                Medal.NumPuzzlesCompleted = numCompletedPuzzles;
                
                int score = _currentEndlessRun.Score;
                _currentScore = score;
                _isHighScore = _puzzleManager.GetEndlessRunHighScore(EndlessRunScene.PuzzleArea) == score;
                ScoreText.gameObject.SetActive(true);
                ScoreText.text = score == 0 ? "" : score.ToString();
            }

            int bestNumCompletedPuzzles = _puzzleManager.GetEndlessRunMostPuzzlesCompleted(EndlessRunScene.PuzzleArea);
            if (bestNumCompletedPuzzles > 0)
            {
                BestScoreRectTransform.gameObject.SetActive(true);
                BestMedal.NumPuzzlesCompleted = bestNumCompletedPuzzles;
                BestScoreText.text = _puzzleManager.GetEndlessRunHighScore(EndlessRunScene.PuzzleArea).ToString();
            }
            else
            {   
                BestScoreRectTransform.gameObject.SetActive(false);
            }
        }

        private void OnEndRunButtonClicked()
        {
            EndlessRunScene.EndEndlessRun();
        }

        private void OnEndlessRunSceneStateChanged(EndlessRunSceneState state)
        {
            Dismiss();
        }

        private void OnEndlessPuzzleCompleted(PuzzleManager.EndlessPuzzleCompletionEvent completionInfo)
        {
            if (completionInfo.CompletionType != PuzzleCompletionType.PuzzleSolved) return;

            _displayedScore = _currentScore - completionInfo.PointsEarned;
            Medal.NumPuzzlesCompleted = completionInfo.TotalPuzzlesCompleted - 1;
            if (Medal.NumPuzzlesCompleted == 0) Medal.gameObject.SetActive(false);
            _shouldIncrementMedal = true; 
            ScoreText.text = _displayedScore == 0 ? "" : _displayedScore.ToString();

            if (_isHighScore)
            {
                _previousHighScore = _puzzleManager.GetEndlessRunCompletedHighScore(EndlessRunScene.PuzzleArea);
                if (_displayedScore > _previousHighScore) _previousHighScore = _displayedScore;
                BestScoreText.text = _previousHighScore.ToString();
            }

            ChangeState(State.DisplayingScoreChange);
        }

        private void ChangeState(State state)
        {
            if (state == _currentState) return;

            if (_currentState == State.DisplayingScoreChange)
            {
                LeanTween.scale(ScoreText.gameObject, Vector2.one * 1.25f, 1.0f).setEase(LeanTweenType.punch);

                if (_isHighScore)
                {
                    LeanTween.scale(BestScoreText.gameObject, Vector2.one * 1.25f, 1.0f).setEase(LeanTweenType.punch);
                }
            }

            _currentState = state;
            _stateEnterTime = Time.time;

            switch (_currentState)
            {
                case State.NotStarted:
                    break;
                case State.DisplayingScoreChange:
                    break;
                case State.DisplayingCurrentScore:
                    break;
            }
        }
    }
}