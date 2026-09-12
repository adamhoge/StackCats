using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void PresentOpened(Present present);

    public enum PresentState
    {
        Unopened,
        Opening,
        Opened
    }

    public class Present : MonoBehaviour
    {
        public event PresentOpened onPresentOpened;

        public PresentInfo PresentInfo;
        public SpriteRenderer PresentSpriteRenderer;
        public Sprite OpenedPresentSprite;
        public GameObject PresentLabel;
        public TextMeshPro PresentLabelNameText;

        public PresentState State { get { return _state; } }

        private PresentState _state;

        public void OpenPresent()
        {
            if(_state == PresentState.Unopened)
            {
                _state = PresentState.Opening;
                LeanTween.scale(gameObject, Vector3.one * 0.9f, 0.75f).setEase(LeanTweenType.easeOutQuint).setOnComplete(OpeningComplete);
            }
        }

        private void OpeningComplete()
        {
            gameObject.transform.localScale = Vector3.one;
            LeanTween.scale(gameObject, Vector3.one * 1.25f, 0.5f).setEase(LeanTweenType.punch);
            PresentSpriteRenderer.sprite = OpenedPresentSprite;
            _state = PresentState.Opened;
            if (onPresentOpened != null) onPresentOpened(this);
        }

        protected void Start()
        {
            Cat fromCat = PresentInfo.FromCat;
            if (fromCat)
            {
                PresentLabelNameText.text = GameManager.Instance.Cats.IsBonded(fromCat) ? fromCat.Name : "???";
            }
            else
            {
                PresentLabel.SetActive(false);
            }
        }
    }
}