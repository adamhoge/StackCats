using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    [Serializable]
    public class EmoteSpritesDictionary : SerializableDictionaryBase<ChatCatEmote, Sprite> { }

    public class ChatCatOverlayScreen : OverlayScreen
    {
        public CanvasGroup PortraitCanvasGroup;
        public CanvasGroup MessageCanvasGroup;
        public CanvasGroup NameLabelCanvasGroup;
        public TextMeshProUGUI MessageText;
        public Image EmoteImage;
        public RectTransform PromptContinueRectTransform;
        public Queue<ChatCatExpression> MessageQueue = new Queue<ChatCatExpression>();
        public EmoteSpritesDictionary EmoteSprites;

        private static string HIDE_BEGIN = "<color=#00000000>";
        private static string HIDE_END = "</color>";
        private ChatCatExpression _currentMessage;
        private int _currentLetterIndex;
        private float _lastLetterOutputTime;
        private Vector2 _promptContinueRectTransformPosition;

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            MessageText.text = "";
            _currentMessage = null;
            EmoteImage.sprite = null;
            EmoteImage.enabled = false;
            _currentLetterIndex = 0;
            _lastLetterOutputTime = 0.0f;

            LeanTween.cancel(EmoteImage.gameObject);

            PortraitCanvasGroup.alpha = 0.0f;
            LeanTween.cancel(PortraitCanvasGroup.gameObject);
            LeanTween.alphaCanvas(PortraitCanvasGroup, 1.0f, TransitionInDuration).setEase(TransitionInTween);

            MessageCanvasGroup.alpha = 0.0f;
            LeanTween.cancel(MessageCanvasGroup.gameObject);
            LeanTween.alphaCanvas(MessageCanvasGroup, 1.0f, TransitionInDuration).setEase(TransitionInTween);

            NameLabelCanvasGroup.alpha = 0.0f;
            LeanTween.cancel(NameLabelCanvasGroup.gameObject);
            LeanTween.alphaCanvas(NameLabelCanvasGroup, 1.0f, TransitionInDuration).setEase(TransitionInTween);

            LoadNextMessage();
        }

        public override void OnActive()
        {
            base.OnActive();
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            LeanTween.cancel(PortraitCanvasGroup.gameObject);
            LeanTween.alphaCanvas(PortraitCanvasGroup, 0.0f, TransitionOutDuration).setEase(TransitionOutTween);

            LeanTween.cancel(MessageCanvasGroup.gameObject);
            LeanTween.alphaCanvas(MessageCanvasGroup, 0.0f, TransitionOutDuration).setEase(TransitionOutTween);

            LeanTween.cancel(NameLabelCanvasGroup.gameObject);
            LeanTween.alphaCanvas(NameLabelCanvasGroup, 0.0f, TransitionOutDuration).setEase(TransitionOutTween);

            LeanTween.cancel(PromptContinueRectTransform.gameObject);
        }

        protected void Awake()
        {
            _promptContinueRectTransformPosition = PromptContinueRectTransform.localPosition;
        }

        protected override void Update()
        {
            base.Update();

            if (_isActive && _currentMessage != null)
            {
                UpdateMessageText();

                if (Input.GetMouseButtonDown(0))
                {
                    if (_currentLetterIndex == _currentMessage.Message.Length)
                    {
                        LoadNextMessage();
                    }
                    else
                    {
                        SkipMessageFeed();
                    }
                }
            }
        }

        private void UpdateMessageText()
        {
            if (_currentLetterIndex < _currentMessage.Message.Length && Time.time > _lastLetterOutputTime + _currentMessage.LetterInterval)
            {
                _lastLetterOutputTime = Time.time;

                ++_currentLetterIndex;

                string formattedMessage = _currentMessage.Message;
                if (_currentLetterIndex < _currentMessage.Message.Length)
                {
                    formattedMessage = formattedMessage.Insert(_currentLetterIndex, HIDE_BEGIN);
                    formattedMessage += HIDE_END;
                }
                else
                {
                    ShowPromptContinue();
                }

                MessageText.text = formattedMessage;
            }
        }

        private void LoadNextMessage()
        {
            HidePromptContinue();

            if (MessageQueue.Count > 0)
            {
                _currentMessage = MessageQueue.Dequeue();
                Sprite emoteSprite = EmoteSprites[_currentMessage.Emote];
                SetEmoteSprite(emoteSprite);
                _currentLetterIndex = 0;
            }
            else
            {
                Dismiss();
            }
        }

        private void SetEmoteSprite(Sprite sprite)
        {
            if (EmoteImage.sprite == sprite) return;

            EmoteImage.sprite = sprite;
            if (EmoteImage.enabled)
            {
                LeanTween.cancel(EmoteImage.gameObject);
                LeanTween.scale(EmoteImage.gameObject, Vector3.one * 1.25f, 0.5f).setEase(LeanTweenType.punch);
            }
            else
            {
                EmoteImage.enabled = true;
            }
        }

        private void SkipMessageFeed()
        {
            _currentLetterIndex = _currentMessage.Message.Length;
            MessageText.text = _currentMessage.Message;
        }

        private void ShowPromptContinue()
        {
            PromptContinueRectTransform.gameObject.SetActive(true);
            PromptContinueRectTransform.localPosition = _promptContinueRectTransformPosition;
            LeanTween.moveLocalY(PromptContinueRectTransform.gameObject, PromptContinueRectTransform.localPosition.y + 20.0f, 0.5f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong();
        }

        private void HidePromptContinue()
        {
            PromptContinueRectTransform.gameObject.SetActive(false);
            LeanTween.cancel(PromptContinueRectTransform.gameObject);
        }
    }
}