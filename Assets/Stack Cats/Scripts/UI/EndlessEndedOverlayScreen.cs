using System.Collections.Generic;
using System.Linq;
using RockhopperGames.StackCats.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockhopperGames.StackCats.UI
{
    public class EndlessEndedOverlayScreen : OverlayScreen
    {
        public Button CloseButton;
        public TextMeshProUGUI ScoreText;
        public EndlessRunScene EndlessRunScene;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            ScoreText.text = EndlessRunScene.CurrentEndlessRun.Score.ToString();
        }

        protected void OnEnable()
        {
            CloseButton.onClick.AddListener(OnCloseButtonClicked);
        }

        protected void OnDisable()
        {
            CloseButton.onClick.RemoveListener(OnCloseButtonClicked);
        }

        private void OnCloseButtonClicked()
        {
            Dismiss();
        }
    }
}
