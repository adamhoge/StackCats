using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class ProgressionPuzzleRouteItem
    {
        public Vector2 Coordinates;
        public StoryPuzzle NormalPuzzle;
        public StoryPuzzle HardPuzzle;
    }

    public class PuzzleAreaMap : MonoBehaviour
    {
        public PuzzleArea PuzzleArea;
        public List<ProgressionPuzzleRouteItem> ProgressionPuzzleRoute;
        public Vector2 NextAreaCoordinates;
        public int RouteIndexToNextArea;
        public int RouteIndexToChallengeMode;
        public int RouteIndexToEndlessMode;

        public void PlayStoryPuzzle(StoryPuzzle puzzleData)
        {
            GameManager.Instance.PlayStoryPuzzle(PuzzleArea, puzzleData);
        }
    }
}