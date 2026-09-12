using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Story Puzzle", menuName = "Stack Cats/Story Puzzle")]
    public class StoryPuzzle : ScriptableObject, IIdentifiable
    {
        [TextArea]
        public string JsonData;

        [SerializeField]
        public List<int> StarMoveRequirements = new List<int>();

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