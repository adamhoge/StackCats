using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Minigame Information", menuName = "Stack Cats/Minigame Information")]
    public class MinigameInformation : ScriptableObject, IIdentifiable
    {
        public string MinigameTitle;
        public Sprite TitleScreenSprite;
        public Minigame MinigamePrefab;
        public MinigameUI MinigameUIPrefab;
        public List<Cat> RequiredCats;
        public int MinimumCats = 1;
        public int MaximumCats = 4;

        [SerializeField]
        [HideInInspector]
        private string _id;

        public string GetId()
        {
            if (string.IsNullOrEmpty(_id)) { _id = Guid.NewGuid().ToString(); }

            return _id;
        }
    }
}