using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class ReplayCutScenesScene : SceneBehaviour
    {
        public CutScene TutorialCutScene;
        public List<CutScene> UnlockedCutScenes { get { return _unlockedCutScenes; } }

        private List<CutScene> _unlockedCutScenes = new List<CutScene>();

        public void PlayCutScene(CutScene cutScene)
        {
            _gameManager.CutScenes.EnqueueCutScene(cutScene);
            _gameManager.ReplayCutScenes();
        }

        public void GoHome()
        {
            _gameManager.GoHome();
        }

        protected override void Awake()
        {
            base.Awake();

            GameManager gameManager = GameManager.Instance;
            EventManager eventManager = gameManager.Events;
            PuzzleManager puzzleManager = gameManager.Puzzles;

            _unlockedCutScenes.Add(TutorialCutScene);

            foreach(StoryFirstCompletionEvent e in eventManager.StoryFirstCompletionEvents)
            {
                foreach(EventResponse eventResponse in e.EventResponses)
                {
                    if (eventResponse.PlayCutScene != null && puzzleManager.IsStoryPuzzleCompleted(e.StoryPuzzle))
                    {
                        _unlockedCutScenes.Add(eventResponse.PlayCutScene);
                    }
                }
            }
        }
    }
}