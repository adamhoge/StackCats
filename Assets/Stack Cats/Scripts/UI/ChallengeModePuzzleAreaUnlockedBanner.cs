using TMPro;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class ChallengeModePuzzleAreaUnlockedBanner : BannerUI
    {
        public PuzzleArea PuzzleArea;
        public Image BannerBackgroundImage;
        public TextMeshProUGUI BannerMessageText;
        public Button BannerButton;
        public AudioEvent BannerStartSound;

        private GameManager _gameManager;

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
        }

        protected void Start()
        {
            BannerBackgroundImage.color = PuzzleArea.PuzzleTheme.UIColor;
            BannerMessageText.text = PuzzleArea.AreaTitle + " Challenge Mode Unlocked!";
            if(BannerStartSound) _gameManager.Audio.PlaySoundEffect(BannerStartSound);
        }
    }
}