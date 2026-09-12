using System;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class MinigameScene : SceneBehaviour
    {
        public Camera Camera;

        private Minigame _minigameInstance;

        protected override void Start()
        {
            base.Start();

            MinigameInformation currentMinigame = _gameManager.Minigames.CurrentMinigame;
            if(!currentMinigame)
            {
                Debug.LogError("Minigame is not defined in MinigameScene.");
                _gameManager.GoHome();
                return;
            }

            _minigameInstance = Instantiate(currentMinigame.MinigamePrefab, transform);
            _minigameInstance.name = currentMinigame.MinigameTitle + " (Minigame)";
            _minigameInstance.onMinigameComplete += OnMinigameComplete;
            _minigameInstance.Initialize(Camera, _gameManager.Minigames.MinigameCats);
        }

        private void OnMinigameComplete()
        {
            _gameManager.GoHome();
        }
    }
}