using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class BlockTutorialOverlayScreen : TutorialOverlayScreen
    {
        public RectTransform PuzzleDemoRectTransform;
        public PuzzleDemo PuzzleDemoPrefab;
        public PuzzleDemoUI PuzzleDemoUIPrefab;
        public TextMeshProUGUI BlockInformationText;

        private BlockTutorial _blockTutorial;
        private PointerGestureUI _pointerGestureUI;
        private PuzzleDemo _puzzleDemoInstance;
        private PuzzleDemoUI _puzzleDemoUIInstance;

        public void Initialize(BlockTutorial blockTutorial, PointerGestureUI pointerGestureUI)
        {
            base.Initialize(blockTutorial);

            _blockTutorial = blockTutorial;
            _pointerGestureUI = pointerGestureUI;
        }

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            if (!string.IsNullOrEmpty(_blockTutorial.DemoPuzzleJsonData))
            {
                _puzzleDemoInstance = Instantiate(PuzzleDemoPrefab, PuzzleDemoRectTransform);
                _puzzleDemoInstance.PuzzleJsonData = _blockTutorial.DemoPuzzleJsonData;
                _puzzleDemoInstance.DemoMoves = _blockTutorial.DemoMoves;

                //foreach (Renderer renderer in _puzzleDemoInstance.GetComponents<Renderer>())
                //{
                //    Color rendererColor = renderer.material.color;
                //    rendererColor.a = 0.0f;
                //    renderer.material.color = rendererColor;
                //}
                //LeanTween.alpha(_puzzleDemoInstance.gameObject, 1.0f, 1.0f).setEase(LeanTweenType.easeOutSine);

                _puzzleDemoUIInstance = Instantiate(PuzzleDemoUIPrefab, transform);
                _puzzleDemoUIInstance.PuzzleDemo = _puzzleDemoInstance;
                _puzzleDemoUIInstance.PointerGesture = _pointerGestureUI;

                if (!string.IsNullOrWhiteSpace(_blockTutorial.BlockInformation))
                {
                    BlockInformationText.text = _blockTutorial.BlockInformation;
                }
            }
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            Destroy(_puzzleDemoUIInstance.gameObject);
            Destroy(_puzzleDemoInstance.gameObject);
            _pointerGestureUI.SetVisible(false, false);
        }
    }
}
