using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleControllerUI : MonoBehaviour
    {
        public PuzzleLoader PuzzleLoader;
        public Canvas WorldCanvas;
        public Color StackFocusValidColor;
        public Color StackFocusInvalidColor;

        protected Puzzle _currentPuzzle;

        private PuzzleController _currentPuzzleController;
        private Stack _focusedStack;
        private Image _stackFocusImage;

        protected void Start()
        {
            if (!PuzzleLoader)
            {
                Debug.LogError("PuzzleLoader was not provided for " + gameObject.name + ". Destroying gameObject.");
                Destroy(gameObject);
                return;
            }

            PuzzleLoader.onPuzzleLoaded += OnPuzzleLoaded;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;
        }

        protected virtual void OnPuzzleLoaded(Puzzle puzzle)
        {
            _currentPuzzleController = puzzle.GetComponent<PuzzleController>();
            _currentPuzzle = puzzle;

            if (_currentPuzzleController)
            {
                _currentPuzzleController.onFocused += OnFocused;
                _currentPuzzleController.onChecked += OnChecked;
                _currentPuzzleController.onBlocksSelected += OnBlocksBlocksSelected;
                _currentPuzzleController.onCancelled += OnCancelled;
            }
        }

        protected virtual void OnPuzzleUnloaded(Puzzle puzzle)
        {
            _currentPuzzleController = null;
            _currentPuzzle = null;

            if (_currentPuzzleController)
            {
                _currentPuzzleController.onFocused -= OnFocused;
                _currentPuzzleController.onChecked -= OnChecked;
                _currentPuzzleController.onBlocksSelected -= OnBlocksBlocksSelected;
                _currentPuzzleController.onCancelled -= OnCancelled;
            }
        }

        protected virtual void OnFocused(PuzzleMarker marker, bool isMovable)
        {
            if (marker == null || marker.Stack != _focusedStack)
            {
                RemoveStackFocusImage();
            }
        }

        protected virtual void OnChecked(PuzzleMarker source, PuzzleMarker destination, int numBlocks, bool isValid)
        {
            if (destination.Stack == _focusedStack) return;
            _focusedStack = destination.Stack;

            if (_currentPuzzleController.Selection.Stack == destination.Stack)
            {
                RemoveStackFocusImage();
                return;
            }

            Stack destinationStack = destination.Stack;
            _stackFocusImage = new GameObject("Focused Stack").AddComponent<Image>();
            CanvasGroup canvasGroup = _stackFocusImage.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0.0f;
            _stackFocusImage.transform.SetParent(WorldCanvas.transform);
            _stackFocusImage.transform.position = destinationStack.transform.position + Vector3.forward * 8.0f + (Vector3.up * _currentPuzzle.MaxMovableStackHeight / 2);
            _stackFocusImage.rectTransform.sizeDelta = new Vector2(1 + _currentPuzzle.StackSpacing, _currentPuzzle.MaxMovableStackHeight);
            _stackFocusImage.color = isValid ? StackFocusValidColor : StackFocusInvalidColor;

            LeanTween.alphaCanvas(canvasGroup, 1.0f, 0.5f).setEase(LeanTweenType.easeOutQuint);
        }

        protected virtual void OnBlocksBlocksSelected(PuzzleMarker marker) { }

        protected virtual void OnCancelled(PuzzleMarker marker)
        {
            RemoveStackFocusImage();
            _focusedStack = null;
        }

        protected virtual void RemoveStackFocusImage()
        {
            if (_stackFocusImage)
            {
                LeanTween.cancel(_stackFocusImage.gameObject);
                LeanTween.alphaCanvas(_stackFocusImage.GetComponent<CanvasGroup>(), 0.0f, 0.25f).setEase(LeanTweenType.easeOutQuint);
                Destroy(_stackFocusImage.gameObject, 0.25f);
            }
        }
    }
}