using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class PuzzleCompletionData : object
    {
        public string puzzleId;
        public float duration;
        public int numMovesMade;
        public DateTime completionDateTime;
    }

    [Serializable]
    public class LocalPuzzleData : LocalData<LocalPuzzleData>, IPuzzleData
    {
        /// <summary>
        /// The daily puzzles (if any).
        /// </summary>
        public IDailyPuzzleData DailyPuzzles { get { return _dailyPuzzles; } set { _dailyPuzzles = value; } }

        /// <summary>
        /// The ID of the currently active puzzle.
        /// </summary>
        public string CurrentAreaId { get { return _currentAreaId; } set { _currentAreaId = value; Save(); } }

        /// <summary>
        /// The selected play mode of the current area.
        /// </summary>
        public PuzzleMode CurrentMode { get { return _currentMode; } set { _currentMode = value; Save(); } }

        /// <summary>
        /// The current story puzzle of the current area.
        /// </summary>
        public string CurrentStoryPuzzleId { get { return _currentStoryPuzzleId; } set { _currentStoryPuzzleId = value; Save(); } }

        /// <summary>
        /// The default selected challenge run difficulty.
        /// </summary>
        public ChallengeRunDifficulty DefaultChallengeRunDifficulty { get { return _defaultChallengeRunDifficulty; } set { _defaultChallengeRunDifficulty = value; Save(); } }

        /// <summary>
        /// The player's current luck bonus.
        /// </summary>
        public float CurrentLuck { get { return _currentLuck; } set { _currentLuck = value; Save(); } }

        public int SpecialPuzzlesRemaining { get { return _luckModifiersRemaining; } set { _luckModifiersRemaining = value; Save(); } }

        private readonly string _dataPath;
        private IDailyPuzzleData _dailyPuzzles;
        private string _currentAreaId = "";
        private PuzzleMode _currentMode;
        private string _currentStoryPuzzleId;
        private Dictionary<string, IChallengeRunData> _currentChallengeRun = new Dictionary<string, IChallengeRunData>();
        private Dictionary<string, List<IChallengeRunData>> _completedChallengeRuns = new Dictionary<string, List<IChallengeRunData>>();
        private ChallengeRunDifficulty _defaultChallengeRunDifficulty;
        private Dictionary<string, IEndlessRunData> _currentEndlessRun = new Dictionary<string, IEndlessRunData>();
        private Dictionary<string, List<IEndlessRunData>> _completedEndlessRuns = new Dictionary<string, List<IEndlessRunData>>();
        private float _currentLuck;
        private int _luckModifiersRemaining;
        private readonly List<string> _unlockedAreas = new List<string>();
        private readonly List<string> _unlockedPuzzles = new List<string>();
        private readonly List<PuzzleCompletionData> _completedPuzzles = new List<PuzzleCompletionData>();

        public LocalPuzzleData(string dataPath) : base(dataPath) { }

        /// <summary>
        /// Check if an area is locked.
        /// </summary>
        /// <param name="areaId">The ID of the checked area.</param>
        /// <returns>A flag indicating whether or not the puzzle is locked.</returns>
        public bool IsAreaLocked(string areaId)
        {
            return !_unlockedAreas.Contains(areaId);
        }

        /// <summary>
        /// Unlock an area.
        /// </summary>
        /// <param name="areaId">The ID of the area to unlock.</param>
        public void UnlockArea(string areaId)
        {
            if (!_unlockedAreas.Contains(areaId))
            {
                _unlockedAreas.Add(areaId);

                Save();
            }
        }

        /// <summary>
        /// Check whether or not a puzzle is locked.
        /// </summary>
        /// <param name="puzzleId">Th ID of the checked puzzle.</param>
        public bool IsProgressionPuzzleLocked(string puzzleId)
        {
            return !_unlockedPuzzles.Contains(puzzleId);
        }

        /// <summary>
        /// Check whether or not a puzzle is completed.
        /// </summary>
        /// <param name="puzzleId"></param>
        /// <returns></returns>
        public bool IsProgressionPuzzleCompleted(string puzzleId)
        {
            return _completedPuzzles.FirstOrDefault(p => p.puzzleId == puzzleId) != null;
        }

        /// <summary>
        /// Get the best move score achieved on a puzzle.
        /// </summary>
        /// <param name="puzzleId">The ID of the checked puzzle.</param>
        /// <returns>The best move score achieved.</returns>
        public int? GetStoryPuzzleBestMoveScore(string puzzleId)
        {
            List<PuzzleCompletionData> puzzleCompletions = _completedPuzzles.Where(p => p.puzzleId == puzzleId).ToList();

            return puzzleCompletions.Count > 0 ? (int?)puzzleCompletions.Min(p => p.numMovesMade) : null;
        }

        /// <summary>
        /// Unlock the specified puzzle.
        /// </summary>
        /// <param name="puzzleId">The ID of the puzzle to unlock.</param>
        public void UnlockProgressionPuzzle(string puzzleId)
        {
            if (!_unlockedPuzzles.Contains(puzzleId))
            {
                _unlockedPuzzles.Add(puzzleId);

                Save();
            }
        }

        /// <summary>
        /// Complete a puzzle.
        /// </summary>
        /// <param name="puzzleId"></param>
        public void CompleteProgressionPuzzle(string puzzleId, DateTime completionDateTime, float duration, int numMovesMade)
        {
            PuzzleCompletionData data = new PuzzleCompletionData();
            data.puzzleId = puzzleId;
            data.completionDateTime = completionDateTime;
            data.numMovesMade = numMovesMade;
            data.duration = duration;
            _completedPuzzles.Add(data);

            Save();
        }

        public IChallengeRunData StartChallengeRun(string areaId, ChallengeRunDifficulty difficulty)
        {
            if (_currentChallengeRun.ContainsKey(areaId)) return null;

            IChallengeRunData newChallengeRun = new LocalChallengeRunData(difficulty);
            _currentChallengeRun.Add(areaId, newChallengeRun);

            return newChallengeRun;
        }

        public IChallengePuzzleData StartChallengePuzzle(string areaId, string puzzleJsonData, int difficultyRating, int minMoves, int maxMoves, int numUndos)
        {
            if (_currentChallengeRun[areaId].CurrentPuzzle == null)
            {
                _currentChallengeRun[areaId].StartNewPuzzle(puzzleJsonData, difficultyRating, minMoves, maxMoves, numUndos);
                Save();
            }

            return _currentChallengeRun[areaId].CurrentPuzzle;
        }

        public void UpdateChallengePuzzle(string areaId, string puzzleStateJsonData, int movesMade, List<string> catsSeen, List<string> newCatsSeen, int numUndosRemaining)
        {
            if (!_currentChallengeRun.ContainsKey(areaId)) return;

            _currentChallengeRun[areaId].UpdateCurrentPuzzle(puzzleStateJsonData, movesMade, catsSeen, newCatsSeen, numUndosRemaining);
            Save();
        }

        public bool UseLuckPotion(string areaId, string puzzleJsonData)
        {
            if (!_currentChallengeRun.ContainsKey(areaId)) return false;

            if (_currentChallengeRun[areaId].UseLuckPotion(puzzleJsonData))
            {
                Save();
                return true;
            }

            return false;
        }

        public void CompleteChallengePuzzle(string areaId, PuzzleCompletionType completionType, int movesMade)
        {
            IChallengeRunData currentChallengeRun = _currentChallengeRun[areaId];
            currentChallengeRun.CompleteCurrentPuzzle(completionType, movesMade);
            Save();
        }

        public void CompleteChallengeRun(string areaId, Dictionary<Currency, int> currencyReward, IPresentData presentReward)
        {
            IChallengeRunData currentChallengeRun = _currentChallengeRun[areaId];
            currentChallengeRun.CompleteChallengeRun(currencyReward, presentReward);
            Save();
        }

        public void EndChallengeRun(string areaId)
        {
            if (!_currentChallengeRun.ContainsKey(areaId)) return;

            if (!_completedChallengeRuns.ContainsKey(areaId)) _completedChallengeRuns.Add(areaId, new List<IChallengeRunData>());
            _completedChallengeRuns[areaId].Add(_currentChallengeRun[areaId]);
            _currentChallengeRun.Remove(areaId);
            Save();
        }

        public IChallengeRunData GetLastCompletedChallengeRun(string areaId)
        {
            return _currentChallengeRun.ContainsKey(areaId) ? _completedChallengeRuns[areaId][_completedChallengeRuns[areaId].Count - 1] : null;
        }

        public IChallengeRunData GetCurrentChallengeRun(string areaId)
        {
            return _currentChallengeRun.ContainsKey(areaId) ? _currentChallengeRun[areaId] : null;
        }

        public List<IChallengeRunData> GetCompletedChallengeRuns(string areaId)
        {
            return _completedChallengeRuns.ContainsKey(areaId) ? _completedChallengeRuns[areaId] : new List<IChallengeRunData>();
        }

        public bool IsChallengeRunDifficultyCompleted(string areaId, ChallengeRunDifficulty difficulty)
        {
            if (!_completedChallengeRuns.ContainsKey(areaId)) return false;

            return _completedChallengeRuns[areaId].FirstOrDefault(crd => crd.Difficulty == difficulty && crd.IsComplete && crd.CompletedPuzzles.Count == 10) != null;
        }

        public IEndlessRunData StartEndlessRun(string areaId)
        {
            if (_currentEndlessRun.ContainsKey(areaId)) return null;

            IEndlessRunData newEndlessRun = new LocalEndlessRunData();
            _currentEndlessRun.Add(areaId, newEndlessRun);
            Save();

            return newEndlessRun;
        }

        public IEndlessPuzzleData StartEndlessPuzzle(string areaId, string puzzleJsonData, int difficultyRating, int minMoves, int maxMoves, int numUndos)
        {
            if (_currentEndlessRun[areaId].CurrentPuzzle == null)
            {
                _currentEndlessRun[areaId].StartNewPuzzle(puzzleJsonData, difficultyRating, minMoves, maxMoves, numUndos);
                Save();
            }

            return _currentEndlessRun[areaId].CurrentPuzzle;
        }

        public void UpdateEndlessPuzzle(string areaId, string puzzleStateJsonData, int movesMade, List<string> catsSeen, List<string> newCatsSeen, int numUndosRemaining)
        {
            if (_currentEndlessRun == null) return;

            _currentEndlessRun[areaId].UpdateCurrentPuzzle(puzzleStateJsonData, movesMade, catsSeen, newCatsSeen, numUndosRemaining);
            Save();
        }

        public IEndlessRunData GetCurrentEndlessRun(string areaId)
        {
            return _currentEndlessRun.ContainsKey(areaId) ? _currentEndlessRun[areaId] : null;
        }

        public List<IEndlessRunData> GetCompletedEndlessRuns(string areaId)
        {
            return _completedEndlessRuns.ContainsKey(areaId) ? _completedEndlessRuns[areaId] : new List<IEndlessRunData>();
        }

        public void CompleteEndlessPuzzle(string areaId, PuzzleCompletionType completionType, int movesMade)
        {
            _currentEndlessRun[areaId].CompleteCurrentPuzzle(completionType, movesMade);
            Save();
        }

        public void EndEndlessRun(string areaId)
        {
            if (!_currentEndlessRun.ContainsKey(areaId)) return;

            if (!_completedEndlessRuns.ContainsKey(areaId)) _completedEndlessRuns.Add(areaId, new List<IEndlessRunData>());
            _completedEndlessRuns[areaId].Add(_currentEndlessRun[areaId]);
            _currentEndlessRun.Remove(areaId);
            Save();
        }
    }
}