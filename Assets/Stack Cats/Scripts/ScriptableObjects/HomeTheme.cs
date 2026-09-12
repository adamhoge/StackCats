using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Home Theme", menuName = "Stack Cats/Home Theme")]
    public class HomeTheme : ScriptableObject
    {
        public AudioClip BackgroundMusic;
        public Color CameraBackgroundColor;
        public Sprite BackgroundSprite;
        public Sprite DrawerButtonSprite;
        public Color PiggyBankColor = Color.white;
        public ColorBlock ButtonColors;
        public Color ButtonTextColor;
    }
}
