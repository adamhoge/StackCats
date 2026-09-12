using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public class TutorialManagerUI : MonoBehaviour
    {
        public OverlayScreenManager OverlayScreenManager;
        public RectTransform OverlayScreensRectTransform;
        public PuzzleTutorialOverlayScreen PuzzleTutorialOverlayScreenPrefab;
        public BlockTutorialOverlayScreen BlockTutorialOverlayScreenPrefab;
        public PointerGestureUI PointerGestureUI;

        private TutorialManager _tutorialManager;

        protected void Awake()
        {
            _tutorialManager = GameManager.Instance.TutorialManager;
        }

        protected void OnEnable()
        {
            _tutorialManager.onPuzzleTutorialStarted += OnPuzzleTutorialStarted;
            _tutorialManager.onBlockTutorialStarted += OnBlockTutorialStarted;
        }

        protected void OnDisable()
        {
            _tutorialManager.onPuzzleTutorialStarted -= OnPuzzleTutorialStarted;
            _tutorialManager.onBlockTutorialStarted -= OnBlockTutorialStarted;
        }

        private void OnPuzzleTutorialStarted(PuzzleTutorial puzzleTutorial)
        {
            PuzzleTutorialOverlayScreen puzzleTutorialInstance = Instantiate(PuzzleTutorialOverlayScreenPrefab, OverlayScreensRectTransform);
            puzzleTutorialInstance.Initialize(puzzleTutorial, PointerGestureUI);
            OverlayScreenManager.EnqueueScreen(puzzleTutorialInstance);
        }

        private void OnBlockTutorialStarted(BlockTutorial blockTutorial)
        {
            BlockTutorialOverlayScreen blockTutorialInstance = Instantiate(BlockTutorialOverlayScreenPrefab, OverlayScreensRectTransform);
            blockTutorialInstance.Initialize(blockTutorial, PointerGestureUI);
            OverlayScreenManager.EnqueueScreen(blockTutorialInstance);
        }
    }
}
