using UnityEngine;
using UnityEngine.Events;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(BannerManagerUI))]
    [RequireComponent(typeof(PuzzleManagerUI))]
    public class GameManagerUI : MonoBehaviour
    {
        public OverlayScreenManager OverlayScreenManager;
        public ConfirmationOverlayScreen ConfirmationOverlayScreen;

        /// <summary>
        /// An instance of the GameManagerUI.
        /// </summary>
        public static GameManagerUI Instance { get { return _instance; } }

        public BannerManagerUI BannerManagerUI { get { return _bannerManagerUI; } }

        public PuzzleManagerUI PuzzleManagerUI { get { return _puzzleManagerUI; } }

        private static GameManagerUI _instance;
        private GameManager _gameManager;
        private BannerManagerUI _bannerManagerUI;
        private PuzzleManagerUI _puzzleManagerUI;

        protected void Awake()
        {
            if (!_instance)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                _gameManager = GameManager.Instance;
                _gameManager.onConfirmAction += OnConfirmAction;
                _bannerManagerUI = GetComponent<BannerManagerUI>();
                _puzzleManagerUI = GetComponent<PuzzleManagerUI>();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnConfirmAction(string message, UnityAction action)
        {
            ConfirmationOverlayScreen.ConfirmationAction = action;
            ConfirmationOverlayScreen.MessageText.text = message;
            ConfirmationOverlayScreen.ConfirmationText.text = action != null ? "Confirm" : "Got it!";
            ConfirmationOverlayScreen.CancelButton.gameObject.SetActive(action != null);
            OverlayScreenManager.EnqueueScreen(ConfirmationOverlayScreen);
        }
    }
}