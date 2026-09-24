using System;
using System.Collections.Generic;
using UnityEngine;

namespace RockhopperGames.StackCats.UI
{
    public class PuzzleRulesOverlayScreen : OverlayScreen
    {
        public BlockTypeButton BlockTypeButtonPrefab;

        public RectTransform BlockTypeButtonContainer;

        private PuzzleScene _puzzleScene;
        private List<BlockTypeButton> _blockTypeButtonInstances = new();

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            foreach (var blockTypeButton in _blockTypeButtonInstances)
            {
                Destroy(blockTypeButton.gameObject);
            }
            _blockTypeButtonInstances.Clear();

            _puzzleScene = FindAnyObjectByType<PuzzleScene>();
            List<Type> blockTypes = _puzzleScene.PuzzleLoader.Puzzle.GetAllBlockTypes();

            foreach (var blockType in blockTypes)
            {
                var blockTypeButton = Instantiate(BlockTypeButtonPrefab, BlockTypeButtonContainer);
                blockTypeButton.BlockType = blockType;
                blockTypeButton.TypeLabelTextMesh.text = blockType.ToString();
                _blockTypeButtonInstances.Add(blockTypeButton);
            }
        }
    }
}
