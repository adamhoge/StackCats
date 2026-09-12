using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class ChatCatUI : MonoBehaviour
    {
        public OverlayScreenManager OverlayScreenManager;
        public ChatCatOverlayScreen ChatCatOverlayScreen;
        public PuzzleLoader PuzzleLoader;
        public Button ChatButton;

        public void OpenChat()
        {
            if (!ChatCatOverlayScreen.DisplayedBy)
            {
                OverlayScreenManager.EnqueueScreen(ChatCatOverlayScreen);
                ChatButton.interactable = false;
            }
        }

        protected void OnEnable()
        {
            GameManager.Instance.ChatCat.onTalking += OnTalking;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;
            PuzzleLoader.onPuzzleRestarted += OnPuzzleUnloaded;
        }

        protected void OnDisable()
        {
            GameManager.Instance.ChatCat.onTalking -= OnTalking;
            PuzzleLoader.onPuzzleUnloaded -= OnPuzzleUnloaded;
            PuzzleLoader.onPuzzleRestarted -= OnPuzzleUnloaded;
        }

        private void OnTalking(ChatCatExpression expression)
        {
            ChatCatOverlayScreen.MessageQueue.Enqueue(expression);

            if (!ChatCatOverlayScreen.DisplayedBy)
            {
                ChatButton.interactable = true;
            }
        }

        private void OnPuzzleUnloaded(Puzzle puzzle)
        {
            ChatCatOverlayScreen.MessageQueue.Clear();
            ChatButton.interactable = false;
        }
    }
}