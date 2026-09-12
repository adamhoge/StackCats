using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class AxisControllerUI : MonoBehaviour
    {
        public Image MarkerPrefab;
        public Canvas WorldCanvas;
        public float MarkerFlashSpeed = 1.0f;
        public Color MovableBlockMarkerColor = Color.white;
        public Color ImmovableBlockMarkerColor = Color.red;

        public PuzzleController PuzzleController { get { return _puzzleController; } set { SetPuzzleController(value); } }

        [SerializeField]
        [HideInInspector]
        private PuzzleController _puzzleController;

        private Image _marker;
        private float _markerAlpha;
        private GameObject _placementPreview = null;

        protected void Awake()
        {
            if (_puzzleController)
            {
                _puzzleController.onFocused += OnFocus;
                _puzzleController.onChecked += OnCheck;
            }
        }

        protected void Update()
        {
            if (_marker != null && MarkerFlashSpeed != 0)
            {
                _markerAlpha = (_markerAlpha + Time.deltaTime) % (MarkerFlashSpeed * 2);
                float alpha = Mathf.Abs(-MarkerFlashSpeed + _markerAlpha) / MarkerFlashSpeed * 0.25f + 0.75f;
                _marker.color = new Color(_marker.color.r, _marker.color.g, _marker.color.b, alpha);
            }
        }

        private void SetPuzzleController(PuzzleController value)
        {
            if (_puzzleController)
            {
                _puzzleController.onFocused -= OnFocus;
                _puzzleController.onChecked -= OnCheck;
                if (_marker)
                {
                    Destroy(_marker.gameObject);
                    _marker = null;
                }
            }

            _puzzleController = value;

            if (_puzzleController)
            {
                _puzzleController.onFocused += OnFocus;
                _puzzleController.onChecked += OnCheck;
            }
        }

        private void OnFocus(PuzzleMarker marker, bool isMovable)
        {
            if (marker == null)
            {
                if (_marker) Destroy(_marker.gameObject);
                return;
            }

            PuzzleMarker selection = _puzzleController.Selection;

            if (selection == null)
            {
                if (!_marker && MarkerPrefab && WorldCanvas)
                {
                    _marker = Instantiate(MarkerPrefab, WorldCanvas.transform);
                }

                if (_marker)
                {
                    _marker.transform.position = marker.Block.transform.position;
                    _marker.transform.localScale = marker.Block.transform.localScale;
                    _marker.color = isMovable ? MovableBlockMarkerColor : ImmovableBlockMarkerColor;
                }
            }
        }

        private void OnCheck(PuzzleMarker source, PuzzleMarker marker, int numBlocks, bool isValid)
        {
            if (_placementPreview) Destroy(_placementPreview.gameObject);

            //else if (_puzzleController.Puzzle.IsPlaceable(_puzzleController.Selection.Block, marker.Stack.TopBlock))
            //{
            //    _placementPreview = new GameObject("Placement Preview");
            //    foreach (Block block in _puzzleController.SelectedBlocks)
            //    {
            //        GameObject clonedSpriteRenderers = GameObjectExtensions.CloneComponentHeirarchy<SpriteRenderer>(block.gameObject);
            //        if (clonedSpriteRenderers)
            //        {
            //            clonedSpriteRenderers.transform.SetParent(_placementPreview.transform, false);
            //        }
            //    }
            //}

            if (isValid /* || marker.Stack == _puzzleController.Selection.Stack*/)
            {
                foreach (Block block in _puzzleController.SelectedBlocks)
                {
                }
            }
        }
    }
}