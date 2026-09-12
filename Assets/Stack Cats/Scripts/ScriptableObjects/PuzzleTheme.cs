using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Puzzle Theme", menuName = "Stack Cats/Puzzle Theme")]
    public class PuzzleTheme : ScriptableObject
    {
        public GameObject PuzzleScenaryPrefab;
        public GameObject HomeScenaryPrefab;
        public Sprite TopBoundarySprite;
        public float TopBoundaryOffset;
        public Sprite BottomBoundarySprite;
        public float BottomBoundaryOffset;
        public Color BackgroundColor = Color.white;
        public Color BackgroundGradient = Color.clear;
        public Color AmbientColor = Color.white;
        public Color UIColor = Color.white;
        public AudioLoop BackgroundMusic;
    }
}
