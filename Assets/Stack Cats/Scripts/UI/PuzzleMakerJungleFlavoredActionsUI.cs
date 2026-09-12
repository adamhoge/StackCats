using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PuzzleMakerJungleFlavoredActionsUI : PuzzleMakerActionsUI
    {
        public Button SetJigsawBlocksButton;

        protected override void OnEnable()
        {
            base.OnEnable();

            SetJigsawBlocksButton.onClick.AddListener(OnSetJigsawBlocksButtonClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SetJigsawBlocksButton.onClick.AddListener(OnSetJigsawBlocksButtonClicked);
        }

        protected override void OnAddBlockTypeChanged(BlockType blockType)
        {
            base.OnAddBlockTypeChanged(blockType);

            SetJigsawBlocksButton.image.color =
                blockType == BlockType.CatBlock ? BlockSelectionColor : Color.white;
        }

        private void OnSetJigsawBlocksButtonClicked()
        {
            PuzzleMakerActionsController.ChangeBlockType(BlockType.CatBlock);
        }
    }
}
