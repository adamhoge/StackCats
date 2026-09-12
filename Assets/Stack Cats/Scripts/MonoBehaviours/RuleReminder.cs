using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class RuleReminder : MonoBehaviour
    {
        public PuzzleLoader PuzzleLoader;
        public float RuleReminderDelay = 1.0f;
        public TextMeshProUGUI RuleReminderText;
        public CanvasGroup RuleReminderCanvasGroup;

        private Puzzle _currentPuzzle;
        private PuzzleController _currentPuzzleController;
        private string _ruleDescription;
        private float _ruleDescriptionTime;
        private bool _ruleDescriptionPending;

        protected void Awake()
        {
            if (!PuzzleLoader)
            {
                Debug.LogError("PuzzleLoader was not provided for " + gameObject.name + ". Destroying gameObject.");
                Destroy(gameObject);
                return;
            }

            RuleReminderText.text = "";
            RuleReminderCanvasGroup.alpha = 0.0f;
            PuzzleLoader.onPuzzleLoaded += OnPuzzleLoaded;
            PuzzleLoader.onPuzzleUnloaded += OnPuzzleUnloaded;
        }

        protected void Update()
        {
            if (_ruleDescriptionPending && Time.time > _ruleDescriptionTime + RuleReminderDelay)
            {
                RuleReminderText.text = _ruleDescription;
                LeanTween.alphaCanvas(RuleReminderCanvasGroup, 1.0f, 0.25f).setEase(LeanTweenType.easeOutSine);
                _ruleDescriptionPending = false;
            }
        }

        protected virtual void OnPuzzleLoaded(Puzzle puzzle)
        {
            _currentPuzzle = puzzle;
            _currentPuzzleController = puzzle.GetComponent<PuzzleController>();

            if (_currentPuzzleController)
            {
                _currentPuzzleController.onChecked += OnChecked;
                _currentPuzzleController.onCancelled += OnCancelled;
            }
        }

        protected virtual void OnPuzzleUnloaded(Puzzle puzzle)
        {
            _currentPuzzle = null;
            _currentPuzzleController = null;

            if (_currentPuzzleController)
            {
                _currentPuzzleController.onChecked -= OnChecked;
                _currentPuzzleController.onCancelled -= OnCancelled;
            }
        }

        protected virtual void OnChecked(PuzzleMarker source, PuzzleMarker destination, int numBlocks, bool isValid)
        {
            if (isValid || source.Stack == destination.Stack)
            {
                SetRuleReminder(null);
                return;
            }

            List<string> ruleExceptions;
            if (!_currentPuzzle.CanFit(source.Stack, source.Block, destination.Stack)){
                ruleExceptions = new List<string> { "Not enough room to move the selected blocks" };
            }
            else
            {
                ruleExceptions = _currentPuzzle.GetIsPlaceableRuleExceptions(source.Block, destination.Stack.TopBlock);
            }
            if (ruleExceptions.Count == 0)
            {
                SetRuleReminder(null);
                return;
            }
            SetRuleReminder(ruleExceptions[0]);
        }

        protected virtual void OnCancelled(PuzzleMarker selection)
        {
            SetRuleReminder("");
        }

        protected virtual void SetRuleReminder(string ruleDescription)
        {
            LeanTween.cancel(RuleReminderCanvasGroup.gameObject);
            LeanTween.alphaCanvas(RuleReminderCanvasGroup, 0.0f, 0.25f).setEase(LeanTweenType.easeOutSine);
            _ruleDescription = ruleDescription;
            _ruleDescriptionTime = Time.time;
            _ruleDescriptionPending = !string.IsNullOrEmpty(ruleDescription);
        }
    }
}
