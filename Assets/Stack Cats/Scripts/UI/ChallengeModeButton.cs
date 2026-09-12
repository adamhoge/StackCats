using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(Button))]
    public class ChallengeModeButton : MonoBehaviour
    {
        [Serializable]
        public class ChallengeModeTrophy
        {
            public Image StarImage;
            public CanvasGroup StarFlashCanvasGroup;
        }

        public List<ChallengeModeTrophy> ChallengeModeTrophies;
        public List<int> CompletedDifficulties;
        public Color HasntTrophyColor;

        public Button Button { get { return _button; } }

        private Button _button;

        protected void Awake()
        {
            _button = GetComponent<Button>();
        }

        protected void Start()
        {
            for (int i = 0; i < 5; i++)
            {
                // TODO: Set trophy icon.

                if (CompletedDifficulties.Contains(i))
                {
                    ChallengeModeTrophies[i].StarImage.color = Color.white;
                }
                else
                {
                    ChallengeModeTrophies[i].StarImage.color = HasntTrophyColor;
                }
            }
        }
    }
}