using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    [Serializable]
    public class ColorAtIndex
    {
        public int Index;
        public Color Color;
    }

    public class MedalUI : MonoBehaviour
    {
        public Image MedalImage;
        public Image MedalOutlineImage;
        public Sprite MedalPlaceholderSprite;
        public TextMeshProUGUI MedalText;
        public MedalSpriteCollection MedalSpriteCollection;
        public List<ColorAtIndex> MedalTextColors;

        public bool HasOutline
        {
            get { return _hasOutline; }
            set { SetHasOutline(value); }
        }

        public int NumPuzzlesCompleted
        {
            get { return _numPuzzlesCompleted; }
            set { SetNumPuzzlesCompleted(value); }
        }

        [SerializeField]
        [HideInInspector]
        private bool _hasOutline = true;

        [SerializeField]
        [HideInInspector]
        private int _numPuzzlesCompleted;

        private void SetHasOutline(bool value)
        {
            if (value == _hasOutline)
                return;

            _hasOutline = value;

            if (_hasOutline)
            {
                MedalOutlineImage.gameObject.SetActive(true);
                MedalOutlineImage.sprite = GetMedalOutlineSprite(_numPuzzlesCompleted);
            }
            else
            {
                MedalOutlineImage.gameObject.SetActive(false);
            }
        }

        private void SetNumPuzzlesCompleted(int value)
        {
            if (value == _numPuzzlesCompleted)
                return;

            _numPuzzlesCompleted = value;

            MedalImage.sprite = GetMedalSprite(_numPuzzlesCompleted);

            if (_numPuzzlesCompleted <= 0)
            {
                MedalText.gameObject.SetActive(false);
            }
            else
            {
                MedalText.gameObject.SetActive(true);
                MedalText.text = _numPuzzlesCompleted.ToString();
                MedalText.color = GetMedalTextColor(_numPuzzlesCompleted);
            }

            if (_hasOutline)
            {
                MedalOutlineImage.sprite = GetMedalOutlineSprite(_numPuzzlesCompleted);
            }
        }

        private Sprite GetMedalSprite(int numPuzzlesCompleted)
        {
            if (numPuzzlesCompleted <= 0)
                return MedalPlaceholderSprite;

            var medalSprites = MedalSpriteCollection.MedalSprites;
            if (numPuzzlesCompleted >= medalSprites.Count)
                return medalSprites[medalSprites.Count - 1];

            return medalSprites[numPuzzlesCompleted - 1];
        }

        private Sprite GetMedalOutlineSprite(int numPuzzlesCompleted)
        {
            if (numPuzzlesCompleted <= 0)
                return MedalPlaceholderSprite;

            var medalOutlineSprites = MedalSpriteCollection.MedalOutlineSprites;
            if (numPuzzlesCompleted >= medalOutlineSprites.Count)
                return medalOutlineSprites[medalOutlineSprites.Count - 1];

            return medalOutlineSprites[numPuzzlesCompleted - 1];
        }

        private Color GetMedalTextColor(int numPuzzlesCompleted)
        {
            for (int i = 0; i < MedalTextColors.Count; i++)
            {
                int nextIndex = i + 1;
                if (
                    nextIndex >= MedalTextColors.Count
                    || MedalTextColors[nextIndex].Index > numPuzzlesCompleted
                )
                {
                    return MedalTextColors[i].Color;
                }
            }

            return MedalTextColors.Last().Color;
        }
    }
}
