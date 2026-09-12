using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Tofuwu.StackCats
{
    /// <summary>
    /// Stores events that should only be handled once.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OneOffEvent<T>
    {
        private readonly List<T> _events = new List<T>();

        public void AddEvent(T val)
        {
            _events.Add(val);
        }

        public void ConsumeFirst(UnityAction<T> action)
        {
            if (_events.Count > 0)
            {
                action.Invoke(_events[0]);
                _events.RemoveAt(0);
            }
        }

        public void ConsumeAll(UnityAction<T> action)
        {
            while(_events.Count > 0)
            {
                ConsumeFirst(action);
            }
        }

        public void ConsumeValue(T eventValue, UnityAction<T> action)
        {
            for(int i = _events.Count - 1; i >= 0; i--)
            {
                if (_events[i].Equals(eventValue))
                {
                    action.Invoke(eventValue);
                    _events.RemoveAt(i);
                }
            }
        }

        public void ClearAll()
        {
            _events.Clear();
        }
    }
}
