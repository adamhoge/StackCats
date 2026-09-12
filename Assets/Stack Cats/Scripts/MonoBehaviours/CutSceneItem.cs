using System;
using System.Collections.Generic;
using Tofuwu.StackCats.UI;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void CutSceneItemEnded();

    public enum LeanTweenMotion
    {
        Scale,
        MoveY
    }

    [Serializable]
    public class TweenInfo
    {
        public LeanTweenMotion Motion;
        public LeanTweenType Type;
        public float Value;
        public float Delay;
        public float Duration;
        public bool LoopPingPong;
    }

    [Serializable]
    public class CutSceneItemSpriteTween
    {
        public SpriteRenderer SpriteRenderer;
        public List<TweenInfo> Tweens;
    }

    public class CutSceneItem : MonoBehaviour
    {
        public event CutSceneItemEnded onCutSceneItemEnded;

        public string Narrative;
        public float Duration = 5.0f;
        public List<ChatCatExpression> Expressions;
        public AudioLoop StartBackgroundMusic;
        public bool InheritBackgroundMusic = true;
        public List<CutSceneItemSpriteTween> SpriteTweens;

        private float _timeElapsed;

        public void EndCutSceneItem()
        {
            if (onCutSceneItemEnded != null) onCutSceneItemEnded();
        }

        protected void OnEnable()
        {
            _timeElapsed = 0.0f;

            foreach(CutSceneItemSpriteTween spriteTween in SpriteTweens)
            {
                foreach(TweenInfo tweenInfo in spriteTween.Tweens)
                {
                    switch (tweenInfo.Motion)
                    {
                        case LeanTweenMotion.Scale:
                            LTDescr scale = LeanTween.scale(spriteTween.SpriteRenderer.gameObject, Vector3.one * tweenInfo.Value, tweenInfo.Duration)
                                .setEase(tweenInfo.Type)
                                .setDelay(tweenInfo.Delay);
                            if (tweenInfo.LoopPingPong) scale.setLoopPingPong();
                            break;
                        case LeanTweenMotion.MoveY:
                            LTDescr moveY = LeanTween.moveLocalY(spriteTween.SpriteRenderer.gameObject, tweenInfo.Value, tweenInfo.Duration)
                                .setEase(tweenInfo.Type)
                                .setDelay(tweenInfo.Delay);
                            if (tweenInfo.LoopPingPong) moveY.setLoopPingPong();
                            break;
                    }
                }
            }
        }

        protected void Update()
        {
            _timeElapsed += Time.deltaTime;

            if (_timeElapsed >= Duration)
            {
                EndCutSceneItem();
            }
        }
    }
}