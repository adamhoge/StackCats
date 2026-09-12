using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class ChallengeAreaInfoUI : MonoBehaviour
    {
        public PuzzleArea PuzzleArea;
        public bool IsLocked;
        public Button Button;
        public Image PuzzleButtonImage;
        public TextMeshProUGUI PuzzleAreaTitleText;
        public RectTransform LockedInfoRectTransform;

        protected void Start()
        {
            if (!IsLocked)
            {
                PuzzleButtonImage.color = PuzzleArea.PuzzleTheme.UIColor;
                PuzzleAreaTitleText.text = PuzzleArea.AreaTitle;
                LockedInfoRectTransform.gameObject.SetActive(false);
            }
            else
            {
                PuzzleAreaTitleText.gameObject.SetActive(false);
                Button.interactable = false;
            }
        }
    }
}