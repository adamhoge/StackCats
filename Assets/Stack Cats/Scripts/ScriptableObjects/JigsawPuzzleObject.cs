using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Jigsaw Puzzle Object", menuName = "Stack Cats/Jigsaw Puzzle Object")]
    public class JigsawPuzzleObject : ScriptableObject, IIdentifiable
    {
        public List<Sprite> JigsawSprites;

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