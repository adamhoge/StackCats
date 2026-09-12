using UnityEngine;
using UnityEngine.EventSystems;

namespace Tofuwu.StackCats
{
    [RequireComponent(typeof(MapScene))]
    public class MapSceneController : MonoBehaviour
    {
        /// <summary>
        /// Drag sensitivity in pixels-to-activate
        /// </summary>
        [Tooltip("Drag sensitivity in pixels-to-activate")]
        public float DragSensitivity = 20.0f;

        /// <summary>
        /// Duration of transitions from one map to another
        /// </summary>
        [Tooltip("Duration of transitions from one map to another")]
        public float MapTransitionDuration = 0.75f;

        /// <summary>
        /// The delay before repeating an axis command
        /// </summary>
        [Tooltip("The delay before repeating an axis command")]
        public float AxisControllerRepeatDelay = 0.75f;

        private MapScene _mapScene;
        private BoxCollider2D _boxCollider;
        private float _dragYStart;
        private float _dragYDistance;
        private bool _isDragDisabled;
        private float _lastMapNavigationTime;

        private void OnMouseDown()
        {
            if (!enabled) return;

            _dragYStart = Input.mousePosition.y;
        }

        private void OnMouseDrag()
        {
            if (!enabled || _isDragDisabled) return;

            _dragYDistance = Input.mousePosition.y - _dragYStart;

            if (Mathf.Abs(_dragYDistance) > DragSensitivity)
            {
                if (_dragYDistance < 0)
                {
                    _mapScene.ViewNextPuzzleArea();
                }
                else
                {
                    _mapScene.ViewPreviousPuzzleArea();
                }

                _isDragDisabled = true;
            }
        }

        private void OnMouseUp()
        {
            if (!enabled) return;

            _isDragDisabled = false;
        }

        protected void Awake()
        {
            _mapScene = GetComponent<MapScene>();
            _boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        protected void Start()
        {
            _boxCollider.size = _mapScene.FullMapSize;
            _boxCollider.offset = Vector2.up * (_mapScene.FullMapSize.y / 2 - _mapScene.UsableAreaMapHeight / 2);
            _lastMapNavigationTime = -AxisControllerRepeatDelay;
        }

        protected void Update()
        {
            if (Time.time > _lastMapNavigationTime + AxisControllerRepeatDelay)
            {
                if (Input.GetAxis("Vertical") > 0 || Input.mouseScrollDelta.y > 0)
                {
                    _mapScene.ViewNextPuzzleArea();
                    _lastMapNavigationTime = Time.time;
                }
                else if (Input.GetAxis("Vertical") < 0 || Input.mouseScrollDelta.y < 0)
                {
                    _mapScene.ViewPreviousPuzzleArea();
                    _lastMapNavigationTime = Time.time;
                }
            }
        }
    }
}