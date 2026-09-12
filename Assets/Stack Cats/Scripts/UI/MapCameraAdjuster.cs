using UnityEngine;

namespace Tofuwu.StackCats
{
    public class MapCameraAdjuster : MonoBehaviour
    {
        private class AspectSettings
        {
            public float OrthographicSize { get; set; }
            public float CameraOffsetX { get; set; }
        }

        public MapScene MapScene;
        public RectTransform HeaderRectTransform;
        public RectTransform FooterRectTransform;
        public RectTransform LeftPanelRectTransorm;
        public LeanTweenType AspectTransitionTweenType = LeanTweenType.easeInOutSine;
        public float AspectTransitionDuration = 1.0f;

        private AspectSettings _currentAspectSettings;
        private Vector2 _screenSize;
        private float _mapWidth;
        private float _mapHeight;

        protected void Awake()
        {
            _mapWidth = MapScene.UsableAreaMapWidth;
            _mapHeight = MapScene.UsableAreaMapHeight;
        }

        protected void Start()
        {
            FitCameraAndUIToMapAspect();
        }

        protected void Update()
        {
            if (MapScene.IsViewingMap)
            {
                FitCameraAndUIToMapAspect();
            }
        }

        private void FitCameraAndUIToAspect(AspectSettings aspectSettings)
        {
            _currentAspectSettings = aspectSettings;

            float cameraY = MapScene.Camera.transform.position.y;

            MapScene.Camera.orthographicSize = _currentAspectSettings.OrthographicSize;
            MapScene.Camera.transform.position = new Vector2(_currentAspectSettings.CameraOffsetX, cameraY);
        }

        public void FitCameraAndUIToMapAspect(Puzzle puzzle = null)
        {
            bool screenSizeChanged = Screen.width != _screenSize.x || Screen.height != _screenSize.y;
            _screenSize = new Vector2(Screen.width, Screen.height);

            if (!screenSizeChanged && _currentAspectSettings != null) return;

            float minHeaderYPercent = 1.0f - HeaderRectTransform.anchorMin.y;
            float minFooterYPercent = FooterRectTransform.anchorMax.y;
            float minLeftPanelXPercent = LeftPanelRectTransorm.anchorMax.x;
            float cameraAspect = MapScene.Camera.aspect;
            float mapAspect = _mapWidth / _mapHeight;
            float maxUsableCameraAspect = (_screenSize.x * (1.0f - minLeftPanelXPercent)) / (_screenSize.y * (1.0f - minHeaderYPercent - minFooterYPercent));
            float orthographicSize;

            if (mapAspect <= maxUsableCameraAspect)
            {
                orthographicSize = (_mapWidth + _mapWidth * minLeftPanelXPercent) / 2 / mapAspect / cameraAspect * maxUsableCameraAspect;
            }
            else
            {
                float addedPercent = (_screenSize.x / maxUsableCameraAspect - _screenSize.x / mapAspect) / _screenSize.y;
                if (addedPercent < 0) addedPercent = 0;
                orthographicSize = (_mapWidth + _mapWidth * minLeftPanelXPercent) / 2 / cameraAspect;
            }
            float cameraOffsetX = -orthographicSize * minLeftPanelXPercent / 2;

            AspectSettings newAspectSettings = new AspectSettings
            {
                OrthographicSize = orthographicSize,
                CameraOffsetX = cameraOffsetX
                //CameraPosition = Vector3.up * (puzzleHeight - 0.5f) / 2,
            };

            FitCameraAndUIToAspect(newAspectSettings);
        }
    }
}