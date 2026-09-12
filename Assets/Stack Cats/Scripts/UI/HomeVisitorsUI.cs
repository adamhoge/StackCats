using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats.UI
{
    public class HomeVisitorsUI : MonoBehaviour
    {
        public HomeScene HomeScene;
        public HomeVisitors HomeVisitors;
        public Button VisitorsMinigameButton;
        public TextMeshProUGUI VisitorsMinigameButtonText;
        public List<Button> InteractWithVisitorButtons;

        protected void OnEnable()
        {
            VisitorsMinigameButton.onClick.AddListener(PlayVisitorsMinigame);
        }

        protected void OnDisable()
        {
            VisitorsMinigameButton.onClick.RemoveListener(PlayVisitorsMinigame);
        }

        protected void Awake()
        {
            foreach(Button interactWithVisitorButton in InteractWithVisitorButtons)
            {
                interactWithVisitorButton.onClick.AddListener(() => { HomeVisitors.InteractWithVisitor(InteractWithVisitorButtons.IndexOf(interactWithVisitorButton)); });
            }
        }

        protected void Start()
        {
            MinigameInformation visitorsMinigame = HomeVisitors.VisitorsMinigame;
            if (visitorsMinigame != null)
            {
                VisitorsMinigameButtonText.text = "Play " + visitorsMinigame.MinigameTitle;
            }
            else
            {
                VisitorsMinigameButton.gameObject.SetActive(false);
            }
        }

        private void PlayVisitorsMinigame()
        {
            HomeScene.PlayMinigame(HomeVisitors.VisitorsMinigame);
        }
    }
}