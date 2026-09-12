using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tofuwu.StackCats.UI
{
    [RequireComponent(typeof(Button))]
    public class FeatureButton : MonoBehaviour
    {
        public string FeatureLabel;
        public CanvasGroup HighlightCanvasGroup;

        private DataManager _dataManager;
        private Button _button;

        protected void Awake()
        {
            _dataManager = GameManager.Instance.Data;
            _button = GetComponent<Button>();
        }

        protected void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        protected void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        protected void Start()
        {
            LeanTween.alphaCanvas(HighlightCanvasGroup, 0.5f, 0.5f).setEase(LeanTweenType.easeOutSine).setLoopPingPong();
        }

        protected void Update()
        {
            GameObject highlightGameObject = HighlightCanvasGroup.gameObject;
            highlightGameObject.SetActive(highlightGameObject.activeSelf && _button.interactable && !_dataManager.FlagData.IsFlagSet(FeatureLabel));
        }

        private void OnButtonClicked()
        {
            _dataManager.FlagData.SetFlag(FeatureLabel, true);
        }
    }
}