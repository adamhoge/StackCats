using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Tofuwu.StackCats.Data;
using Tofuwu.StackCats.Models;
using Tofuwu.StackCats.Procedural;
using static Tofuwu.StackCats.Procedural.CatBlockContentsGenerator;

namespace Tofuwu.StackCats
{
    public delegate void StoryPuzzleFirstCompletion(StoryPuzzle storyPuzzle);
    public delegate void PuzzleAreaUnlocked(PuzzleArea puzzleArea);
    public delegate void ChallengeModePuzzleAreaUnlocked(PuzzleArea puzzleArea);
    public delegate void EndlessModePuzzleAreaUnlocked(PuzzleArea puzzleArea);

    public class PuzzleManager : MonoBehaviour
    {
        public class StoryPuzzleCompletionEvent
        {
            public StoryPuzzle Puzzle;
            public int PreviousNumStarsEarned;
            public int CurrentNumStarsEarned;
            public bool WasPreviouslyCompleted;
        }

        public class PuzzleAreaUnlockedEvent
        {
            public PuzzleArea PuzzleArea;
        }

        public class ChallengePuzzleCompletionEvent
        {
            public PuzzleCompletionType CompletionType;
        }

        public class EndlessPuzzleCompletionEvent
        {
            public PuzzleCompletionType CompletionType;
            public int PointsEarned;
            public int TotalPuzzlesCompleted;
        }

        public event StoryPuzzleFirstCompletion onStoryPuzzleFirstCompletion;
        public event PuzzleAreaUnlocked onPuzzleAreaUnlocked;
        public event ChallengeModePuzzleAreaUnlocked onChallengeModePuzzleAreaUnlocked;
        public event EndlessModePuzzleAreaUnlocked onEndlessModePuzzleAreaUnlocked;

        public PuzzleAreaCollection PuzzleAreaCollection;
        public CurrencyAmountDictionary DefaultCurrencyReward;
        public int StartingNumUndos = 10;
        public string DailyPuzzlesURL;
        public float CheckForDailyPuzzlesInterval = 30.0f;

        /// <summary>
        /// The puzzle area that is currently used to generate puzzles.
        /// </summary>
        public PuzzleArea CurrentArea { get { return _currentArea; } set { SetCurrentArea(value); } }

        /// <summary>
        /// The game mode of the current area.
        /// </summary>
        public PuzzleMode CurrentMode { get { return _currentMode; } set { SetCurrentAreaMode(value); } }

        /// <summary>
        /// The currently selected progression puzzle.
        /// </summary>
        public StoryPuzzle CurrentStoryPuzzle { get { return _currentStoryPuzzle; } set { SetCurrentStoryPuzzle(value); } }

        /// <summary>
        /// Flag indicating whether or not all story puzzles have been completed.
        /// </summary>
        public bool HasCompletedAllStoryPuzzles { get { return GetNumCompletedStoryPuzzles() == GetNumStoryPuzzlesInAllAreas(); } }

        /// <summary>
        /// The selected difficulty of a challenge run.
        /// </summary>
        public ChallengeRunDifficulty DefaultChallengeRunDifficulty { get { return _data.DefaultChallengeRunDifficulty; } set { _data.DefaultChallengeRunDifficulty = value; } }

        /// <summary>
        /// The maximum number of challenge puzzles played in a run.
        /// </summary>
        public int MaxChallengeRunPuzzles { get { return MAX_CHALLENGE_RUN_PUZZLES; } }

        /// <summary>
        /// The player's current luck bonus.
        /// </summary>
        public float CurrentLuck { get { return _data.CurrentLuck; } set { _data.CurrentLuck = value; } }

        /// <summary>
        /// The number of automatic special puzzles remaining.
        /// </summary>
        public int SpecialPuzzlesRemaining { get { return _data.SpecialPuzzlesRemaining; } set { _data.SpecialPuzzlesRemaining = value; } }

        /// <summary>
        /// Indicates whether or not there are any daily puzzles to play.
        /// </summary>
        public bool HasDailyPuzzles { get { return _data.DailyPuzzles != null; } }

        /// <summary>
        /// One off event for unlocked puzzles.
        /// </summary>
        public OneOffEvent<StoryPuzzle> OneOffProgressionPuzzleUnlocked { get { return _oneOffProgressionPuzzleUnlocked; } }

        /// <summary>
        /// One off event for completed challenge puzzles.
        /// </summary>
        public OneOffEvent<ChallengePuzzleCompletionEvent> OneOffChallengePuzzleCompleted { get { return _oneOffChallengePuzzleCompleted; } }

        /// <summary>
        /// One off event for completed challenge puzzles.
        /// </summary>
        public OneOffEvent<EndlessPuzzleCompletionEvent> OneOffEndlessPuzzleCompleted { get { return _oneOffEndlessPuzzleCompleted; } }

        /// <summary>
        /// One off event for completed story puzzles.
        /// </summary>
        public OneOffEvent<StoryPuzzleCompletionEvent> OneOffStoryPuzzleCompleted { get { return _oneOffStoryPuzzleCompleted; } }

        /// <summary>
        /// One off event for unlocked areas.
        /// </summary>
        public OneOffEvent<PuzzleAreaUnlockedEvent> OneOffPuzzleAreaUnlocked { get { return _oneOffPuzzleAreaUnlocked; } }

        /// <summary>
        /// One off event for completed daily puzzles.
        /// </summary>
        public OneOffEvent<int> OneOffDailyPuzzleCompleted;

        private const int MAX_CHALLENGE_RUN_PUZZLES = 10;

        private GameManager _gameManager;
        private CatManager _catManager;
        private CurrencyManager _currencyManager;
        private StuffManager _stuffManager;
        private IPuzzleData _data;
        private PuzzleArea _currentArea;
        private PuzzleMode _currentMode;
        private StoryPuzzle _currentStoryPuzzle;
        private StoryPuzzle _currentDailyPuzzle;
        private readonly OneOffEvent<StoryPuzzle> _oneOffProgressionPuzzleUnlocked = new OneOffEvent<StoryPuzzle>();
        private readonly OneOffEvent<StoryPuzzleCompletionEvent> _oneOffStoryPuzzleCompleted = new OneOffEvent<StoryPuzzleCompletionEvent>();
        private readonly OneOffEvent<ChallengePuzzleCompletionEvent> _oneOffChallengePuzzleCompleted = new OneOffEvent<ChallengePuzzleCompletionEvent>();
        private readonly OneOffEvent<EndlessPuzzleCompletionEvent> _oneOffEndlessPuzzleCompleted = new OneOffEvent<EndlessPuzzleCompletionEvent>();
        private readonly OneOffEvent<PuzzleAreaUnlockedEvent> _oneOffPuzzleAreaUnlocked = new OneOffEvent<PuzzleAreaUnlockedEvent>();
        private float _newDailyPuzzlesTime;
        private float _lastTryGetDailyPuzzlesAt;

        /// <summary>
        /// Check whether or not an area is locked.
        /// </summary>
        /// <param name="puzzleArea">The area to check.</param>
        /// <returns>A flag indicating whether or not the area is locked.</returns>
        public bool IsAreaLocked(PuzzleArea puzzleArea)
        {
            if (!puzzleArea) return false;

            return _data.IsAreaLocked(puzzleArea.GetId());
        }

        /// <summary>
        /// Unlock a puzzle area.
        /// </summary>
        /// <param name="puzzleArea">The puzzle area to unlock.</param>
        public void UnlockArea(PuzzleArea puzzleArea)
        {
            if (!puzzleArea) return;

            if (IsAreaLocked(puzzleArea))
            {
                _data.UnlockArea(puzzleArea.GetId());
                List<StoryPuzzle> storyPuzzles = GetStoryPuzzles(puzzleArea);
                if (storyPuzzles.Count > 0) UnlockStoryPuzzle(storyPuzzles[0]);
                if (onPuzzleAreaUnlocked != null) onPuzzleAreaUnlocked(puzzleArea);
                _oneOffPuzzleAreaUnlocked.AddEvent(new PuzzleAreaUnlockedEvent { PuzzleArea = puzzleArea });
            }

            if (!_currentArea) SetCurrentArea(puzzleArea);
        }

        /// <summary>
        /// Check whether or not a story puzzle area was completed.
        /// </summary>
        /// <param name="puzzleArea">The puzzle area.</param>
        public bool IsStoryPuzzleAreaCompleted(PuzzleArea puzzleArea)
        {
            List<ProgressionPuzzleRouteItem> route = puzzleArea.PuzzleAreaMap.ProgressionPuzzleRoute;

            if (puzzleArea.PuzzleAreaMap.RouteIndexToNextArea == -1) return true;

            return route.Count == 0 || _data.IsProgressionPuzzleCompleted(route[puzzleArea.PuzzleAreaMap.RouteIndexToNextArea].NormalPuzzle.GetId());
        }

        /// <summary>
        /// Get a list of story puzzles in a puzzle area.
        /// </summary>
        /// <param name="puzzleArea">The puzzle area.</param>
        /// <returns></returns>
        public List<StoryPuzzle> GetStoryPuzzles(PuzzleArea puzzleArea)
        {
            return puzzleArea.PuzzleAreaMap.ProgressionPuzzleRoute.Select(r => r.NormalPuzzle).ToList();
        }

        /// <summary>
        /// Check whether or not a puzzle is locked.
        /// </summary>
        /// <param name="puzzle">The puzzle to check.</param>
        /// <returns>A flag indicating whether or not a puzle is locked.</returns>
        public bool IsStoryPuzzleLocked(StoryPuzzle puzzle)
        {
            if (!puzzle) return true;

            return _data.IsProgressionPuzzleLocked(puzzle.GetId());
        }

        /// <summary>
        /// Check whether or not a puzzle is completed.
        /// </summary>
        /// <param name="puzzle">The puzzle to check.</param>
        /// <returns>A flag indicating whether or not the puzzle is completed.</returns>
        public bool IsStoryPuzzleCompleted(StoryPuzzle puzzle)
        {
            if (!puzzle) return false;

            return _data.IsProgressionPuzzleCompleted(puzzle.GetId());
        }

        /// <summary>
        /// Get the best move score for a puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle checked.</param>
        /// <returns>The best move score of the puzzle.</returns>
        public int? GetStoryPuzzleBestMoveScore(StoryPuzzle puzzle)
        {
            if (!puzzle) return null;

            return _data.GetStoryPuzzleBestMoveScore(puzzle.GetId());
        }

        /// <summary>
        /// Get the number of stars earned for a progression puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle checked.</param>
        /// <returns>The number of stars earned.</returns>
        public int GetStoryPuzzleNumStarsEarned(StoryPuzzle puzzle)
        {
            if (!puzzle) return 0;

            int? bestMoveScore = GetStoryPuzzleBestMoveScore(puzzle);
            if (bestMoveScore == null) return 0;

            int starsEarned = 0;
            foreach (int starRequirement in puzzle.StarMoveRequirements)
            {
                if (bestMoveScore <= starRequirement) ++starsEarned;
            }

            return starsEarned;
        }

        public int GetNumStoryPuzzlesInAllAreas()
        {
            return PuzzleAreaCollection.List.Sum(pa => pa.PuzzleAreaMap.ProgressionPuzzleRoute.Count);
        }

        /// <summary>
        /// Get the number of completed puzzles in a given area.
        /// </summary>
        /// <param name="puzzleArea">The puzzle area to check.</param>
        /// <returns></returns>
        public int GetNumCompletedStoryPuzzles()
        {
            return PuzzleAreaCollection.List.Sum(pa => GetNumCompletedStoryPuzzles(pa));
        }

        /// <summary>
        /// Get the number of completed puzzles in a given area.
        /// </summary>
        /// <param name="puzzleArea">The puzzle area to check.</param>
        /// <returns></returns>
        public int GetNumCompletedStoryPuzzles(PuzzleArea puzzleArea)
        {
            return GetStoryPuzzles(puzzleArea).Count(IsStoryPuzzleCompleted);
        }

        /// <summary>
        /// Get the number of stars earned for all puzzle areas.
        /// </summary>
        /// <returns>The total number of stars earned.</returns>
        public int GetTotalNumStarsEarned()
        {
            return PuzzleAreaCollection.List.Sum(pa => GetTotalNumStarsEarned(pa));
        }

        /// <summary>
        /// Get the total number of stars earned in a given puzzle area.
        /// </summary>
        /// <param name="puzzleArea">The puzzle area to check.</param>
        /// <returns>The total number of stars earned.</returns>
        public int GetTotalNumStarsEarned(PuzzleArea puzzleArea)
        {
            return GetStoryPuzzles(puzzleArea).Sum(pp => GetStoryPuzzleNumStarsEarned(pp));
        }

        /// <summary>
        /// Get the total number of stars in all areas.
        /// </summary>
        public int GetNumStarsInAllAreas()
        {
            return PuzzleAreaCollection.List.Sum(pa => GetStoryPuzzles(pa).Sum(p => p ? p.StarMoveRequirements.Count : 0));
        }

        /// <summary>
        /// Get the total number of stars in a given area.
        /// </summary>
        /// <param name="puzzleArea"></param>
        /// <returns></returns>
        public int GetNumStarsInArea(PuzzleArea puzzleArea)
        {
            return GetStoryPuzzles(puzzleArea).Sum(p => p ? p.StarMoveRequirements.Count : 0);
        }

        /// <summary>
        /// Unlock a puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle to unlock.</param>
        public void UnlockStoryPuzzle(StoryPuzzle puzzle)
        {
            if (!puzzle) return;

            string puzzleId = puzzle.GetId();
            if (_data.IsProgressionPuzzleLocked(puzzleId))
            {
                _oneOffProgressionPuzzleUnlocked.AddEvent(puzzle);
                _data.UnlockProgressionPuzzle(puzzleId);
            }
        }

        /// <summary>
        /// Complete a progression puzzle.
        /// </summary>
        /// <param name="puzzle">The progression puzzle to complete.</param>
        /// <param name="duration">The puzzled time elapsed.</param>
        public void CompleteStoryPuzzle(StoryPuzzle puzzle, float duration, int numMovesMade)
        {
            if (!puzzle) return;

            bool wasPrevCompleted = IsStoryPuzzleCompleted(puzzle);
            int prevNumStarsEarned = GetStoryPuzzleNumStarsEarned(puzzle);

            _data.CompleteProgressionPuzzle(puzzle.GetId(), System.DateTime.Now, duration, numMovesMade);
            PuzzleAreaMap currentMap = CurrentArea.PuzzleAreaMap;
            int currentPuzzleIndex = currentMap.ProgressionPuzzleRoute.FindIndex(ri => ri.NormalPuzzle == puzzle);
            if (currentPuzzleIndex == currentMap.RouteIndexToNextArea)
            {
                PuzzleArea nextPuzzleArea = GetNextPuzzleArea(_currentArea);
                if (nextPuzzleArea && IsAreaLocked(nextPuzzleArea)) UnlockArea(nextPuzzleArea);
            }

            if (!wasPrevCompleted)
            {
                if (currentPuzzleIndex == currentMap.RouteIndexToChallengeMode)
                {
                    if (onChallengeModePuzzleAreaUnlocked != null) onChallengeModePuzzleAreaUnlocked(currentMap.PuzzleArea);
                }

                if (currentPuzzleIndex == currentMap.RouteIndexToEndlessMode)
                {
                    if (onEndlessModePuzzleAreaUnlocked != null) onEndlessModePuzzleAreaUnlocked(currentMap.PuzzleArea);
                }
            }

            int currentNumStarsEarned = GetStoryPuzzleNumStarsEarned(puzzle);

            StoryPuzzleCompletionEvent completionEvent = new StoryPuzzleCompletionEvent
            {
                Puzzle = puzzle,
                WasPreviouslyCompleted = wasPrevCompleted,
                PreviousNumStarsEarned = prevNumStarsEarned,
                CurrentNumStarsEarned = currentNumStarsEarned
            };
            _oneOffStoryPuzzleCompleted.AddEvent(completionEvent);

            if (!wasPrevCompleted && onStoryPuzzleFirstCompletion != null) onStoryPuzzleFirstCompletion(puzzle);
        }

        /// <summary>
        /// Get the progression puzzle immediately following the provided puzzle.
        /// </summary>
        /// <param name="progressionPuzzle">The puzzle after which to check.</param>
        /// <returns>The next progression puzzle.</returns>
        public StoryPuzzle GetNextStoryPuzzle(StoryPuzzle progressionPuzzle)
        {
            List<ProgressionPuzzleRouteItem> routeItems = CurrentArea.PuzzleAreaMap.ProgressionPuzzleRoute;
            int puzzleIndex = routeItems.FindIndex(ri => ri.NormalPuzzle == progressionPuzzle);
            int nextPuzzleIndex = puzzleIndex == -1 || puzzleIndex == routeItems.Count - 1 ? -1 : puzzleIndex + 1;

            return nextPuzzleIndex == -1 ? null : routeItems[nextPuzzleIndex].NormalPuzzle;
        }

        /// <summary>
        /// Get the puzzle area immediately following the provided puzzle area.
        /// </summary>
        /// <param name="puzzleArea">The puzzle area after which to check.</param>
        /// <returns>The next puzzle area.</returns>
        public PuzzleArea GetNextPuzzleArea(PuzzleArea puzzleArea)
        {
            List<PuzzleArea> puzzleAreas = PuzzleAreaCollection.List;
            int puzzleAreaIndex = puzzleAreas.IndexOf(puzzleArea);
            int nextPuzzleAreaIndex = puzzleAreaIndex == -1 || puzzleAreaIndex == puzzleAreas.Count - 1 ? -1 : puzzleAreaIndex + 1;

            return nextPuzzleAreaIndex == -1 ? null : puzzleAreas[nextPuzzleAreaIndex];
        }

        /// <summary>
        /// Check whether or not challenge mode is locked.
        /// </summary>
        /// <returns>A flag indicating whether or not challenge mode is locked</returns>
        public bool IsChallengeModeLocked(PuzzleArea puzzleArea = null)
        {
            if (puzzleArea == null) puzzleArea = PuzzleAreaCollection.List[0];

            if (IsAreaLocked(puzzleArea)) return true;

            var puzzleAreaMap = puzzleArea.PuzzleAreaMap;
            var challengeUnlockedIndex = puzzleAreaMap.RouteIndexToChallengeMode;

            if (puzzleAreaMap.ProgressionPuzzleRoute.Count == 0) return false;

            return !IsStoryPuzzleCompleted(puzzleAreaMap.ProgressionPuzzleRoute[challengeUnlockedIndex].NormalPuzzle);
        }

        /// <summary>
        /// Start a challenge run.
        /// </summary>
        /// <param name="puzzleArea"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public ChallengeRunModel StartChallengeRun(PuzzleArea puzzleArea, ChallengeRunDifficulty difficulty)
        {
            ChallengeRunModel currentChallengeRun = GetCurrentChallengeRun(puzzleArea);
            if (currentChallengeRun != null) return currentChallengeRun;

            _data.StartChallengeRun(puzzleArea.GetId(), difficulty);

            GenerateNextChallengePuzzle(puzzleArea);

            return GetCurrentChallengeRun(puzzleArea);
        }

        /// <summary>
        /// Check whether or not endless mode is locked.
        /// </summary>
        /// <returns>A flag indicating whether or not challenge mode is locked</returns>
        public bool IsEndlessModeLocked(PuzzleArea puzzleArea = null)
        {
            if (puzzleArea == null) puzzleArea = PuzzleAreaCollection.List[0];

            if (IsAreaLocked(puzzleArea)) return true;

            var puzzleAreaMap = puzzleArea.PuzzleAreaMap;
            var endlessUnlockedIndex = puzzleAreaMap.RouteIndexToEndlessMode;

            if (puzzleAreaMap.ProgressionPuzzleRoute.Count <= endlessUnlockedIndex) return false;

            return !IsStoryPuzzleCompleted(puzzleAreaMap.ProgressionPuzzleRoute[endlessUnlockedIndex].NormalPuzzle);
        }

        /// <summary>
        /// Start an endless run.
        /// </summary>
        /// <returns></returns>
        public EndlessRunModel StartEndlessRun(PuzzleArea puzzleArea)
        {
            EndlessRunModel currentEndlessRun = GetCurrentEndlessRun(puzzleArea);
            if (currentEndlessRun != null) return currentEndlessRun;

            _data.StartEndlessRun(puzzleArea.GetId());

            GenerateNextEndlessPuzzle(puzzleArea);

            return GetCurrentEndlessRun(puzzleArea);
        }

        /// <summary>
        /// Update the state of the current challenge puzzle.
        /// </summary>
        /// <param name="puzzleArea"></param>
        /// <param name="puzzleStateJsonData"></param>
        /// <param name="movesMade"></param>
        public void UpdateCurrentChallengePuzzle(PuzzleArea puzzleArea, string puzzleStateJsonData, int movesMade, List<Cat> catsSeen, List<Cat> newCatsSeen, int numUndosRemaining)
        {
            List<string> catsSeenIds = catsSeen.Select(c => c.GetId()).ToList();
            List<string> newCatsSeenIds = newCatsSeen.Select(c => c.GetId()).ToList();
            _data.UpdateChallengePuzzle(puzzleArea.GetId(), puzzleStateJsonData, movesMade, catsSeenIds, newCatsSeenIds, numUndosRemaining);
        }

        /// <summary>
        /// Update the state of the current endless puzzle.
        /// </summary>
        /// <param name="puzzleStateJsonData"></param>
        /// <param name="movesMade"></param>
        public void UpdateCurrentEndlessPuzzle(PuzzleArea puzzleArea, string puzzleStateJsonData, int movesMade, List<Cat> catsSeen, List<Cat> newCatsSeen, int numUndosRemaining)
        {
            List<string> catsSeenIds = catsSeen.Select(c => c.GetId()).ToList();
            List<string> newCatsSeenIds = newCatsSeen.Select(c => c.GetId()).ToList();
            _data.UpdateEndlessPuzzle(puzzleArea.GetId(), puzzleStateJsonData, movesMade, catsSeenIds, newCatsSeenIds, numUndosRemaining);
        }

        /// <summary>
        /// Use a luck potion on the current puzzle.
        /// </summary>
        /// <param name="puzzleArea">The puzzle area of the target challenge run.</param>
        /// <param name="puzzleJsonData">The updated puzzle data.</param>
        /// <returns></returns>
        public bool UseLuckPotion(PuzzleArea puzzleArea, string puzzleJsonData)
        {
            if (_data.UseLuckPotion(puzzleArea.GetId(), puzzleJsonData))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Complete the current challenge puzzle.
        /// </summary>
        public void CompleteCurrentChallengePuzzle(PuzzleArea puzzleArea, PuzzleCompletionType completionType, int movesMade)
        {
            _data.CompleteChallengePuzzle(puzzleArea.GetId(), completionType, movesMade);

            IChallengeRunData currentChallengeRun = _data.GetCurrentChallengeRun(puzzleArea.GetId());
            if (completionType == PuzzleCompletionType.PuzzleFailed || currentChallengeRun.CompletedPuzzles.Count == 10)
            {
                _data.CompleteChallengeRun(puzzleArea.GetId(), new Dictionary<Currency, int>(), null);
            }
            else
            {
                GenerateNextChallengePuzzle(puzzleArea);
            }

            ChallengePuzzleCompletionEvent completionEvent = new ChallengePuzzleCompletionEvent
            {
                CompletionType = completionType
            };
            _oneOffChallengePuzzleCompleted.AddEvent(completionEvent);
        }


        /// <summary>
        /// Complete the current endless puzzle.
        /// </summary>
        public void CompleteCurrentEndlessPuzzle(PuzzleArea puzzleArea, PuzzleCompletionType completionType, int movesMade)
        {
            _data.CompleteEndlessPuzzle(puzzleArea.GetId(), completionType, movesMade);

            IEndlessRunData currentEndlessRun = _data.GetCurrentEndlessRun(puzzleArea.GetId());
            int pointsEarned = currentEndlessRun.GetCompletedPuzzleScore(currentEndlessRun.CompletedPuzzles.Last());
            int numPuzzlesCompleted = currentEndlessRun.CompletedPuzzles.Count;

            EndlessPuzzleCompletionEvent completionEvent = new EndlessPuzzleCompletionEvent
            {
                CompletionType = completionType,
                PointsEarned = pointsEarned,
                TotalPuzzlesCompleted = numPuzzlesCompleted
            };
            _oneOffEndlessPuzzleCompleted.AddEvent(completionEvent);

            if (!currentEndlessRun.IsComplete) GenerateNextEndlessPuzzle(puzzleArea);
        }

        /// <summary>
        /// End the current challenge run.
        /// </summary>
        /// <param name="puzzleArea"></param>
        public void EndChallengeRun(PuzzleArea puzzleArea)
        {
            IChallengeRunData currentChallengeRun = _data.GetCurrentChallengeRun(puzzleArea.GetId());
            List<ChallengePuzzleReward> rewards = puzzleArea.ChallengeRunRewards.Where(r => r.AtDifficulty == currentChallengeRun.Difficulty).ToList();
            List<IChallengePuzzleCompletionData> completedPuzzles = currentChallengeRun.CompletedPuzzles;
            int numCompletedPuzzles = completedPuzzles.Count;

            // Add trophy reward.
            if (numCompletedPuzzles == MAX_CHALLENGE_RUN_PUZZLES)
            {
                _stuffManager.AddItem(puzzleArea.ChallengeRunTrophies[currentChallengeRun.Difficulty]);
            }

            // Add currency rewards.
            int goldPaws = 0;
            int farmGems = 0;
            int jungleGems = 0;
            int cityGems = 0;
            int desertGems = 0;
            int galaxyGems = 0;
            for (int i = 0; i < numCompletedPuzzles; i++)
            {
                ChallengePuzzleReward rewardAtIndex = rewards.FirstOrDefault(r => r.AtPuzzleIndex == i);
                CurrencyAmountDictionary currencyRewards = rewardAtIndex != null ? rewardAtIndex.CurrencyReward : DefaultCurrencyReward;
                if (currencyRewards.ContainsKey(Currency.GoldPaw)) goldPaws += currencyRewards[Currency.GoldPaw];
                if (currencyRewards.ContainsKey(Currency.FarmGem)) farmGems += currencyRewards[Currency.FarmGem];
                if (currencyRewards.ContainsKey(Currency.JungleGem)) jungleGems += currencyRewards[Currency.JungleGem];
                if (currencyRewards.ContainsKey(Currency.CityGem)) cityGems += currencyRewards[Currency.CityGem];
                if (currencyRewards.ContainsKey(Currency.DesertGem)) desertGems += currencyRewards[Currency.DesertGem];
                if (currencyRewards.ContainsKey(Currency.GalaxyGem)) galaxyGems += currencyRewards[Currency.GalaxyGem];
            }

            if (goldPaws > 0)
            {
                _currencyManager.ChangeCurrency(Currency.GoldPaw, goldPaws);
            }
            if (farmGems > 0)
            {
                _currencyManager.ChangeCurrency(Currency.FarmGem, farmGems);
            }
            if (jungleGems > 0)
            {
                _currencyManager.ChangeCurrency(Currency.JungleGem, jungleGems);
            }
            if (cityGems > 0)
            {
                _currencyManager.ChangeCurrency(Currency.CityGem, cityGems);
            }
            if (desertGems > 0)
            {
                _currencyManager.ChangeCurrency(Currency.DesertGem, desertGems);
            }
            if (galaxyGems > 0)
            {
                _currencyManager.ChangeCurrency(Currency.GalaxyGem, galaxyGems);
            }


            _data.EndChallengeRun(puzzleArea.GetId());
        }

        /// <summary>
        /// End the current endless run.
        /// </summary>
        public void EndEndlessRun(PuzzleArea puzzleArea)
        {
            _data.EndEndlessRun(puzzleArea.GetId());
        }


        /// <summary>
        /// Get the current challenge run.
        /// </summary>
        /// <param name="puzzleArea"></param>
        /// <returns></returns>
        public ChallengeRunModel GetCurrentChallengeRun(PuzzleArea puzzleArea)
        {
            IChallengeRunData currentChallengeRunData = _data.GetCurrentChallengeRun(puzzleArea.GetId());

            if (currentChallengeRunData == null) return null;

            ChallengePuzzleModel currentPuzzleModel = DataModelMapper.MapToChallengePuzzleModel(currentChallengeRunData.CurrentPuzzle);
            if (currentPuzzleModel != null)
            {
                currentPuzzleModel.CatsSeen = currentChallengeRunData.CurrentPuzzle.CatsSeen.Select(c => _catManager.CatCollection.GetById(c)).ToList();
                currentPuzzleModel.NewCatsSeen = currentChallengeRunData.CurrentPuzzle.NewCatsSeen.Select(c => _catManager.CatCollection.GetById(c)).ToList();
            }

            List<ChallengePuzzleCompletionModel> completedPuzzles = new List<ChallengePuzzleCompletionModel>();
            foreach (IChallengePuzzleCompletionData puzzleCompletionData in currentChallengeRunData.CompletedPuzzles)
            {
                completedPuzzles.Add(DataModelMapper.MapToChallengePuzzleCompletionModel(puzzleCompletionData));
            }

            ChallengeRunModel currentChallengeRun = new ChallengeRunModel
            {
                Difficulty = currentChallengeRunData.Difficulty,
                CurrentPuzzle = currentPuzzleModel,
                CompletedPuzzles = completedPuzzles
            };

            return currentChallengeRun;
        }

        /// <summary>
        /// Get the current endless run.
        /// </summary>
        /// <param name="puzzleArea"></param>
        /// <returns></returns>
        public EndlessRunModel GetCurrentEndlessRun(PuzzleArea puzzleArea)
        {
            IEndlessRunData currentEndlessRunData = _data.GetCurrentEndlessRun(puzzleArea.GetId());

            if (currentEndlessRunData == null) return null;

            EndlessPuzzleModel currentPuzzleModel = DataModelMapper.MapToEndlessPuzzleModel(currentEndlessRunData.CurrentPuzzle, this);
            if (currentPuzzleModel != null)
            {
                currentPuzzleModel.CatsSeen = currentEndlessRunData.CurrentPuzzle.CatsSeen.Select(c => _catManager.CatCollection.GetById(c)).ToList();
                currentPuzzleModel.NewCatsSeen = currentEndlessRunData.CurrentPuzzle.NewCatsSeen.Select(c => _catManager.CatCollection.GetById(c)).ToList();
            }

            List<EndlessPuzzleCompletionModel> completedPuzzles = new List<EndlessPuzzleCompletionModel>();
            foreach (IEndlessPuzzleCompletionData puzzleCompletionData in currentEndlessRunData.CompletedPuzzles)
            {
                completedPuzzles.Add(DataModelMapper.MapToEndlessPuzzleCompletionModel(puzzleCompletionData, this));
            }

            EndlessRunModel currentEndlessRun = new EndlessRunModel
            {
                CurrentPuzzle = currentPuzzleModel,
                CompletedPuzzles = completedPuzzles,
                Score = currentEndlessRunData.GetScore(),
                IsComplete = currentEndlessRunData.IsComplete
            };

            return currentEndlessRun;
        }

        /// <summary>
        /// Check whether or not a challenge run has been completed at the provided difficulty.
        /// </summary>
        /// <param name="puzzleArea"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public bool IsChallengeRunDifficultyComplete(PuzzleArea puzzleArea, ChallengeRunDifficulty difficulty)
        {
            return _data.IsChallengeRunDifficultyCompleted(puzzleArea.GetId(), difficulty);
        }

        /// <summary>
        /// Get the highest difficulty completed for the given puzzle area.
        /// </summary>
        /// <param name="puzzleArea"></param>
        /// <returns></returns>
        public ChallengeRunDifficulty? GetChallengeRunHighestDifficultyCompletion(PuzzleArea puzzleArea)
        {
            for (int i = 4; i >= 0; i--)
            {
                ChallengeRunDifficulty difficulty = (ChallengeRunDifficulty)i;
                if (IsChallengeRunDifficultyComplete(puzzleArea, difficulty))
                {
                    return difficulty;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the highest score achieved in an endless run
        /// </summary>
        /// <returns>The highest achieved score, or 0 if none.</returns>
        public int GetEndlessRunHighScore(PuzzleArea puzzleArea)
        {
            List<IEndlessRunData> endlessRuns = new List<IEndlessRunData>(_data.GetCompletedEndlessRuns(puzzleArea.GetId()));
            IEndlessRunData currentEndlessRun = _data.GetCurrentEndlessRun(puzzleArea.GetId());
            if (currentEndlessRun != null) endlessRuns.Add(currentEndlessRun);

            return endlessRuns.Any() ? endlessRuns.Max(r => r.GetScore()) : 0;
        }

        /// <summary>
        /// Get the highest score achieved in an endless run excluding the current one
        /// </summary>
        /// <returns>The highest achieved score, or 0 if none.</returns>
        public int GetEndlessRunCompletedHighScore(PuzzleArea puzzleArea)
        {
            List<IEndlessRunData> endlessRuns = _data.GetCompletedEndlessRuns(puzzleArea.GetId());
            return endlessRuns.Any() ? endlessRuns.Max(r => r.GetScore()) : 0;
        }

        /// <summary>
        /// Get the most puzzles completed in an endless run
        /// </summary>
        /// <returns></returns>
        public int GetEndlessRunMostPuzzlesCompleted(PuzzleArea puzzleArea)
        {
            List<IEndlessRunData> endlessRuns = new List<IEndlessRunData>(_data.GetCompletedEndlessRuns(puzzleArea.GetId()));
            IEndlessRunData currentEndlessRun = _data.GetCurrentEndlessRun(puzzleArea.GetId());
            if (currentEndlessRun != null) endlessRuns.Add(currentEndlessRun);
            return endlessRuns.Any() ? endlessRuns.Max(r => r.CompletedPuzzles.Count) : 0;
        }

        /// <summary>
        /// Add cats/stuff to cat blocks in the provided puzzle based on the puzzle area and mode
        /// </summary>
        /// <param name="puzzle"></param>
        /// <param name="puzzleArea"></param>
        /// <param name="puzzleMode"></param>
        /// <param name="wasRestarted"></param>
        /// <returns></returns>
        public Puzzle AddStuffToCatBlocks(Puzzle puzzle, PuzzleArea puzzleArea, PuzzleMode puzzleMode, bool wasRestarted)
        {
            List<CatBlock> catBlocks = new List<CatBlock>();
            foreach (Stack stack in puzzle.Stacks)
            {
                foreach (Block block in stack.Blocks)
                {
                    CatBlock catBlock = block.GetComponent<CatBlock>();
                    if (catBlock)
                    {
                        catBlock.Cat = null;
                        catBlocks.Add(catBlock);
                        if (puzzle.IsSpecial)
                        {
                            catBlock.IsSpecial = true;
                        }
                    }
                }
            }
            int numCatBlocks = catBlocks.Count;

            PuzzleCatBlockContentsInfo catBlockContentsInfo = new PuzzleCatBlockContentsInfo
            {
                PuzzleArea = puzzleArea,
                PuzzleMode = puzzleMode,
                IsLuckyPuzzle = puzzle.IsSpecial
            };
            foreach (CatBlock catBlock in catBlocks)
            {
                CatBlockContents contents = CatBlockContentsGenerator.GenerateCatBlockContents(catBlockContentsInfo);
                if (contents.Cat)
                {
                    catBlock.Cat = contents.Cat;
                    catBlockContentsInfo.CatsAlreadyInBlocks.Add(contents.Cat);
                }
                else
                {
                    catBlock.Yarn = contents.NumSilverPaws;
                }

            }

            return puzzle;
        }

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
            _catManager = _gameManager.Cats;
            _currencyManager = _gameManager.Currency;
            _stuffManager = _gameManager.Stuff;
            _data = GameManager.Instance.Data.PuzzleData;

            _currentArea = PuzzleAreaCollection.GetById(_data.CurrentAreaId);
            _currentStoryPuzzle = GetCurrentStoryPuzzle();
            _currentMode = _data.CurrentMode;
        }

        protected void Start()
        {
            IDailyPuzzleData dailyPuzzles = _data.DailyPuzzles;
            DateTime now = DateTime.Now;
            DateTime dailyPuzzleDate = new DateTime(now.Year, now.Month, now.Day);
            if (dailyPuzzles == null || dailyPuzzles.ForDate != dailyPuzzleDate)
            {
                UpdateDailyPuzzles(dailyPuzzleDate);
            }
        }

        private StoryPuzzle GetCurrentStoryPuzzle()
        {
            string storyPuzzleId = _data.CurrentStoryPuzzleId;

            if (string.IsNullOrEmpty(storyPuzzleId))
            {
                StoryPuzzle currentStoryPuzzle = _currentArea.PuzzleAreaMap.ProgressionPuzzleRoute[0].NormalPuzzle;
                _data.CurrentStoryPuzzleId = currentStoryPuzzle.GetId();
                return currentStoryPuzzle;
            }

            return _currentArea.PuzzleAreaMap.ProgressionPuzzleRoute.FirstOrDefault(ri => ri.NormalPuzzle?.GetId() == storyPuzzleId)?.NormalPuzzle;
        }

        private ChallengePuzzleModel GenerateNextChallengePuzzle(PuzzleArea puzzleArea)
        {
            ChallengeRunModel challengeRunModel = GetCurrentChallengeRun(puzzleArea);

            if (challengeRunModel == null) return null;

            int difficulty = 12 + (int)challengeRunModel.Difficulty * 14 + challengeRunModel.CompletedPuzzles.Count * 2;
            //Puzzle challengePuzzle = OldPuzzleGenerator.GenerateChallengePuzzle(puzzleArea, difficulty);

            Type puzzlePrefabType = puzzleArea.PuzzlePrefab.GetType();
            Puzzle challengePuzzle = null;
            int generationMovesMade = 0;

            if (puzzlePrefabType == typeof(GalaxyFlavoredPuzzle))
            {
                challengePuzzle = GalaxyFlavoredPuzzleBuilder.GenerateNew(puzzleArea, 5, 8, difficulty);
            }
            else if (puzzlePrefabType == typeof(NightFlavoredPuzzle))
            {
                GeneratedPuzzleInfo<PuzzleArea, NightFlavoredPuzzle> generatedPuzzleInfo = new NightFlavoredPuzzleGenerator(puzzleArea).Generate(difficulty);
                challengePuzzle = generatedPuzzleInfo.Puzzle;
                generationMovesMade = generatedPuzzleInfo.NumMovesMade;
            }
            else if (puzzlePrefabType == typeof(JungleFlavoredPuzzle))
            {
                GeneratedPuzzleInfo<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle> generatedPuzzleInfo = new JungleFlavoredPuzzleGenerator((JungleFlavoredPuzzleArea)puzzleArea).Generate(difficulty);
                challengePuzzle = generatedPuzzleInfo.Puzzle;
                generationMovesMade = generatedPuzzleInfo.NumMovesMade;
            }
            else if (puzzlePrefabType == typeof(DesertFlavoredPuzzle))
            {
                GeneratedPuzzleInfo<PuzzleArea, DesertFlavoredPuzzle> generatedPuzzleInfo = new DesertFlavoredPuzzleGenerator(puzzleArea).Generate(difficulty);
                challengePuzzle = generatedPuzzleInfo.Puzzle;
                generationMovesMade = generatedPuzzleInfo.NumMovesMade;
            }
            else if (puzzlePrefabType == typeof(FarmFlavoredPuzzle))
            {
                GeneratedPuzzleInfo<PuzzleArea, FarmFlavoredPuzzle> generatedPuzzleInfo = new FarmFlavoredPuzzleGenerator(puzzleArea).Generate(difficulty);
                challengePuzzle = generatedPuzzleInfo.Puzzle;
                generationMovesMade = generatedPuzzleInfo.NumMovesMade;
            }

            challengePuzzle.IsSpecial = UnityEngine.Random.value >= 0.95f;

            AddStuffToCatBlocks(challengePuzzle, puzzleArea, PuzzleMode.Challenge, false);

            string challengePuzzleJsonData = PuzzleBuilder.GetPuzzleJsonData(challengePuzzle);
            Destroy(challengePuzzle.gameObject);
            int minMoves = generationMovesMade;
            int maxMoves = minMoves > 0 ? (generationMovesMade + 10 - Mathf.FloorToInt(0.1f * difficulty)) : 0;
            IChallengePuzzleData newChallengePuzzle = _data.StartChallengePuzzle(puzzleArea.GetId(), challengePuzzleJsonData, difficulty, minMoves, maxMoves, StartingNumUndos);

            return DataModelMapper.MapToChallengePuzzleModel(newChallengePuzzle);
        }

        private EndlessPuzzleModel GenerateNextEndlessPuzzle(PuzzleArea puzzleArea)
        {
            EndlessRunModel endlessRunModel = GetCurrentEndlessRun(puzzleArea);

            if (endlessRunModel == null) return null;

            int difficulty = 12 + endlessRunModel.CompletedPuzzles.Count * 4;

            Type puzzlePrefabType = puzzleArea.PuzzlePrefab.GetType();
            Puzzle endlessPuzzle = null;
            int generationMovesMade = 0;

            if (puzzlePrefabType == typeof(GalaxyFlavoredPuzzle))
            {
                endlessPuzzle = GalaxyFlavoredPuzzleBuilder.GenerateNew(puzzleArea, 5, 8, difficulty);
            }
            else if (puzzlePrefabType == typeof(NightFlavoredPuzzle))
            {
                GeneratedPuzzleInfo<PuzzleArea, NightFlavoredPuzzle> generatedPuzzleInfo = new NightFlavoredPuzzleGenerator(puzzleArea).Generate(difficulty);
                endlessPuzzle = generatedPuzzleInfo.Puzzle;
                generationMovesMade = generatedPuzzleInfo.NumMovesMade;
            }
            else if (puzzlePrefabType == typeof(JungleFlavoredPuzzle))
            {
                GeneratedPuzzleInfo<JungleFlavoredPuzzleArea, JungleFlavoredPuzzle> generatedPuzzleInfo = new JungleFlavoredPuzzleGenerator((JungleFlavoredPuzzleArea)puzzleArea).Generate(difficulty);
                endlessPuzzle = generatedPuzzleInfo.Puzzle;
                generationMovesMade = generatedPuzzleInfo.NumMovesMade;
            }
            else if (puzzlePrefabType == typeof(DesertFlavoredPuzzle))
            {
                GeneratedPuzzleInfo<PuzzleArea, DesertFlavoredPuzzle> generatedPuzzleInfo = new DesertFlavoredPuzzleGenerator(puzzleArea).Generate(difficulty);
                endlessPuzzle = generatedPuzzleInfo.Puzzle;
                generationMovesMade = generatedPuzzleInfo.NumMovesMade;
            }
            else if (puzzlePrefabType == typeof(FarmFlavoredPuzzle))
            {
                GeneratedPuzzleInfo<PuzzleArea, FarmFlavoredPuzzle> generatedPuzzleInfo = new FarmFlavoredPuzzleGenerator(puzzleArea).Generate(difficulty);
                endlessPuzzle = generatedPuzzleInfo.Puzzle;
                generationMovesMade = generatedPuzzleInfo.NumMovesMade;
            }

            endlessPuzzle.IsSpecial = UnityEngine.Random.value >= 0.95f;

            AddStuffToCatBlocks(endlessPuzzle, puzzleArea, PuzzleMode.Endless, false);

            string endlessPuzzleJsonData = PuzzleBuilder.GetPuzzleJsonData(endlessPuzzle);
            Destroy(endlessPuzzle.gameObject);
            int minMoves = generationMovesMade;
            int maxMoves = minMoves > 0 ? (generationMovesMade + 10 - Mathf.FloorToInt(0.1f * difficulty)) : 0;
            IEndlessPuzzleData newEndlessPuzzle = _data.StartEndlessPuzzle(puzzleArea.GetId(), endlessPuzzleJsonData, difficulty, minMoves, maxMoves, StartingNumUndos);

            return DataModelMapper.MapToEndlessPuzzleModel(newEndlessPuzzle, this);
        }

        private void UpdateDailyPuzzles(DateTime dailyPuzzleDate)
        {
            //DailyPuzzlesModel dailyPuzzles = _data.DailyPuzzlesList.FirstOrDefault(dp => dp.ForDate == dailyPuzzleDate);

            //if (dailyPuzzles == null)
            //{
            //    StartCoroutine(GetDailyPuzzlesList());
            //}
        }

        private IEnumerator GetDailyPuzzlesList()
        {
            using (UnityWebRequest dailyPuzzlesRequest = UnityWebRequest.Get(DailyPuzzlesURL))
            {
                yield return dailyPuzzlesRequest.SendWebRequest();

                if (dailyPuzzlesRequest.isNetworkError)
                {
                    Debug.Log("Unable to fetch daily puzzle data.");
                    yield return new WaitForSeconds(CheckForDailyPuzzlesInterval);
                    StartCoroutine(GetDailyPuzzlesList());
                }
                else
                {
                    // Set puzzles from data.
                    DateTime now = DateTime.Now;
                    DateTime dailyPuzzleDate = new DateTime(now.Year, now.Month, now.Day);
                    UpdateDailyPuzzles(dailyPuzzleDate);
                }
            }
        }

        private Cat PullCat(PuzzleArea puzzleArea, PuzzleMode puzzleMode, bool isSpecial)
        {
            List<Cat> cats = null;
            switch (CurrentMode)
            {
                case PuzzleMode.Story:
                    cats = puzzleArea.ProgressionPuzzleCats;
                    break;
                case PuzzleMode.Challenge:
                    cats = puzzleArea.GeneratedPuzzleCats;
                    break;
            }

            float rarityValue = UnityEngine.Random.value;

            float rareRequirement;
            float uncommonRequirement;

            if (isSpecial)
            {
                rareRequirement = 0.995f;
                uncommonRequirement = 0.92f;
            }
            else
            {
                rareRequirement = 0.9975f;
                uncommonRequirement = 0.96f;
            }


            if (rarityValue >= rareRequirement)
            {
                List<Cat> rareCats = cats.Where(c => c.Rarity == Rarity.Rare).ToList();
                if (rareCats.Count > 0)
                {
                    return rareCats[UnityEngine.Random.Range(0, rareCats.Count)];
                }
            }

            if (rarityValue >= uncommonRequirement)
            {
                List<Cat> uncommonCats = cats.Where(c => c.Rarity == Rarity.Uncommon).ToList();
                if (uncommonCats.Count > 0)
                {
                    return uncommonCats[UnityEngine.Random.Range(0, uncommonCats.Count)];
                }
            }

            List<Cat> commonCats = cats.Where(c => c.Rarity == Rarity.Common).ToList();
            if (commonCats.Count > 0)
            {
                return commonCats[UnityEngine.Random.Range(0, commonCats.Count)];
            }

            return null;
        }

        private void SetCurrentArea(PuzzleArea puzzleArea)
        {
            _currentArea = puzzleArea;
            _data.CurrentAreaId = puzzleArea.GetId();
        }

        private void SetCurrentAreaMode(PuzzleMode mode)
        {
            _currentMode = mode;
            _data.CurrentMode = mode;
        }

        private void SetCurrentStoryPuzzle(StoryPuzzle storyPuzzle)
        {
            _currentStoryPuzzle = storyPuzzle;
            _data.CurrentStoryPuzzleId = storyPuzzle.GetId();
        }
    }
}