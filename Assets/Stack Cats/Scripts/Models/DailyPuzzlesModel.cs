using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats.Models
{
    [Serializable]
    public class DailyPuzzlesModel
    {
        public DateTime ForDate { get { return _forDate; } }
        public List<PuzzleModel> Puzzles { get { return _puzzles; } }

        public DailyPuzzlesModel(DateTime forDate)
        {
            _forDate = forDate;
        }

        [SerializeField]
        private DateTime _forDate;

        [SerializeField]
        private List<PuzzleModel> _puzzles = new List<PuzzleModel>();
    }
}