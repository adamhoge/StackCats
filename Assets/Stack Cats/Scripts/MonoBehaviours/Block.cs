using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    /// <summary>
    /// Base object for any item that can be added to a stack.
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Block : MonoBehaviour
    {
        public Stack ParentStack;
        public SpriteRenderer BlockShine;
        public SpriteRenderer BlockOverlay;
        public SpriteMask BlockOverlayMask;
        public AudioEvent KnockSoundEffect;
        public AudioEvent SelectSoundEffect;

        public Block GetBlockBelow()
        {
            if (ParentStack)
            {
                int blockIndex = ParentStack.Blocks.IndexOf(this);
                if (blockIndex > 0) return ParentStack.Blocks[blockIndex - 1];
            }

            return null;
        }

        public List<Block> GetBlocksBelow()
        {
            List<Block> blocksBelow = new List<Block>();

            if (ParentStack)
            {
                int blockIndex = ParentStack.Blocks.IndexOf(this);
                for (int i = blockIndex - 1; i >= 0; i--)
                {
                    blocksBelow.Add(ParentStack.Blocks[i]);
                }
            }

            return blocksBelow;
        }

        public Block GetBlockAbove()
        {
            if (ParentStack)
            {
                int nextBlockIndex = ParentStack.Blocks.IndexOf(this) + 1;
                if (nextBlockIndex < ParentStack.Blocks.Count) return ParentStack.Blocks[nextBlockIndex];
            }

            return null;
        }

        public List<Block> GetBlocksAbove()
        {
            List<Block> blocksAbove = new List<Block>();

            if (ParentStack)
            {
                int blockIndex = ParentStack.Blocks.IndexOf(this);
                for (int i = blockIndex + 1; i < ParentStack.Blocks.Count; i++)
                {
                    blocksAbove.Add(ParentStack.Blocks[i]);
                }
            }

            return blocksAbove;
        }

        public bool HasBlockBelow<T>() where T : BlockComponent
        {
            Block blockBelow = GetBlockBelow();
            return blockBelow ? blockBelow.GetComponent<T>() != null : false;
        }

        public void FlashBlock(Color color, float duration = 1.0f, LeanTweenType tweenType = LeanTweenType.easeInQuint)
        {
            LeanTween.cancel(BlockOverlay.gameObject);
            BlockOverlay.color = color;
            LeanTween.color(BlockOverlay.gameObject, new Color(color.r, color.g, color.b, 0.0f), duration).setEase(tweenType);
        }

        public void KnockBlock()
        {
            LeanTween.cancel(gameObject);
            transform.localPosition = ParentStack ? ParentStack.GetBlockLocalPosition(this) : Vector3.zero;
            LeanTween.moveLocalX(gameObject, 0.025f, 0.15f).setEase(LeanTweenType.easeShake);
            GameManager.Instance.Audio.PlaySoundEffect(KnockSoundEffect);
        }

        public void SelectBlock()
        {
            GameManager.Instance.Audio.PlaySoundEffect(SelectSoundEffect);
        }
    }
}