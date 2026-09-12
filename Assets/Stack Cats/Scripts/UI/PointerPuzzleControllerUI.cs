using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    public class PointerPuzzleControllerUI : PuzzleControllerUI
    {
        public Sprite PlaceholderSprite;

        private List<Image> _placeholderImages = new List<Image>();

        protected override void OnBlocksBlocksSelected(PuzzleMarker marker)
        {
            base.OnBlocksBlocksSelected(marker);

            List<Block> blocksSelected = marker.Stack.GetBlocksAt(marker.Block);
            foreach(Block block in blocksSelected)
            {
                Image placeholderImage = new GameObject("Placeholder Block").AddComponent<Image>();
                placeholderImage.transform.SetParent(WorldCanvas.transform);
                placeholderImage.sprite = PlaceholderSprite;
                placeholderImage.color = new Color(1.0f, 1.0f, 1.0f, 0.75f);
                placeholderImage.rectTransform.sizeDelta = Vector3.one;
                placeholderImage.rectTransform.position = _currentPuzzle.transform.position + _currentPuzzle.GetStackLocalPosition(marker.Stack) + marker.Stack.GetBlockLocalPosition(block) + (Vector3.up * 0.5f);
                _placeholderImages.Add(placeholderImage);
            }
        }

        protected override void OnCancelled(PuzzleMarker marker)
        {
            base.OnCancelled(marker);

            foreach(Image placeholderImage in _placeholderImages)
            {
                Destroy(placeholderImage.gameObject);
            }
            _placeholderImages.Clear();
        }
    }
}