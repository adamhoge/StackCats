using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleMakerDesertFlavoredActionsUI : PuzzleMakerActionsUI
    {
        public Button FitToPuzzleButton;

        private PuzzleMakerDesertFlavoredActions _puzzleMakerDesertFlavoredActions;

        protected override void OnEnable()
        {
            base.OnEnable();

            FitToPuzzleButton.onClick.AddListener(OnFitToPuzzle);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            FitToPuzzleButton.onClick.AddListener(OnFitToPuzzle);
        }

        protected override void Start()
        {
            base.Start();

            _puzzleMakerDesertFlavoredActions = (PuzzleMakerDesertFlavoredActions)PuzzleMakerActions;
        }

        private void OnFitToPuzzle()
        {
            _puzzleMakerDesertFlavoredActions.FitStackHeightRequirementsToPuzzle();
        }
    }
}