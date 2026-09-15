using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tofuwu.StackCats
{
    public class TestFunctionsController : MonoBehaviour
    {
        public Item TestItem;

        private GameManager _gameManager;
        private PuzzleManager _puzzles;
        private CatManager _cats;
        private CurrencyManager _currency;
        private StuffManager _stuff;
        private int _keypresses = 0;
        private int _addSpecialPuzzlesKeypresses = 0;

        public void AddSpecialPuzzles()
        {
            _puzzles.SpecialPuzzlesRemaining = 5;
        }

        public void UnlockAllAreasAndPuzzles()
        {
            foreach (PuzzleArea area in _puzzles.PuzzleAreaCollection.List)
            {
                _puzzles.UnlockArea(area);
                foreach (
                    StoryPuzzle puzzle in area.PuzzleAreaMap.ProgressionPuzzleRoute.Select(ppr =>
                        ppr.NormalPuzzle
                    )
                )
                {
                    _puzzles.UnlockStoryPuzzle(puzzle);
                    _puzzles.CompleteStoryPuzzle(puzzle, 0, 99);
                }
            }
        }

        public void AddAllCats()
        {
            foreach (Cat cat in _cats.CatCollection.List)
            {
                while (_cats.GetSightingsCount(cat) < cat.BondedAt)
                {
                    _cats.AddCatSighting(cat);
                }
            }
        }

        public void AddAllItems()
        {
            foreach (Item item in _stuff.AllItems)
            {
                if (!_stuff.HasItem(item))
                {
                    _stuff.AddItem(item);
                }
            }

            _currency.ChangeCurrency(Currency.SilverPaw, 1000);
            CurrencyAmountDictionary testCurrency = new CurrencyAmountDictionary();
            testCurrency.Add(Currency.GoldPaw, 1);
            testCurrency.Add(Currency.SilverPaw, 1);
            ItemAmountDictionary testItems = new ItemAmountDictionary();
            testItems.Add(TestItem, 1);
            _stuff.AddPresent(
                GameManager.Instance.Cats.CatCollection.List[0],
                testCurrency,
                testItems
            );
        }

        protected void Awake()
        {
            _gameManager = GameManager.Instance;
            _puzzles = _gameManager.Puzzles;
            _cats = _gameManager.Cats;
            _currency = _gameManager.Currency;
            _stuff = _gameManager.Stuff;
        }

        protected void Update()
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                return;
            }

            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (Input.GetKey(KeyCode.P))
            {
                ++_keypresses;
                if (_keypresses == 5)
                    UnlockAllAreasAndPuzzles();
            }

            if (Input.GetKey(KeyCode.Alpha1))
            {
                ++_addSpecialPuzzlesKeypresses;
                if (_addSpecialPuzzlesKeypresses == 5)
                    _puzzles.SpecialPuzzlesRemaining = 5;
            }
        }
    }
}
