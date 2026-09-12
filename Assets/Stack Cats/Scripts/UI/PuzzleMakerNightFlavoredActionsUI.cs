using System;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleMakerNightFlavoredActionsUI : PuzzleMakerActionsUI
    {
        private PuzzleMakerNightFlavoredActions _puzzleMakerNightFlavoredActions;

        public Toggle AutoDropCurtainToggle;
        public InputField CurtainDropMoveCountInputField;
        public InputField CurtainIntervalInputField;
        public Button RaiseCurtainButton;
        public Button DropCurtainButton;

        protected override void OnEnable()
        {
            base.OnEnable();

            AutoDropCurtainToggle.onValueChanged.AddListener(OnToggleAutoDropCurtainChanged);
            CurtainDropMoveCountInputField.onEndEdit.AddListener(OnCurtainDropMoveCountInputChanged);
            CurtainIntervalInputField.onEndEdit.AddListener(OnCurtainIntervalInputChanged);
            RaiseCurtainButton.onClick.AddListener(OnRaiseCurtainButtonClicked);
            DropCurtainButton.onClick.AddListener(OnDropCurtainButtonClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            AutoDropCurtainToggle.onValueChanged.RemoveListener(OnToggleAutoDropCurtainChanged);
            CurtainDropMoveCountInputField.onEndEdit.RemoveListener(OnCurtainDropMoveCountInputChanged);
            CurtainIntervalInputField.onEndEdit.RemoveListener(OnCurtainIntervalInputChanged);
            RaiseCurtainButton.onClick.RemoveListener(OnRaiseCurtainButtonClicked);
            DropCurtainButton.onClick.RemoveListener(OnDropCurtainButtonClicked);
        }

        protected override void Start()
        {
            base.Start();

            _puzzleMakerNightFlavoredActions = (PuzzleMakerNightFlavoredActions)PuzzleMakerActions;
            _puzzleMakerNightFlavoredActions.onCurtainDropMoveCountChanged += OnCurtainDropMoveCountChanged;
            AutoDropCurtainToggle.isOn = _puzzleMakerNightFlavoredActions.IsAutoLiftCurtainEnabled;
            CurtainDropMoveCountInputField.text = "0";
            CurtainIntervalInputField.text = _puzzleMakerNightFlavoredActions.NightFlavoredPuzzle.CurtainDropInterval.ToString();
        }

        private void OnToggleAutoDropCurtainChanged(bool value)
        {
            _puzzleMakerNightFlavoredActions.IsAutoLiftCurtainEnabled = value; 
        }

        private void OnCurtainDropMoveCountChanged(int moveCount)
        {
            CurtainDropMoveCountInputField.text = moveCount.ToString();
        }

        private void OnCurtainDropMoveCountInputChanged(string value)
        {
            int curtainDropMoveCount;
            if (int.TryParse(value, out curtainDropMoveCount))
            {
                _puzzleMakerNightFlavoredActions.SetCurtainDropMoveCount(curtainDropMoveCount);
            }
        }

        private void OnCurtainIntervalInputChanged(string value)
        {
            int interval;
            if (int.TryParse(value, out interval))
            {
                _puzzleMakerNightFlavoredActions.SetCurtainDropInterval(interval);
            }
        }

        private void OnRaiseCurtainButtonClicked()
        {
            _puzzleMakerNightFlavoredActions.LiftCurtain();
        }

        private void OnDropCurtainButtonClicked()
        {
            _puzzleMakerNightFlavoredActions.DropCurtain();
        }
    }
}