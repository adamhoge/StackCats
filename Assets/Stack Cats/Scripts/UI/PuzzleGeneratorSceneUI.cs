using TMPro;
using UnityEngine;

namespace RockhopperGames.StackCats
{
    public class PuzzleGeneratorSceneUI : MonoBehaviour
    {
        public PuzzleGeneratorScene PuzzleGeneratorScene;
        public TextMeshProUGUI EstimatedDifficultyText;
        public TextMeshProUGUI NumMovesMadeText;

        protected void Update()
        {
            EstimatedDifficultyText.text =
                PuzzleGeneratorScene.CurrentPuzzleEstimatedDifficulty.ToString();
            NumMovesMadeText.text = PuzzleGeneratorScene.CurrentPuzzleNumMovesMade.ToString();
        }
    }
}
