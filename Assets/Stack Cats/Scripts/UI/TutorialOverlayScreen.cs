using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public abstract class TutorialOverlayScreen : OverlayScreen
    {
        private TutorialManager _tutorialManager;
        private Tutorial _tutorial;
        private bool _isInitialized;

        public virtual void Initialize(Tutorial tutorial)
        {
            _tutorial = tutorial;
            _isInitialized = true;
        }

        public override void OnTransitioningIn()
        {
            base.OnTransitioningIn();

            if(!_isInitialized)
            {
                Debug.LogError("Data was not intialized. Exiting overlay.");
                Dismiss();
            }
        }

        public override void OnTransitioningOut()
        {
            base.OnTransitioningOut();

            CompleteTutorial();
            _isInitialized = false;
        }

        protected void Awake()
        {
            _tutorialManager = GameManager.Instance.TutorialManager;
        }

        protected override void Update()
        {
            base.Update();

            if (_isActive)
            {
                if (Input.GetMouseButtonDown(0)) Dismiss();
            }
        }

        private void CompleteTutorial()
        {
            _tutorialManager.CompleteTutorial(_tutorial.TypeName);
        }
    }
}