using UnityEngine;
using System.Collections.Generic;

namespace Tofuwu.StackCats
{
    public delegate void CutSceneItemLoaded(CutSceneItem cutSceneItem);
    public delegate void CutSceneStarted(CutScene cutScene);
    public delegate void CutSceneEnded(CutScene cutScene);

    public class CutScene : MonoBehaviour
    {
        /// <summary>
        /// Invoked whenever a new cut scene is loaded.
        /// </summary>
        public event CutSceneItemLoaded onCutSceneItemLoaded;

        /// <summary>
        /// Invoked when the cut scene starts.
        /// </summary>
        public event CutSceneEnded onCutSceneStarted;

        /// <summary>
        /// Invoked when the cut scene ends.
        /// </summary>
        public event CutSceneEnded onCutSceneEnded;

        /// <summary>
        /// The cut scene items to be played.
        /// </summary>
        public List<CutSceneItem> CutSceneItems;

        public bool IsChatCatDoneTalking;

        private ChatCat _chatCat;
        private int _currentCutSceneItemIndex = -1;
        private float _cutSceneTimeElapsed;
        private float _cutSceneItemDuration;

        public void GoToNextCutSceneItem()
        {
            if (_currentCutSceneItemIndex != -1)
            {
                CutSceneItem currentCutSceneItem = CutSceneItems[_currentCutSceneItemIndex];
                if (_currentCutSceneItemIndex != CutSceneItems.Count - 1)
                {
                    currentCutSceneItem.gameObject.SetActive(false);
                }
            }

            ++_currentCutSceneItemIndex;
            if (CutSceneItems.Count <= _currentCutSceneItemIndex)
            {
                if (onCutSceneEnded != null) onCutSceneEnded(this);
                return;
            }

            CutSceneItem cutSceneItem = CutSceneItems[_currentCutSceneItemIndex];
            cutSceneItem.gameObject.SetActive(true);
            _cutSceneItemDuration = cutSceneItem.Duration;
            _cutSceneTimeElapsed = 0.0f;

            IsChatCatDoneTalking = cutSceneItem.Expressions.Count == 0;
            foreach(ChatCatExpression expression in cutSceneItem.Expressions)
            {
                _chatCat.Say(expression.Message, expression.Emote, expression.LetterInterval);
            }

            if (cutSceneItem.StartBackgroundMusic || !cutSceneItem.InheritBackgroundMusic)
            {
                GameManager.Instance.Audio.BackgroundMusic.PlayAudioLoop(cutSceneItem.StartBackgroundMusic);
            }

            if (onCutSceneItemLoaded != null) onCutSceneItemLoaded(cutSceneItem);
        }

        protected void Awake()
        {
            _chatCat = GameManager.Instance.ChatCat;
        }

        protected void Start()
        {
            GoToNextCutSceneItem();
            if (onCutSceneStarted != null) onCutSceneStarted(this);
        }

        protected void Update()
        {
            _cutSceneTimeElapsed += Time.deltaTime;

            if (_cutSceneTimeElapsed > _cutSceneItemDuration && IsChatCatDoneTalking)
            {
                GoToNextCutSceneItem();
            }
        }
    }
}