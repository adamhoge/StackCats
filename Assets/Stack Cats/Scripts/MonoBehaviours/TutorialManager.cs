using Tofuwu.StackCats.Data;
using Tofuwu.StackCats.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void PuzzleTutorialStarted(PuzzleTutorial puzzleTutorial);
    public delegate void BlockTutorialStarted(BlockTutorial blockTutorial);

    public class TutorialManager : MonoBehaviour
    {
        public event PuzzleTutorialStarted onPuzzleTutorialStarted;
        public event BlockTutorialStarted onBlockTutorialStarted;

        public List<PuzzleTutorial> PuzzleTutorials = new List<PuzzleTutorial>();
        public List<BlockTutorial> BlockTutorials = new List<BlockTutorial>();

        private ITutorialData _tutorialData;

        protected void Awake()
        {
            _tutorialData = GameManager.Instance.Data.TutorialData;
        }

        public void BeginTutorialsForPuzzle(Puzzle puzzle)
        {
            Type puzzleType = puzzle.GetType();
            if (!IsTypeKnown(puzzleType))
            {
                PuzzleTutorial puzzleTutorial = GetPuzzleTutorialForType(puzzleType);

                if (puzzleTutorial) BeginPuzzleTutorial(puzzleTutorial);
            }

            foreach (Type blockType in puzzle.GetAllBlockTypes())
            {
                if (!IsTypeKnown(blockType))
                {
                    BlockTutorial blockTutorial = GetBlockTutorialForType(blockType);
                    if (blockTutorial) BeginBlockTutorial(blockTutorial);
                }
            }
        }

        public bool IsTypeKnown(Type type)
        {
            return _tutorialData.IsTypeKnown(type.Name);
        }

        public PuzzleTutorial GetPuzzleTutorialForType(Type puzzleType)
        {
            return PuzzleTutorials.FirstOrDefault(t => t.TypeName == puzzleType.Name);
        }

        public BlockTutorial GetBlockTutorialForType(Type blockType)
        {
            return BlockTutorials.FirstOrDefault(t => t.TypeName == blockType.Name);
        }

        public void BeginPuzzleTutorial(PuzzleTutorial puzzleTutorial)
        {
            if (onPuzzleTutorialStarted != null) onPuzzleTutorialStarted(puzzleTutorial);
        }

        public void BeginBlockTutorial(BlockTutorial blockTutorial)
        {
            if (onBlockTutorialStarted != null) onBlockTutorialStarted(blockTutorial);
        }

        public void CompleteTutorial(string typeName)
        {
            _tutorialData.SetTypeKnown(typeName);
        }
    }
}