using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class PuzzleCameraAdjuster : MonoBehaviour
    {
        private class AspectSettings
        {
            public float PuzzleWidth { get; set; }
            public float PuzzleHeight { get; set; }
            public float OrthographicSize { get; set; }
            public Vector3 CameraPosition { get; set; }
        }

        public Camera Camera;
        public RectTransform HeaderRectTransform;
        public RectTransform FooterRectTransform;
        public float DefaultPuzzleWidth = 5.9f;
        public float DefaultPuzzleHeight = 8.5f;
        public LeanTweenType AspectTransitionTweenType = LeanTweenType.easeInOutSine;
        public float AspectTransitionDuration = 1.0f;
        public float MinOrthographicSize = 4.5f;
        public bool FitToUIAndMenuOnAwake = true;

        private AspectSettings _currentAspectSettings;
        private Vector2 _screenSize;

        protected void Awake()
        {
            if (FitToUIAndMenuOnAwake)
            {
                FitCameraAndUIToMenu();
            }
            else
            {
                FitCameraAndUIToPuzzleAspect();
            }
        }

        private void FitCameraAndUIToAspect(AspectSettings aspectSettings)
        {
            if (aspectSettings.OrthographicSize < MinOrthographicSize)
            {
                aspectSettings.OrthographicSize = MinOrthographicSize;
            }

            if (_currentAspectSettings == null || AspectTransitionDuration == 0.0f)
            {
                _currentAspectSettings = aspectSettings;

                Camera.orthographicSize = _currentAspectSettings.OrthographicSize;
                Camera.transform.position = _currentAspectSettings.CameraPosition;
            }
            else
            {
                _currentAspectSettings = aspectSettings;

                LeanTween.cancel(Camera.gameObject);

                LeanTween.value(Camera.gameObject, Camera.orthographicSize, _currentAspectSettings.OrthographicSize, AspectTransitionDuration).setEase(AspectTransitionTweenType).setOnUpdate(
                    (float newOrthoSize) => { Camera.orthographicSize = newOrthoSize; });
                LeanTween.moveLocal(Camera.gameObject, _currentAspectSettings.CameraPosition, AspectTransitionDuration).setEase(AspectTransitionTweenType);
            }
        }

        public void FitCameraAndUIToMenu()
        {
            _screenSize = new Vector2(Screen.width, Screen.height);

            AspectSettings newAspectSettings = new AspectSettings
            {
                OrthographicSize = 6.75f,
                CameraPosition = Vector3.up * 5.0f,
            };

            FitCameraAndUIToAspect(newAspectSettings);
        }

        public void FitCameraAndUIToPuzzleAspect(Puzzle puzzle = null)
        {
            bool screenSizeChanged = Screen.width != _screenSize.x || Screen.height != _screenSize.y;
            _screenSize = new Vector2(Screen.width, Screen.height);

            float puzzleWidth = puzzle ? puzzle.Stacks.Count * (1.0f + puzzle.StackSpacing) + puzzle.StackSpacing : DefaultPuzzleWidth;
            puzzleWidth += 0.5f;
            float puzzleHeight = puzzle ? puzzle.MaxStackHeight + 0.5f : DefaultPuzzleHeight;
            puzzleHeight += 0.5f;

            if (!screenSizeChanged && _currentAspectSettings != null && _currentAspectSettings.PuzzleWidth == puzzleWidth && _currentAspectSettings.PuzzleHeight == puzzleHeight) return;

            float minHeaderYPercent = 1.0f - HeaderRectTransform.anchorMin.y;
            float minFooterYPercent = FooterRectTransform.anchorMax.y;
            float cameraAspect = Camera.aspect;
            float puzzleAspect = puzzleWidth / puzzleHeight;
            float maxUsableCameraAspect = _screenSize.x / (_screenSize.y * (1.0f - minHeaderYPercent - minFooterYPercent));
            float orthographicSize;
            float topBoundaryMin;
            float bottomBoundaryMax;
            if (puzzleAspect <= maxUsableCameraAspect)
            {
                orthographicSize = puzzleWidth / 2 / puzzleAspect / cameraAspect * maxUsableCameraAspect;
                topBoundaryMin = minHeaderYPercent;
                bottomBoundaryMax = minFooterYPercent;
            }
            else
            {
                float addedPercent = (_screenSize.x / maxUsableCameraAspect - _screenSize.x / puzzleAspect) / _screenSize.y;
                if (addedPercent < 0) addedPercent = 0;
                topBoundaryMin = minHeaderYPercent + addedPercent / 2;
                bottomBoundaryMax = minFooterYPercent + addedPercent / 2;
                orthographicSize = puzzleWidth / 2 / cameraAspect;
            }

            AspectSettings newAspectSettings = new AspectSettings
            {
                PuzzleWidth = puzzleWidth,
                PuzzleHeight = puzzleHeight,
                OrthographicSize = orthographicSize,
                CameraPosition = Vector3.up * (puzzleHeight - 0.7f) / 2,
            };

            FitCameraAndUIToAspect(newAspectSettings);
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted)
        {
            FitCameraAndUIToPuzzleAspect(puzzle);
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            FitCameraAndUIToMenu();
        }
    }
}