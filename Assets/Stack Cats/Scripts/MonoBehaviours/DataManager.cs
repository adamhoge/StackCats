using System;
using System.Collections.Generic;
using System.IO;
using Tofuwu.StackCats.Data;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class DataManager : MonoBehaviour
    {
        public List<PuzzleArea> StartingAreas;
        public List<WallpaperItem> StartingWallpapers;
        public List<FloorItem> StartingFloors;
        public List<WindowItem> StartingWindows;
        public List<DresserItem> StartingDressers;
        public int NumItemsPlaceableOnDresser = 5;

        public IPreferencesData PreferencesData { get => _preferencesData; }
        public ITutorialData TutorialData { get => _tutorialData; }
        public IPuzzleData PuzzleData { get => _puzzleData; }
        public ICatCollectionData CatCollectionData { get => _catCollectionData; }
        public ICurrencyData CurrencyData { get => _currencyData; }
        public IStuffData StuffData { get => _stuffData; }
        public IHomeData HomeData { get => _homeData; }
        public IMinigameData MinigameData { get => _minigameData; }
        public IEventData EventData { get { return _eventData; } }
        public IFlagData FlagData { get { return _flagData; } }

        private IPreferencesData _preferencesData;
        private ITutorialData _tutorialData;
        private IPuzzleData _puzzleData;
        private ICatCollectionData _catCollectionData;
        private ICurrencyData _currencyData;
        private IStuffData _stuffData;
        private IHomeData _homeData;
        private IMinigameData _minigameData;
        private IEventData _eventData;
        private IFlagData _flagData;

        private const string WebGlPath = "idbfs/stackcats-0cb099d7-ede1-4d66-ba99-8a57dfd1b948";

        protected void Awake()
        {
#if UNITY_WEBGL
            if (!Debug.isDebugBuild && !Directory.Exists(WebGlPath))
            {
                Directory.CreateDirectory(WebGlPath);
            }
#endif

            string applicationPath = GetDataPath();

            string preferencesPath = applicationPath + "/preferences.dat";
            string tutorialPath = applicationPath + "/tutorial.dat";
            string puzzlesPath = applicationPath + "/puzzles.dat";
            string catsPath = applicationPath + "/cats.dat";
            string currencyPath = applicationPath + "/currency.dat";
            string stuffPath = applicationPath + "/stuff.dat";
            string homePath = applicationPath + "/home.dat";
            string minigamesPath = applicationPath + "/minigames.dat";
            string eventsPath = applicationPath + "/events.dat";
            string flagsPath = applicationPath + "/flags.dat";

            _preferencesData = LocalPreferencesData.Load(preferencesPath) ?? CreatePreferencesData(preferencesPath);
            _tutorialData = LocalTutorialData.Load(tutorialPath) ?? CreateTutorialData(tutorialPath);
            _puzzleData = LocalPuzzleData.Load(puzzlesPath) ?? CreatePuzzleData(puzzlesPath);
            _catCollectionData = LocalCatCollectionData.Load(catsPath) ?? CreateCatCollectionData(catsPath);
            _currencyData = LocalCurrencyData.Load(currencyPath) ?? CreateCurrencyData(currencyPath);
            _stuffData = LocalStuffData.Load(stuffPath) ?? CreateStuffData(stuffPath);
            _homeData = LocalHomeData.Load(homePath) ?? CreateHomeData(homePath);
            _minigameData = LocalMinigameData.Load(minigamesPath) ?? CreateMinigameData(minigamesPath);
            _eventData = LocalEventData.Load(eventsPath) ?? CreateEventData(eventsPath);
            _flagData = LocalFlagData.Load(flagsPath) ?? CreateFlagData(flagsPath);
        }

        private string GetDataPath()
        {
#if UNITY_WEBGL
            if (!Debug.isDebugBuild)
            {
                return WebGlPath;
            }
#endif

            return Application.persistentDataPath;
        }

        private LocalPreferencesData CreatePreferencesData(string path)
        {
            LocalPreferencesData preferencesData = new LocalPreferencesData(path);
            return preferencesData;
        }

        private LocalTutorialData CreateTutorialData(string path)
        {
            LocalTutorialData tutorialData = new LocalTutorialData(path);
            return tutorialData;
        }

        private LocalPuzzleData CreatePuzzleData(string path)
        {
            LocalPuzzleData puzzleData = new LocalPuzzleData(path);
            foreach (PuzzleArea area in StartingAreas)
            {
                puzzleData.UnlockArea(area.GetId());
                puzzleData.UnlockProgressionPuzzle(area.PuzzleAreaMap.ProgressionPuzzleRoute[0].NormalPuzzle.GetId());
            }
            puzzleData.CurrentAreaId = StartingAreas[0].GetId();
            puzzleData.CurrentMode = PuzzleMode.Story;
            puzzleData.DefaultChallengeRunDifficulty = ChallengeRunDifficulty.Easy;
            return puzzleData;
        }

        private LocalCatCollectionData CreateCatCollectionData(string path)
        {
            LocalCatCollectionData catCollectionData = new LocalCatCollectionData(path);
            return catCollectionData;
        }

        private LocalCurrencyData CreateCurrencyData(string path)
        {
            LocalCurrencyData currencyData = new LocalCurrencyData(path);
            return currencyData;
        }

        private LocalStuffData CreateStuffData(string path)
        {
            LocalStuffData stuffData = new LocalStuffData(path);
            foreach (WallpaperItem wallpaper in StartingWallpapers) stuffData.AddItem(wallpaper.GetId(), 1);
            foreach (FloorItem floor in StartingFloors) stuffData.AddItem(floor.GetId(), 1);
            foreach (WindowItem window in StartingWindows) stuffData.AddItem(window.GetId(), 1);
            foreach (DresserItem dresser in StartingDressers) stuffData.AddItem(dresser.GetId(), 1);
            return stuffData;
        }

        private LocalHomeData CreateHomeData(string path)
        {
            LocalHomeData homeData = new LocalHomeData(path, NumItemsPlaceableOnDresser);
            homeData.CurrentWallpaperId = StartingWallpapers[0].GetId();
            homeData.CurrentFloorId = StartingFloors[0].GetId();
            homeData.CurrentWindowId = StartingWindows[0].GetId();
            homeData.CurrentDresserId = StartingDressers[0].GetId();
            return homeData;
        }

        private LocalMinigameData CreateMinigameData(string path)
        {
            LocalMinigameData minigameData = new LocalMinigameData(path);
            minigameData.LastMinigamePlayedTime = DateTime.Now;
            return minigameData;
        }

        private LocalEventData CreateEventData(string path)
        {
            LocalEventData eventData = new LocalEventData(path);
            return eventData;
        }

        private LocalFlagData CreateFlagData(string path)
        {
            LocalFlagData eventData = new LocalFlagData(path);
            return eventData;
        }
    }
}