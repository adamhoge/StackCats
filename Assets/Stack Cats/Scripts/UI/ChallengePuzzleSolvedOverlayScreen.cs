using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class ChallengePuzzleSolvedOverlayScreen : PuzzleSolvedOverlayScreen
    {
        protected new void Update()
        {
            base.Update();

            if (_isActive && _activeTimeElapsed > 1.0f && Input.GetMouseButtonDown(0))
            {
                Dismiss();
            }
        }
    }
}