using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Block Tutorial", menuName = "Stack Cats/Tutorial/Block Tutorial")]
    public class BlockTutorial : Tutorial
    {
        [TextArea]
        public string BlockInformation;

        [TextArea]
        public string DemoPuzzleJsonData;

        public List<DemoMove> DemoMoves;
    }
}