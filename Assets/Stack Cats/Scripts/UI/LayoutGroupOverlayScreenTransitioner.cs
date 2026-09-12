using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(LayoutGroup))]
    public class LayoutGroupOverlayScreenTransitioner : MonoBehaviour
    {
        public OverlayScreen OverlayScreen;
        public float TransitionXOffset = 0.0f;
        public float TransitionYOffset = 512.0f;

        private LayoutGroup _layoutGroup;

        protected void Awake()
        {
            _layoutGroup = GetComponent<LayoutGroup>();
        }

        protected void OnEnable()
        {
            OverlayScreen.onTransitioningIn += OnTransitioningIn;
            OverlayScreen.onTransitioningOut += OnTransitioningOut;
        }

        protected void OnDisable()
        {
            OverlayScreen.onTransitioningIn -= OnTransitioningIn;
            OverlayScreen.onTransitioningOut -= OnTransitioningOut;
        }

        private void OnTransitioningIn()
        {
        }

        private void OnTransitioningOut()
        {
        }
    }
}
