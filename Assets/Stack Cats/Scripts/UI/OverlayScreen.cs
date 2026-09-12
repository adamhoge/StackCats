using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public delegate void TransitioningIn();
    public delegate void TransitioningOut();
    public delegate void Hidden();

    public class OverlayScreen : MonoBehaviour
    {
        public event TransitioningIn onTransitioningIn;
        public event TransitioningOut onTransitioningOut;
        public event Hidden onHidden;

        public OverlayScreenManager DisplayedBy;

        public float TransitionInDuration = 0.5f;
        public float TransitionOutDuration = 0.5f;
        public LeanTweenType TransitionInTween = LeanTweenType.easeOutSine;
        public LeanTweenType TransitionOutTween = LeanTweenType.easeOutSine;
        public Color OverlayColor = Color.black;

        protected bool _isActive;
        protected float _activeTimeElapsed;

        public void Dismiss()
        {
            if (DisplayedBy) DisplayedBy.DismissCurrentScreen();
        }

        public virtual void OnTransitioningIn()
        {
            if (onTransitioningIn != null) onTransitioningIn();
        }

        public virtual void OnTransitioningOut()
        {
            _isActive = false;
            if (onTransitioningOut != null) onTransitioningOut();
        }

        public virtual void OnActive()
        {
            _isActive = true;
            _activeTimeElapsed = 0.0f;
        }

        public virtual void OnHidden() {
            if (onHidden != null) onHidden();
        }

        protected virtual void Update()
        {
            if (_isActive)
            {
                _activeTimeElapsed += Time.deltaTime;
            }
        }
    }
}