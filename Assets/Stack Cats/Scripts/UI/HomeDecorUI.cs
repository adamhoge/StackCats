using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public delegate void ChangingDecor();
    public delegate void DoneChangingDecor();

    public class HomeDecorUI : MonoBehaviour
    {
        public event ChangingDecor onChangingDecor;
        public event DoneChangingDecor onDoneChangingDecor;

        public HomeDecor HomeDecor;
        public OverlayScreenManager OverlayScreenManager;
        public ChangeDecorOverlayScreen ChangeDecorOverlayScreen;
        public Button ChangeDecorButton;
        public List<Image> DrawerImages;

        protected void OnEnable()
        {
            HomeDecor.onDresserSet += OnDresserSet;
            ChangeDecorButton.onClick.AddListener(OnChangeDecorButtonClicked);
        }

        protected void OnDisable()
        {
            HomeDecor.onDresserSet -= OnDresserSet;
            ChangeDecorButton.onClick.RemoveListener(OnChangeDecorButtonClicked);
        }

        private void OnDresserSet(DresserItem dresser)
        {
            foreach(Image image in DrawerImages)
            {
                image.sprite = dresser.DrawerSprite;
                image.SetMaterialForItem(dresser.ColorShift);
            }
        }

        private void OnChangeDecorButtonClicked()
        {
            OverlayScreenManager.EnqueueScreen(ChangeDecorOverlayScreen);
            OverlayScreenManager.OnHidden.AddListener(OnOverlayScreenManagerHidden);

            if (onChangingDecor != null) onChangingDecor();
        }

        private void OnOverlayScreenManagerHidden()
        {
            OverlayScreenManager.OnHidden.RemoveListener(OnOverlayScreenManagerHidden);

            if (onDoneChangingDecor != null) onDoneChangingDecor();
        }
    }
}