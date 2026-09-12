using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class EndlessStartOverlayScreen : OverlayScreen
    {
        public EndlessRunScene EndlessRunScene;
        public RectTransform HighScoreRectTransform;
        public TextMeshProUGUI HighScoreText;
        public MedalUI Medal;
        public Button StartButton;

        private PuzzleManager _puzzleManager;

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void OnEnable()
        {
            StartButton.onClick.AddListener(OnStartButtonClicked);
        }

        protected void OnDisable()
        {
            StartButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            int highScore = _puzzleManager.GetEndlessRunHighScore(EndlessRunScene.PuzzleArea);
            if(highScore == 0)
            {
                HighScoreRectTransform.gameObject.SetActive(false);
            }
            else
            {
                HighScoreRectTransform.gameObject.SetActive(true);
                HighScoreText.text = highScore.ToString();
                Medal.NumPuzzlesCompleted = _puzzleManager.GetEndlessRunMostPuzzlesCompleted(EndlessRunScene.PuzzleArea);
            }
        }

        private void OnStartButtonClicked()
        {
            EndlessRunScene.StartEndlessRun();
            Dismiss();
        }
    }
}