using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class NewCatOverlayScreen : OverlayScreen
    {
        public CanvasGroup OverlayCanvas;
        public Image CatPortait;
        public Text CatNameText;
        public Color NotBondedColor = new(0, 0, 0, 0.33f);
        public GameObject NotBondedGameObject;

        /// <summary>
        /// The cat sighted.
        /// </summary>
        public Cat Cat;

        protected override void Update()
        {
            base.Update();

            if (Input.GetMouseButtonDown(0))
                Dismiss();
        }

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            if (Cat && OverlayCanvas)
            {
                OverlayCanvas.alpha = 0.0f;
                LeanTween
                    .alphaCanvas(OverlayCanvas, 1.0f, TransitionInDuration)
                    .setEase(TransitionInTween);

                var isBonded = GameManager.Instance.Cats.IsBonded(Cat);

                if (CatPortait)
                {
                    CatPortait.sprite = Cat.Portrait;
                    CatPortait.color = NotBondedColor;
                    NotBondedGameObject.SetActive(!isBonded);
                }
                if (CatNameText)
                    CatNameText.text = isBonded ? Cat.Name : "Name: ???";
            }
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            if (OverlayCanvas)
            {
                LeanTween
                    .alphaCanvas(OverlayCanvas, 0.0f, TransitionOutDuration)
                    .setEase(TransitionOutTween);
            }
        }
    }
}
