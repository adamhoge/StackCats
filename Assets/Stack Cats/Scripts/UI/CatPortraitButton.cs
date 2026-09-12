using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(Button))]
    public class CatPortraitButton : MonoBehaviour
    {
        public Cat Cat;
        public Image PortraitImage;
        public Sprite NotSeenSprite;
        public Image BackgroundImage;
        public Image BorderImage;
        public TextMeshProUGUI NameText;
        public Color CommonColor;
        public Color UncommonColor;
        public Color RareColor;
        public Color NotBondedColor = new(0, 0, 0, 0.1f);
        public Color NotSeenColor = new(0, 0, 0, 0.05f);

        /// <summary>
        /// The button associated with the cat portrait.
        /// </summary>
        public Button Button { get { return _button; } }

        private Button _button;
        private CatManager _cats;

        protected void Awake()
        {
            _button = GetComponent<Button>();
            _cats = GameManager.Instance.Cats;
        }

        protected void Start()
        {
            // Load button styles based on cat information.
            if (!Cat)
            {
                NameText.text = "[Missing]";
            }
            else
            {
                bool wasSeen = _cats.WasCatSeen(Cat);
                bool isBonded = _cats.IsBonded(Cat);

                // Set the portrait.
                PortraitImage.sprite = wasSeen ? Cat.Portrait : NotSeenSprite;

                // Set the name text.
                NameText.text = isBonded ? Cat.Name : "???";

                // Set the portrait image color.
                if (!wasSeen)
                {
                    PortraitImage.color = NotSeenColor;
                }
                else if (!isBonded)
                {
                    PortraitImage.color = NotBondedColor;
                }

                // Set the border color.
                Color borderColor = Color.clear;
                switch (Cat.Rarity)
                {
                    case Rarity.Common: borderColor = CommonColor; break;
                    case Rarity.Uncommon: borderColor = UncommonColor; break;
                    case Rarity.Rare: borderColor = RareColor; break;
                }
                BorderImage.color = borderColor;

                // Set button activity.
                _button.interactable = wasSeen;
            }
        }
    }
}