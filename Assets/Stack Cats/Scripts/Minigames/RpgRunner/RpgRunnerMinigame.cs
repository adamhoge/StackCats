using System;
using UnityEngine;

namespace Tofuwu.StackCats.Minigames.RpgRunner
{
    public enum RpgRunnerMinigameState
    {
        NotStarted,
        CatsEntering,
        CatsRunning,
        CatsFighting,
        GameOver
    }

    [RequireComponent(typeof(Minigame))]
    public class RpgRunnerMinigame : MonoBehaviour
    {
        private Minigame _minigame;
        private RpgRunnerMinigameState _state;

        protected void Awake()
        {
            _minigame = GetComponent<Minigame>();
        }

        protected void Update()
        {
            if (_minigame.State == MinigameState.Playing)
            {
                switch (_state)
                {
                    case RpgRunnerMinigameState.NotStarted:
                        break;
                    case RpgRunnerMinigameState.CatsEntering:
                        break;
                    case RpgRunnerMinigameState.CatsRunning:
                        break;
                    case RpgRunnerMinigameState.CatsFighting:
                        break;
                    case RpgRunnerMinigameState.GameOver:
                        break;
                }
            }
        }

        private void ChangeState(RpgRunnerMinigameState state)
        {
            _state = state;

            switch (_state)
            {
                case RpgRunnerMinigameState.NotStarted:
                    break;
                case RpgRunnerMinigameState.CatsEntering:
                    break;
                case RpgRunnerMinigameState.CatsRunning:
                    break;
                case RpgRunnerMinigameState.CatsFighting:
                    break;
                case RpgRunnerMinigameState.GameOver:
                    break;
            }
        }
    }
}