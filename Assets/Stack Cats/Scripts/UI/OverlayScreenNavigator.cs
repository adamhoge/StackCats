using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    [System.Serializable]
    public class OverlayScreenItem
    {
        public OverlayScreen OverlayScreen;
        public string OverlayScreenTitle;
    }

    public class OverlayScreenNavigator : MonoBehaviour
    {
        /// <summary>
        /// The overlay screen manager used to load screens.
        /// </summary>
        [Tooltip("The overlay screen manager used to load screens.")]
        public OverlayScreenManager OverlayScreenManager;

        /// <summary>
        /// The list of screens that can be navigated.
        /// </summary>
        [Tooltip("The list of screens that can be navigated.")]
        public List<OverlayScreenItem> OverlayScreens;

        /// <summary>
        /// The display text for the OverlayScreenTitle.
        /// </summary>
        [Tooltip("The display text for the OverlayScreenTitle.")]
        public TextMeshProUGUI OverlayScreenTitleText;

        /// <summary>
        /// Allows navigation to move from the last screen to the first and vice versa.
        /// </summary>
        [Tooltip("Allows navigation to move from the last screen to the first and vice versa.")]
        public bool WrapNavigation = true;

        private int _currentScreenIndex = -1;

        /// <summary>
        /// Navigate to an OverlayScreen
        /// </summary>
        /// <param name="overlayScreen">The OverlayScreen to which to navigate.</param>
        public void GoToScreen(OverlayScreen overlayScreen)
        {
            int overlayScreenIndex = OverlayScreens.FindIndex(os => os.OverlayScreen == overlayScreen);
            GoToScreen(overlayScreenIndex);
        }

        /// <summary>
        /// Naigate to an OverlayScreen.
        /// </summary>
        /// <param name="screenIndex">The index of the OverlayScreen to which to navigate</param>
        public void GoToScreen(int screenIndex)
        {
            if (!OverlayScreenManager)
            {
                Debug.LogError("OverlayScreenManager required to navigate screens.");
                return;
            }

            if (screenIndex < 0 || screenIndex >= OverlayScreens.Count)
            {
                Debug.LogError("Screen index is out of range.");
                return;
            }

            OverlayScreenItem overlayScreenItem = OverlayScreens[screenIndex];
            if (OverlayScreenTitleText) OverlayScreenTitleText.text = overlayScreenItem.OverlayScreenTitle;
            OverlayScreenManager.EnqueueScreen(overlayScreenItem.OverlayScreen, true);
            _currentScreenIndex = screenIndex;
        }

        /// <summary>
        /// Go to the previous screen.
        /// </summary>
        public void GoToPreviousScreen()
        {
            if (_currentScreenIndex > 0 || WrapNavigation)
            {
                int screenIndex = (_currentScreenIndex - 1 + OverlayScreens.Count) % OverlayScreens.Count;
                GoToScreen(screenIndex);
            }
        }

        /// <summary>
        /// Go to the next screen.
        /// </summary>
        public void GoToNextScreen()
        {
            int numScreens = OverlayScreens.Count;
            if(_currentScreenIndex < numScreens || WrapNavigation)
            {
                int screenIndex = (_currentScreenIndex + 1) % numScreens;
                GoToScreen(screenIndex);
            }
        }

        /// <summary>
        /// Hide the current screen (if any).
        /// </summary>
        public void Hide()
        {
            OverlayScreenManager.DismissAllScreens();
        }

        /// <summary>
        /// Show the current screen (if any).
        /// </summary>
        public void Show()
        {
            GoToScreen(_currentScreenIndex);
        }
    }
}