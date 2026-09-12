using UnityEngine;
using System.Collections.Generic;
using System;

namespace Tofuwu.StackCats
{
    [RequireComponent(typeof(PuzzleScene))]
    [RequireComponent(typeof(PuzzleLoader))]
    public class PuzzleUndoer : MonoBehaviour
    {
        public PuzzleScene PuzzleScene;
        public AudioEvent UndoSoundEffect;

        private AudioManager _audioManager;
        private PuzzleLoader _puzzleLoader;
        private Stack<string> _puzzleHistoryData = new Stack<string>();
        private List<string> _openedCatBoxIds = new List<string>();
        private List<Cat> _catSightings = new List<Cat>();

        /// <summary>
        /// Undo the last move made on the current puzzle.
        /// </summary>
        public bool UndoMove()
        {
            if (_puzzleHistoryData.Count < 2) return false;

            _puzzleHistoryData.Pop();
            _puzzleLoader.LoadPuzzle(_puzzleHistoryData.Peek(), PuzzleScene.PuzzleArea, false, true, false);
            return true;
        }

        protected virtual void Awake()
        {
            _audioManager = GameManager.Instance.Audio;
            _puzzleLoader = GetComponent<PuzzleLoader>();
        }

        protected virtual void OnEnable()
        {
            _puzzleLoader.onPuzzleBeginLoad += OnPuzzleBeginLoad;
            _puzzleLoader.onPuzzleBeginUnload += OnPuzzleBeginUnload;
        }

        protected virtual void OnDisable()
        {
            _puzzleLoader.onPuzzleBeginLoad -= OnPuzzleBeginLoad;
            _puzzleLoader.onPuzzleBeginUnload += OnPuzzleBeginUnload;
        }

        private void RecordPuzzleData(Puzzle puzzle)
        {
            _puzzleHistoryData.Push(PuzzleBuilder.GetPuzzleJsonData(puzzle));
        }

        private void OnPuzzleBeginLoad(Puzzle puzzle, bool wasRestarted, bool isUndo)
        {
            puzzle.onBlockMoveResolved += OnBlockMoveResolved;

            if (isUndo)
            {
                LeanTween.scale(puzzle.gameObject, Vector2.one * 0.99f, 0.4f).setEase(LeanTweenType.punch);
                _audioManager.PlaySoundEffect(UndoSoundEffect);
            }
            else
            {
                _puzzleHistoryData.Clear();
                RecordPuzzleData(puzzle);
            }

            foreach (CatBlock catBlock in puzzle.GetAllBlocksOfComponent<CatBlock>())
            {
                if (_openedCatBoxIds.Contains(catBlock.CatBlockId))
                {
                    //catBlock.IsOpened = true;
                    catBlock.Cat = null;
                    catBlock.Yarn = 0;
                }

                catBlock.onCatBlockRemoved += OnCatBlockRemoved;
            }
        }

        private void OnPuzzleBeginUnload(Puzzle puzzle)
        {
            puzzle.onBlockMoveResolved -= OnBlockMoveResolved;
        }

        private void OnBlockMoveResolved(Puzzle puzzle, Stack source, Block block, Stack destination)
        {
            if (!puzzle.IsEditMode)
            {
                RecordPuzzleData(puzzle);
            }
        }

        private void OnCatBlockRemoved(CatBlock catBlock)
        {
            _openedCatBoxIds.Add(catBlock.CatBlockId);
        }
    }
}