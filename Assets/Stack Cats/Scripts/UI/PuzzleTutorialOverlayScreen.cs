using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleTutorialOverlayScreen : TutorialOverlayScreen
    {
        public RectTransform PuzzleDemoRectTransform;
        public PuzzleDemo PuzzleDemoPrefab;
        public PuzzleDemoUI PuzzleDemoUIPrefab;
        public TextMeshProUGUI PuzzleAreaText;
        public TextMeshProUGUI ObjectiveSummaryText;
        public TextMeshProUGUI ObjectiveDetailsText;

        private PuzzleTutorial _puzzleTutorial;
        private PointerGestureUI _pointerGestureUI;
        private PuzzleDemo _puzzleDemoInstance;
        private PuzzleDemoUI _puzzleDemoUIInstance;
        public void Initialize(PuzzleTutorial puzzleTutorial, PointerGestureUI pointerGestureUI)
        {
            base.Initialize(puzzleTutorial);

            _puzzleTutorial = puzzleTutorial;
            _pointerGestureUI = pointerGestureUI;
        }

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            if (!string.IsNullOrEmpty(_puzzleTutorial.DemoPuzzleJsonData))
            {
                _puzzleDemoInstance = Instantiate(PuzzleDemoPrefab, PuzzleDemoRectTransform);
                _puzzleDemoInstance.PuzzleArea = _puzzleTutorial.PuzzleArea;
                _puzzleDemoInstance.PuzzleJsonData = _puzzleTutorial.DemoPuzzleJsonData;
                _puzzleDemoInstance.DemoMoves = _puzzleTutorial.DemoMoves;

                _puzzleDemoUIInstance = Instantiate(PuzzleDemoUIPrefab, transform);
                _puzzleDemoUIInstance.PuzzleDemo = _puzzleDemoInstance;
                _puzzleDemoUIInstance.PointerGesture = _pointerGestureUI;

                PuzzleAreaText.text = _puzzleTutorial.PuzzleArea.AreaTitle;

                if (!string.IsNullOrWhiteSpace(_puzzleTutorial.ObjectiveSummary))
                {
                    ObjectiveSummaryText.text = _puzzleTutorial.ObjectiveSummary;
                }

                if (!string.IsNullOrWhiteSpace(_puzzleTutorial.ObjectiveSummary))
                {
                    ObjectiveDetailsText.text = _puzzleTutorial.ObjectiveDetails;
                }
            }
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            if (_puzzleDemoUIInstance)
            {
                Destroy(_puzzleDemoUIInstance.gameObject);
            }

            if (_puzzleDemoInstance)
            {
                Destroy(_puzzleDemoInstance.gameObject);
            }

            _pointerGestureUI.SetVisible(false, false);
        }
    }
}
