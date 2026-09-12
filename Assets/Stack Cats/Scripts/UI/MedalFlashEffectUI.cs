using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class MedalFlashEffectUI : FlashEffectUI
    {
        public Image MedalMaskImage;
        public Image MedalOutlineMaskImage;
        public MedalSpriteCollection MedalSpriteCollection;

        public bool HasOutline { get { return _hasOutline; } set { SetHasOutline(value); } }

        public int NumPuzzlesCompleted { get { return _numPuzzlesCompleted; } set { SetNumPuzzlesCompleted(value); } }

        [SerializeField]
        [HideInInspector]
        private bool _hasOutline = true;

        [SerializeField]
        [HideInInspector]
        private int _numPuzzlesCompleted;

        private void Awake()
        {
            FlashCanvasGroup.alpha = 0.0f;
            FlashRippleCanvasGroup.alpha = 0.0f;
        }

        private void SetHasOutline(bool value)
        {
            if (value == _hasOutline) return;

            _hasOutline = value;

            if (_hasOutline)
            {
                MedalOutlineMaskImage.gameObject.SetActive(true);
                MedalOutlineMaskImage.sprite = GetMedalOutlineSprite(_numPuzzlesCompleted);
            }
            else
            {
                MedalOutlineMaskImage.gameObject.SetActive(false);
            }
        }

        private void SetNumPuzzlesCompleted(int value)
        {
            if (value == _numPuzzlesCompleted) return;

            _numPuzzlesCompleted = value;

            MedalMaskImage.sprite = GetMedalSprite(_numPuzzlesCompleted);

            if (_hasOutline)
            {
                MedalOutlineMaskImage.sprite = GetMedalOutlineSprite(_numPuzzlesCompleted);
            }
        }

        private Sprite GetMedalSprite(int numPuzzlesCompleted)
        {
            if (numPuzzlesCompleted <= 0) return null;

            var medalSprites = MedalSpriteCollection.MedalSprites;
            if (numPuzzlesCompleted >= medalSprites.Count) return medalSprites[medalSprites.Count - 1];

            return medalSprites[numPuzzlesCompleted - 1];
        }

        private Sprite GetMedalOutlineSprite(int numPuzzlesCompleted)
        {
            if (numPuzzlesCompleted <= 0) return null;

            var medalOutlineSprites = MedalSpriteCollection.MedalOutlineSprites;
            if (numPuzzlesCompleted >= medalOutlineSprites.Count) return medalOutlineSprites[medalOutlineSprites.Count - 1];

            return medalOutlineSprites[numPuzzlesCompleted - 1];
        }
    }
}