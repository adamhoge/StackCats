using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats.UI
{
    public abstract class PointerGestureActionUI
    {
        public float Duration;

        protected PointerGestureUI _pointerGesture;

        public abstract void Invoke(PointerGestureUI pointerGesture);
    }

    public class PointerGestureSetPressedAction : PointerGestureActionUI
    {
        public bool IsPressed;
        public bool IsAnimated;

        public override void Invoke(PointerGestureUI pointerGesture)
        {
            pointerGesture.SetPressed(IsPressed, IsAnimated);
        }
    }

    public class PointerGestureSetVisibleAction : PointerGestureActionUI
    {
        public bool IsVisible;
        public bool IsAnimated;

        public override void Invoke(PointerGestureUI pointerGesture)
        {
            pointerGesture.SetVisible(IsVisible, IsAnimated);
        }
    }

    public class PointerGestureSetPositionAction : PointerGestureActionUI
    {
        public Vector3 Position;
        public bool IsAnimated;

        public override void Invoke(PointerGestureUI pointerGesture)
        {
            pointerGesture.SetPosition(Position, IsAnimated);
        }
    }

    [RequireComponent(typeof(PointerGestureUI))]
    public class PointerGestureSequenceUI : MonoBehaviour
    {
        public List<PointerGestureActionUI> ActionSequenceItems = new List<PointerGestureActionUI>();
        public bool Loop;

        private PointerGestureUI _pointerGesture;
        private float _currentActionExecutionTime;
        private PointerGestureActionUI _currentAction;

        public void PlaySequence()
        {
            _currentAction = null;
            GoToNextAction();
        }

        protected void Awake()
        {
            _pointerGesture = GetComponent<PointerGestureUI>();
        }

        protected void Update()
        {
            if (_currentAction != null && Time.time >= _currentActionExecutionTime + _currentAction.Duration)
            {
                GoToNextAction();
            }
        }

        private void GoToNextAction()
        {
            if (_currentAction == null)
            {
                if (ActionSequenceItems.Count == 0) return;

                _currentAction = ActionSequenceItems[0];
            }
            else
            {
                int nextActionIndex = ActionSequenceItems.IndexOf(_currentAction) + 1;
                if (Loop) nextActionIndex = nextActionIndex % ActionSequenceItems.Count;

                if (nextActionIndex >= 0 && nextActionIndex < ActionSequenceItems.Count)
                {
                    _currentAction = ActionSequenceItems[nextActionIndex];
                }
                else
                {
                    _currentAction = null;
                    return;
                }
            }

            _currentAction.Invoke(_pointerGesture);
            _currentActionExecutionTime = Time.time;
        }
    }
}