using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleMakerDataOverlayScreen : OverlayScreen
    {
        public PuzzleMakerScene PuzzleMakerScene;
        public Button CancelButton;
        public Button ScreenButton;

        public void NewPuzzle()
        {
            Dismiss();
            PuzzleMakerScene.NewPuzzle();
        }

        protected void Awake()
        {
            CancelButton.onClick.AddListener(Dismiss);
            ScreenButton.onClick.AddListener(Dismiss);
        }
    }
}