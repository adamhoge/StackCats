using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Currency Details", menuName = "Stack Cats/Currency Details")]
    public class CurrencyDetails : ScriptableObject
    {
        public string Name;
        public string Abbreviation;
        public PuzzleArea PuzzleArea;
        public Sprite IconSprite;
        public Color Color;
    }
}