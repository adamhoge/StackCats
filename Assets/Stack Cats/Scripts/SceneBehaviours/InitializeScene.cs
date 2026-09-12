using UnityEngine;

namespace Tofuwu.StackCats
{
    public class InitializeScene : SceneBehaviour
    {
        protected new void Start()
        {
            base.Start();

            GameManager gameManager = GameManager.Instance;
            gameManager.GoToTitleScreen();
        }
    }
}