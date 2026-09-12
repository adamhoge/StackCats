using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class PuzzleThemeTransitioner : MonoBehaviour
    {
        private enum State
        {
            None,
            FadingOut,
            FadingIn
        }

        public Camera Camera;
        public SpriteRenderer ScenaryFaderSpriteRenderer;
        public SpriteRenderer TopBoundarySprite;
        public float TransitionDuration = 1.0f;

        private State _state;
        private float _stateTimeElapsed;
        private GameObject _currentScenary;
        private PuzzleTheme _currentPuzzleTheme;

        public void LoadPuzzleTheme(PuzzleTheme puzzleTheme)
        {
            if (puzzleTheme == _currentPuzzleTheme) return;

            _currentPuzzleTheme = puzzleTheme;

            if (!_currentScenary)
            {
                ChangeState(State.FadingIn);
            }
            else
            {
                ChangeState(State.FadingOut);
            }
        }

        protected void Update()
        {
            _stateTimeElapsed += Time.deltaTime;

            switch (_state)
            {
                case State.None:
                    break;
                case State.FadingOut:
                    if (_stateTimeElapsed >= TransitionDuration)
                    {
                        ChangeState(State.FadingIn);
                    }
                    break;
                case State.FadingIn:
                    if (_stateTimeElapsed >= TransitionDuration)
                    {
                        ChangeState(State.None);
                    }
                    break;
            }
        }

        private void ChangeState(State state)
        {
            if (_state == state) return;

            _state = state;
            _stateTimeElapsed = 0.0f;

            switch (state)
            {
                case State.None:
                    break;
                case State.FadingOut:
                    LeanTween.cancel(ScenaryFaderSpriteRenderer.gameObject);
                    LeanTween.alpha(ScenaryFaderSpriteRenderer.gameObject, 1.0f, TransitionDuration).setEase(LeanTweenType.easeOutSine);
                    break;
                case State.FadingIn:
                    if (_currentScenary)
                    {
                        Destroy(_currentScenary.gameObject);
                    }
                    _currentScenary = null;
                    TopBoundarySprite.sprite = null;

                    if (_currentPuzzleTheme != null)
                    {
                        Camera.backgroundColor = _currentPuzzleTheme.BackgroundColor;
                        GameObject puzzleScenaryPrefab = _currentPuzzleTheme.PuzzleScenaryPrefab;
                        if (puzzleScenaryPrefab)
                        {
                            _currentScenary = Instantiate(puzzleScenaryPrefab, transform);
                            _currentScenary.name = "Puzzle Scenary";
                        }
                        else
                        {
                            _currentScenary = null;
                        }

                        if (_currentPuzzleTheme.TopBoundarySprite != null)
                        {
                            TopBoundarySprite.sprite = _currentPuzzleTheme.TopBoundarySprite;
                            TopBoundarySprite.transform.SetParent(transform);
                        }
                        TopBoundarySprite.transform.localPosition = Vector2.up * 6.25f;
                    }
                    else
                    {
                        Camera.backgroundColor = Color.gray;
                    }

                    Color faderColor = ScenaryFaderSpriteRenderer.color;
                    faderColor.a = 1.0f;
                    ScenaryFaderSpriteRenderer.color = faderColor;
                    LeanTween.cancel(ScenaryFaderSpriteRenderer.gameObject);
                    LeanTween.alpha(ScenaryFaderSpriteRenderer.gameObject, 0.0f, TransitionDuration).setEase(LeanTweenType.easeOutSine);
                    break;
            }
        }
    }
}
