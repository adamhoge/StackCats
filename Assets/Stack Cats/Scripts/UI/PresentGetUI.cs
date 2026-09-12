using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class PresentGetUI : MonoBehaviour
    {
        public Sprite PresentSprite;
        public string PresentLabel;
        public int Quantity = 1;
        public TextMeshProUGUI LabelText;
        public TextMeshProUGUI QuantityText;
        public Image BackgroundImage;
        public Color BackgroundColor = Color.white;
        public Image PresentImage;
        public ItemColorShift PresentColorShift;

        [Tooltip("Rotation in degrees per second")]
        public float RotationSpeed = 45.0f;

        protected void Start()
        {
            if (string.IsNullOrEmpty(PresentLabel))
            {
                LabelText.gameObject.SetActive(false);
            }
            else
            {
                LabelText.text = PresentLabel;
            }

            if (Quantity == 1)
            {
                QuantityText.gameObject.SetActive(false);
            }
            else
            {
                QuantityText.text = Quantity.ToString();
            }

            BackgroundImage.color = new Color(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, 0.5f);
            PresentImage.sprite = PresentSprite;
            PresentImage.SetMaterialForItem(PresentColorShift);
        }

        protected void Update()
        {
            BackgroundImage.transform.Rotate(Vector3.forward, Time.deltaTime * RotationSpeed);
        }
    }
}