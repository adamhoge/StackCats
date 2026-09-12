using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void MinigameStateChanged(MinigameState state);
    public delegate void MinigameComplete();

    public enum MinigameState
    {
        NotStarted,
        TitleScreen,
        Playing,
        Completed
    }

    public class Minigame : MonoBehaviour
    {
        public event MinigameStateChanged onMinigameStateChanged;
        public event MinigameComplete onMinigameComplete;

        public float TitleScreenDuration = 3.0f;
        public List<Cat> MinigameCats;
        public CatAvatar CatAvatarPrefab;

        public MinigameState State { get { return _state; } }
        public Camera Camera { get; set; }

        public bool IsComplete { get { return _state == MinigameState.Completed; } }

        private MinigameState _state;
        private float _stateTimeElapsed;
        private List<CatAvatar> _titleCatAvatars = new List<CatAvatar>();

        public void Initialize(Camera camera, List<Cat> minigameCats)
        {
            Camera = camera;
            MinigameCats = minigameCats;
        }

        public void Complete()
        {
            ChangeState(MinigameState.Completed);

            if (onMinigameComplete != null) onMinigameComplete();
        }

        protected void Start()
        {
            ChangeState(MinigameState.TitleScreen);
        }

        protected void Update()
        {
            _stateTimeElapsed += Time.deltaTime;

            switch (_state)
            {
                case MinigameState.NotStarted:
                    break;
                case MinigameState.TitleScreen:
                    if(_stateTimeElapsed > TitleScreenDuration)
                    {
                        ChangeState(MinigameState.Playing);
                    }
                    break;
                case MinigameState.Playing:
                    break;
                case MinigameState.Completed:
                    break;
            }
        }

        private void ShowCats()
        {
            for (int i = 0; i < MinigameCats.Count; i++)
            {
                Cat cat = MinigameCats[i];

                CatAvatar visitorAvatar = Instantiate(CatAvatarPrefab, transform);
                visitorAvatar.transform.localPosition = GetVisitorLocalPositionByIndex(i);
                visitorAvatar.Cat = cat;
                _titleCatAvatars.Add(visitorAvatar);
            }
        }

        private void HideCats()
        {
            foreach(CatAvatar catAvatar in _titleCatAvatars)
            {
                Destroy(catAvatar.gameObject);
            }
            _titleCatAvatars.Clear();
        }

        private Vector3 GetVisitorLocalPositionByIndex(int i)
        {
            float xPosition = (-(MinigameCats.Count / 2.0f) + i + 0.5f) * 1f;

            return new Vector3(xPosition, -2.0f, 0.0f);
        }

        private void ChangeState(MinigameState state)
        {
            _state = state;
            _stateTimeElapsed = 0.0f;

            switch (_state)
            {
                case MinigameState.NotStarted:
                    break;
                case MinigameState.TitleScreen:
                    ShowCats();
                    break;
                case MinigameState.Playing:
                    HideCats();
                    break;
                case MinigameState.Completed:
                    break;
            }

            if (onMinigameStateChanged != null) onMinigameStateChanged(_state);
        }
    }
}