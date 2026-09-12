using System;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalMinigameData : LocalData<LocalMinigameData>, IMinigameData
    {
        public DateTime LastMinigamePlayedTime { get { return _lastMinigamePlayedTime; } set { _lastMinigamePlayedTime = value; Save(); } }

        private DateTime _lastMinigamePlayedTime;

        public LocalMinigameData(string dataPath) : base(dataPath) { }
    }
}
