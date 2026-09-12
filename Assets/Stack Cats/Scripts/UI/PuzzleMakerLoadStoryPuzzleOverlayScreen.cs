using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(PuzzleLoader))]
    public class PuzzleMakerLoadStoryPuzzleOverlayScreen : OverlayScreen
    {
        public PuzzleMakerScene PuzzleMakerScene;

        public RectTransform BrowseAllRectTransform;
        public RectTransform StoryPuzzleButtonsRectTransform;
        public Button CancelButton;
        public Button StoryPuzzleButtonPrefab;

        private PuzzleManager _puzzleManager;
        private List<StoryPuzzle> _storyPuzzles;
        private int _selectedPuzzleIndex;
        private string _selectedPuzzleJsonData;
        private List<Button> _storyPuzzleButtonInstances = new List<Button>();

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            _storyPuzzles = _puzzleManager.GetStoryPuzzles(PuzzleMakerScene.CurrentAreaEditor.PuzzleArea);

            while (_storyPuzzleButtonInstances.Count > 0)
            {
                Destroy(_storyPuzzleButtonInstances[0].gameObject);
                _storyPuzzleButtonInstances.RemoveAt(0);
            }

            for (int i = 0; i < _storyPuzzles.Count; i++)
            {
                StoryPuzzle storyPuzzle = _storyPuzzles[i];
                if (storyPuzzle == null) continue;
                Button storyPuzzleButton = Instantiate(StoryPuzzleButtonPrefab, StoryPuzzleButtonsRectTransform);
                storyPuzzleButton.onClick.AddListener(delegate () { SelectPuzzle(storyPuzzle); });
                storyPuzzleButton.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
                _storyPuzzleButtonInstances.Add(storyPuzzleButton);
            }
        }

        public void SelectPuzzle(StoryPuzzle storyPuzzle)
        {
            PuzzleMakerScene.LoadPuzzle(storyPuzzle.JsonData);

            Dismiss();
        }

        protected void Awake()
        {
            _puzzleManager = GameManager.Instance.Puzzles;
        }

        protected void OnEnable()
        {
            CancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

        protected void OnDisable()
        {
            CancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        }

        private void OnCancelButtonClicked()
        {
            Dismiss();
        }
    }
}