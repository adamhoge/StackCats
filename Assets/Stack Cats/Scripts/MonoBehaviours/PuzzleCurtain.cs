using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    /// <summary>
    /// Used in a puzzle to overlap the area with which the player has to work.
    /// Reports any overlapped puzzle blocks.
    /// </summary>
    public class PuzzleCurtain : MonoBehaviour
    {
        /// <summary>
        /// The puzzle to which the curtain is attached.
        /// </summary>
        public Puzzle Puzzle;

        /// <summary>
        /// The max block height of the curtain.
        /// </summary>
        public int MaxHeight = 10;

        /// <summary>
        /// The total duration of a curtain drop.
        /// </summary>
        public float CurtainDropDuration = 0.4f;

        public float CurtainShakeAmount = 0.1f;

        public float CurtainShakeDuration = 0.25f;

        /// <summary>
        /// The sprite that represents the curtain.
        /// </summary>
        public SpriteRenderer CurtainSprite;

        /// <summary>
        /// The current block height of the curtain.
        /// </summary>
        public int Height { get { return _height; } set { SetHeight(value, false); } }

        /// <summary>
        /// Set whether or not the curtain is shaking
        /// </summary>
        public bool IsShaking { get { return _isShaking; } set { SetIsShaking(value); } }

        private int _height;
        private bool _isShaking;
        private float _paddingTop;
        private int _lastCurtainHeight = 0;
        private int? _curtainDropTweenId;
        private int? _curtainShakeTweenId;

        /// <summary>
        /// Raise the curtain by one block.
        /// </summary>
        public void Raise(bool isAnimated = true)
        {
            Drop(-1, isAnimated);
        }

        /// <summary>
        /// Drop the curtain by one block.
        /// </summary>
        public void Drop(bool isAnimated = true)
        {
            Drop(1, isAnimated);
        }

        /// <summary>
        /// Drop the curtain by the specified number of blocks.
        /// </summary>
        /// <param name="amount">The amount by which to drop the curtain.</param>
        public void Drop(int amount, bool isAnimated = true)
        {
            SetHeight(_height + amount, isAnimated);
        }

        /// <summary>
        /// Lift the curtain by one block.
        /// </summary>
        public void Lift()
        {
            Lift(1);
        }

        /// <summary>
        /// Lift the curtain by the specified number of blocks.
        /// </summary>
        /// <param name="amount"></param>
        public void Lift(int amount, bool isAnimated = true)
        {
            SetHeight(_height - amount, isAnimated);
        }

        /// <summary>
        /// Get a list of blocks covered by the curtain.
        /// </summary>
        /// <returns>A list of blocks covered by the curtain.</returns>
        public List<Block> GetCoveredBlocks()
        {
            List<Block> blocks = new List<Block>();

            if (!Puzzle) return blocks;

            foreach (Stack stack in Puzzle.Stacks)
            {
                blocks.AddRange(stack.GetBlocksBetween(MaxHeight - _height, MaxHeight));
            }

            return blocks;
        }

        protected void Start()
        {
            _paddingTop = 4;
            MaxHeight = Puzzle.MaxStackHeight;

            float curtainScaleY = _height + _paddingTop;
            CurtainSprite.size = new Vector2(CurtainSprite.size.x, curtainScaleY);
            CurtainSprite.transform.position = new Vector2(0.0f, MaxHeight + _paddingTop);
        }

        private void UpdateCurtainTransform(bool isAnimated)
        {
            float moveAmount = _height - _lastCurtainHeight;
            _lastCurtainHeight = _height;

            float curtainScaleY = _height + _paddingTop;
            CurtainSprite.size = new Vector2(CurtainSprite.size.x, curtainScaleY);
            if (_curtainDropTweenId.HasValue) LeanTween.cancel(_curtainDropTweenId.Value);

            if (isAnimated)
            {
                CurtainSprite.transform.Translate(Vector2.up * moveAmount);
                _curtainDropTweenId = LeanTween.moveLocalY(CurtainSprite.gameObject, MaxHeight + _paddingTop, CurtainDropDuration)
                    .setEase(LeanTweenType.easeOutBack)
                    .id;
            }
            else
            {
                _curtainDropTweenId = null;
                CurtainSprite.transform.localPosition = new Vector2(0.0f, MaxHeight + _paddingTop);
            }
        }

        private void SetHeight(int height, bool isAnimated)
        {
            _height = Mathf.Clamp(height, 0, MaxHeight);
            UpdateCurtainTransform(isAnimated);
        }

        private void SetIsShaking(bool value)
        {
            if (value == _isShaking) return;

            _isShaking = value;

            if (_isShaking)
            {
                _curtainShakeTweenId = LeanTween.moveLocalX(CurtainSprite.gameObject, CurtainShakeAmount, CurtainShakeDuration)
                    .setLoopPingPong()
                    .id;
            }
            else if (_curtainShakeTweenId.HasValue)
            {
                LeanTween.cancel(_curtainShakeTweenId.Value);
                var curtainSpritePosition = CurtainSprite.transform.position;
                CurtainSprite.transform.position = new Vector3(0.0f, curtainSpritePosition.y, curtainSpritePosition.z);
            }
        }
    }
}
