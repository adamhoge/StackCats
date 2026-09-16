using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class TutorialSceneUI : MonoBehaviour
    {
        public Camera Camera;
        public TutorialScene TutorialScene;
        public Canvas WorldCanvas;
        public PointerGestureUI PointerGesture;
        public PointerGestureSequenceUI PointerGestureSequence;
        public CanvasGroup HmmmCanvas;
        public CatSightingUI CatSightingUI;
        public OverlayScreenManager OverlayScreenManager;
        public ChatCatOverlayScreen ChatCatOverlayScreen;
        public CanvasGroup ThumpCaption;
        public CanvasGroup PuzzleBlockRulesCanvasGroup;
        public DynamicAudioEvent BondingProgressAudioEvent;

        private TutorialSceneState _tutorialSceneState;
        private float _timeElapsed;
        private bool _guageStarted;
        private StandardAudioSource _bondingProgressAudioSource;

        protected void OnEnable()
        {
            TutorialScene.onEnterState += OnTutorialSceneEnterState;
            TutorialScene.onBoxThumped += OnBoxThumped;
            CatSightingUI.OnBondingProgressStateChanged += OnBondingProgressStateChanged;
            GameManager.Instance.ChatCat.onTalking += OnTalking;
        }

        protected void OnDisable()
        {
            TutorialScene.onEnterState -= OnTutorialSceneEnterState;
            TutorialScene.onBoxThumped -= OnBoxThumped;
            CatSightingUI.OnBondingProgressStateChanged -= OnBondingProgressStateChanged;
            GameManager.Instance.ChatCat.onTalking -= OnTalking;
        }

        protected void Start()
        {
            Stack freeCatMiddleStack = TutorialScene.FreeCatPuzzle.Stacks[1];
            Vector3 freeCatBlockPosition =
                freeCatMiddleStack.GetBlockLocalPosition(freeCatMiddleStack.Blocks[1])
                + Vector3.up * 0.5f;
            PointerGestureSequence.ActionSequenceItems.Add(
                new PointerGestureSetPositionAction
                {
                    Duration = 0.0f,
                    IsAnimated = false,
                    Position = freeCatBlockPosition,
                }
            );
            PointerGestureSequence.ActionSequenceItems.Add(
                new PointerGestureSetVisibleAction
                {
                    Duration = 0.0f,
                    IsVisible = true,
                    IsAnimated = true,
                }
            );
            PointerGestureSequence.ActionSequenceItems.Add(
                new PointerGestureSetPressedAction
                {
                    Duration = 0.75f,
                    IsPressed = true,
                    IsAnimated = true,
                }
            );
            PointerGestureSequence.ActionSequenceItems.Add(
                new PointerGestureSetPositionAction
                {
                    Duration = 0.5f,
                    IsAnimated = true,
                    Position = freeCatBlockPosition + Vector3.right,
                }
            );
            PointerGestureSequence.ActionSequenceItems.Add(
                new PointerGestureSetVisibleAction
                {
                    Duration = 0.0f,
                    IsVisible = false,
                    IsAnimated = true,
                }
            );
            PointerGestureSequence.ActionSequenceItems.Add(
                new PointerGestureSetPressedAction
                {
                    Duration = 0.5f,
                    IsPressed = false,
                    IsAnimated = true,
                }
            );
            PointerGestureSequence.ActionSequenceItems.Add(
                new PointerGestureSetPressedAction
                {
                    Duration = 0.5f,
                    IsPressed = false,
                    IsAnimated = false,
                }
            );
        }

        protected void Update()
        {
            if (
                _tutorialSceneState == TutorialSceneState.BondingWithCat
                && !_guageStarted
                && Time.time > _timeElapsed + 1
            )
            {
                CatSightingUI.ShowNext();
                _guageStarted = true;
            }
        }

        private void OnTutorialSceneEnterState(TutorialSceneState state)
        {
            switch (_tutorialSceneState)
            {
                case TutorialSceneState.FreeingCatWithHelper:
                    PointerGestureSequence.gameObject.SetActive(false);
                    break;
                case TutorialSceneState.BuildingStairsWithHelper:
                    LeanTween
                        .alphaCanvas(PuzzleBlockRulesCanvasGroup, 0.0f, 0.25f)
                        .setEase(LeanTweenType.easeOutSine);
                    break;
            }

            _tutorialSceneState = state;

            switch (state)
            {
                case TutorialSceneState.HearingNoise:
                    HmmmCanvas.alpha = 0.0f;
                    LeanTween.alphaCanvas(HmmmCanvas, 1.0f, 1.0f).setDelay(2.0f);
                    LeanTween.alphaCanvas(HmmmCanvas, 0.0f, 1.0f).setDelay(4.0f);
                    break;
                case TutorialSceneState.FreeingCatWithHelper:
                    PointerGestureSequence.gameObject.SetActive(true);
                    Stack freeCatMiddleStack = TutorialScene.FreeCatPuzzle.Stacks[1];
                    Vector3 freeCatBlockPosition =
                        freeCatMiddleStack.GetBlockLocalPosition(freeCatMiddleStack.Blocks[1])
                        + Vector3.right * 0.5f;
                    PointerGesture.PointerCanvasGroup.transform.position = freeCatBlockPosition;
                    PointerGesture.PointerCanvasGroup.alpha = 0.0f;
                    PointerGestureSequence.PlaySequence();
                    break;
                case TutorialSceneState.BondingWithCat:
                    CatSightingUI.gameObject.SetActive(true);
                    LeanTween
                        .scale(CatSightingUI.gameObject, Vector3.one * 1.15f, 0.25f)
                        .setEase(LeanTweenType.punch);
                    _timeElapsed = Time.time;
                    break;
                case TutorialSceneState.ChatIntroductions:
                    LeanTween.alphaCanvas(CatSightingUI.CanvasGroup, 0.0f, 0.25f);
                    break;
                case TutorialSceneState.BuildingStairsWithHelper:
                    LeanTween.cancel(PuzzleBlockRulesCanvasGroup.gameObject);
                    LeanTween
                        .alphaCanvas(PuzzleBlockRulesCanvasGroup, 1.0f, 1.0f)
                        .setEase(LeanTweenType.easeOutSine);
                    break;
            }
        }

        private void OnTalking(ChatCatExpression expression)
        {
            ChatCatOverlayScreen.MessageQueue.Enqueue(expression);
            if (!ChatCatOverlayScreen.DisplayedBy)
                OverlayScreenManager.EnqueueScreen(ChatCatOverlayScreen);
        }

        private void OnBoxThumped(Vector3 boxPosition, int direction)
        {
            CanvasGroup thumpInstance = Instantiate(ThumpCaption, WorldCanvas.transform);
            float rotationRange = UnityEngine.Random.Range(-1.0f, 1.0f);
            float xPosition = boxPosition.x + 0.75f * direction;
            float yPosition = 0.5f + rotationRange * 0.2f;
            float cameraYMin = Camera.transform.position.y - Camera.orthographicSize + 1.0f;
            if (yPosition < cameraYMin)
                yPosition = cameraYMin;
            Vector3 captionPosition = new Vector3(xPosition, yPosition, 0.0f);
            thumpInstance.transform.position = captionPosition;
            RectTransform rectTransform = thumpInstance.GetComponent<RectTransform>();
            if (direction < 0)
            {
                rectTransform.SetPivotRight();
            }
            else
            {
                rectTransform.SetPivotLeft();
            }
            rectTransform.transform.rotation = Quaternion.Euler(
                0.0f,
                0.0f,
                30.0f * rotationRange * direction
            );

            LeanTween
                .alphaCanvas(thumpInstance, 0.0f, 0.4f)
                .setEase(LeanTweenType.easeInCubic)
                .setDestroyOnComplete(true);
        }

        private void OnBondingProgressStateChanged(CatSightingUI catSightingUI, bool isEntering)
        {
            if (isEntering)
            {
                _bondingProgressAudioSource = GameManager.Instance.Audio.PlaySoundEffect(
                    BondingProgressAudioEvent
                );
            }
            else
            {
                if (_bondingProgressAudioSource)
                {
                    _bondingProgressAudioSource.Stop();
                    _bondingProgressAudioSource = null;
                }
            }
        }
    }
}
