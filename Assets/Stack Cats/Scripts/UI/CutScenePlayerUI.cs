using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tofuwu.StackCats.UI
{
    public class CutScenePlayerUI : MonoBehaviour
    {
        public CutScenePlayer CutScenePlayer;
        public RectTransform NarrativeRectTransform;
        public TextMeshProUGUI NarrativeText;
        public OverlayScreenManager OverlayScreenManager;
        public ChatCatOverlayScreen ChatCatOverlayScreen;

        private CutScene _currentCutScene;

        public void OnEnable()
        {
            GameManager.Instance.ChatCat.onTalking += OnTalking;
            CutScenePlayer.onStartCutScene += OnPlayCutScene;
            CutScenePlayer.onStopCutScene += OnStopCutScene;
        }

        public void OnDisable()
        {
            GameManager.Instance.ChatCat.onTalking -= OnTalking;
            CutScenePlayer.onStartCutScene -= OnPlayCutScene;
            CutScenePlayer.onStopCutScene -= OnStopCutScene;
        }

        private void OnTalking(ChatCatExpression expression)
        {
            ChatCatOverlayScreen.MessageQueue.Enqueue(expression);
            if (!ChatCatOverlayScreen.DisplayedBy) OverlayScreenManager.EnqueueScreen(ChatCatOverlayScreen);
        }

        private void OnPlayCutScene(CutScene cutScene)
        {
            _currentCutScene = cutScene;
            cutScene.onCutSceneItemLoaded += OnCutSceneItemLoaded;
        }

        private void OnChatCatDoneTalking()
        {
            ChatCatOverlayScreen.onTransitioningOut -= OnChatCatDoneTalking;
            _currentCutScene.IsChatCatDoneTalking = true;
        }

        private void OnStopCutScene(CutScene cutScene)
        {
            cutScene.onCutSceneItemLoaded -= OnCutSceneItemLoaded;
            _currentCutScene = null;
        }

        private void OnCutSceneItemLoaded(CutSceneItem cutSceneItem)
        {
            if (!_currentCutScene.IsChatCatDoneTalking)
            {
                ChatCatOverlayScreen.onHidden += OnChatCatDoneTalking;
            }

            if (!string.IsNullOrEmpty(cutSceneItem.Narrative))
            {
                NarrativeRectTransform.gameObject.SetActive(true);
                NarrativeText.text = cutSceneItem.Narrative;
            }
            else
            {
                NarrativeRectTransform.gameObject.SetActive(false);
            }
        }
    }
}