using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class EventResponse
    {
        public CutScene PlayCutScene;
    }

    [Serializable]
    public class StoryFirstCompletionEvent
    {
        public StoryPuzzle StoryPuzzle;
        public List<EventResponse> EventResponses;
    }

    public class EventManager : MonoBehaviour
    {
        public List<StoryFirstCompletionEvent> StoryFirstCompletionEvents;

        private GameManager _gameManager;
        private PuzzleManager _puzzleManager;

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
            _puzzleManager = _gameManager.Puzzles;
        }

        protected void OnEnable()
        {
            _puzzleManager.onStoryPuzzleFirstCompletion += OnStoryPuzzleFirstCompletion;
        }

        protected void OnDisable()
        {
            _puzzleManager.onStoryPuzzleFirstCompletion -= OnStoryPuzzleFirstCompletion;
        }

        private void OnStoryPuzzleFirstCompletion(StoryPuzzle storyPuzzle)
        {
            StoryFirstCompletionEvent completionEvent = StoryFirstCompletionEvents.FirstOrDefault(e => e.StoryPuzzle == storyPuzzle);

            if (completionEvent != null)
            {
                foreach (EventResponse eventResponse in completionEvent.EventResponses)
                {
                    if (eventResponse.PlayCutScene != null)
                    {
                        _gameManager.CutScenes.EnqueueCutScene(eventResponse.PlayCutScene);
                    }
                }
            }
        }
    }
}