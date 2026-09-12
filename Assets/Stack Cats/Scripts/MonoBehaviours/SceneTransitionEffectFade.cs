using UnityEngine;

namespace Tofuwu.StackCats
{
    class SceneTransitionEffectFade : ISceneTransitionEffect
    {
        private readonly ScreenOverlay _overlay;

        public SceneTransitionEffectFade(ScreenOverlay overlay)
        {
            _overlay = overlay;
        }

        public void TransitionUpdate(float transitionPosition)
        {
            Color originalColor = _overlay.OverlayColor;
            float r = originalColor.r;
            float g = originalColor.g;
            float b = originalColor.b;
            float a = transitionPosition;
            _overlay.OverlayColor = new Color(r, g, b, a);
        }
    }
}
