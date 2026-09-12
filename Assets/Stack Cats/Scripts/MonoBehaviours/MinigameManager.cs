using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Tofuwu.StackCats.Data;

namespace Tofuwu.StackCats
{
    public class MinigameManager : MonoBehaviour
    {
        public MinigameInformationCollection Minigames;

        private IMinigameData _minigameData;

        public MinigameInformation CurrentMinigame { get; set; }
        public List<Cat> MinigameCats { get; set; }
        public DateTime LastMinigamePlayedTime { get { return _minigameData.LastMinigamePlayedTime; } set { _minigameData.LastMinigamePlayedTime = value; } }

        protected void Awake()
        {
            _minigameData = GameManager.Instance.Data.MinigameData;
        }

        public List<MinigameInformation> GetMinigamesForCats(List<Cat> cats)
        {
            int numCats = cats.Count;
            return Minigames.List.Where(m =>
                m.RequiredCats.All(c => cats.Contains(c)) &&
                numCats >= m.MinimumCats && numCats <= m.MaximumCats).ToList();
        }
    }
}