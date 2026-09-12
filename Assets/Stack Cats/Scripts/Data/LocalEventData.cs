using System;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalEventData : LocalData<LocalEventData>, IEventData
    {
        public DateTime LastAdDateTime { get { return _lastAdDateTime; } set { _lastAdDateTime = value; Save(); } }

        private DateTime _lastAdDateTime;

        public LocalEventData(string dataPath) : base(dataPath)
        {
            _lastAdDateTime = DateTime.MinValue;
        }
    }
}