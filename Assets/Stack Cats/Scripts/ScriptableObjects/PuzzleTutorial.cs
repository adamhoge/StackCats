using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Puzzle Tutorial", menuName = "Stack Cats/Tutorial/Puzzle Tutorial")]
    public class PuzzleTutorial : Tutorial
    {
        public PuzzleArea PuzzleArea;
        
        public string ObjectiveSummary;
        
        [TextArea]
        public string ObjectiveDetails;

        [TextArea]
        public string DemoPuzzleJsonData;
        
        public List<DemoMove> DemoMoves;
    }
}