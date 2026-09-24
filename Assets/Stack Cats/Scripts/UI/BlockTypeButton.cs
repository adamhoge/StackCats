using System;
using TMPro;
using UnityEngine;

namespace RockhopperGames.StackCats.UI
{
    public class BlockTypeButton : MonoBehaviour
    {
        public TextMeshProUGUI TypeLabelTextMesh;

        public Type BlockType { get; set; }

        protected void Start()
        {
            TypeLabelTextMesh.text = BlockType.ToString();
        }
    }
}
