using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    [RequireComponent(typeof(Button))]
    public class ItemButton : MonoBehaviour
    {
        public Item Item;
        public Image ButtonImage;

        public Button Button { get { return _button; } }

        private Button _button;

        protected void Awake()
        {
            _button = GetComponent<Button>();
        }

        protected void Start()
        {
            ButtonImage.sprite = Item.Icon;
            ButtonImage.SetMaterialForItem(Item.ColorShift);
        }
    }
}