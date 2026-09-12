using Tofuwu.StackCats;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PuzzleAreaMap))]
public class PuzzleAreaMapEditor : Editor
{
    protected void OnSceneGUI()
    {
        PuzzleAreaMap puzzleAreaMap = (PuzzleAreaMap)target;

        Handles.color = new Color(1.0f, 1.0f, 0.0f, 0.1f);

        Handles.color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        for (int i = 0; i < puzzleAreaMap.ProgressionPuzzleRoute.Count; i++)
        {
            ProgressionPuzzleRouteItem routeItem = puzzleAreaMap.ProgressionPuzzleRoute[i];

            if (i > 0)
            {
                ProgressionPuzzleRouteItem lastRouteItem = puzzleAreaMap.ProgressionPuzzleRoute[i - 1];

                Vector2 startTangent;
                if(i > 1)
                {
                    Vector2 tangentRouteCoordinates = puzzleAreaMap.ProgressionPuzzleRoute[i - 2].Coordinates;
                    startTangent = lastRouteItem.Coordinates - (tangentRouteCoordinates - lastRouteItem.Coordinates) / 5;
                }
                else
                {
                    startTangent = lastRouteItem.Coordinates;
                }

                Vector2 endTangent;
                if(i + 1 < puzzleAreaMap.ProgressionPuzzleRoute.Count)
                {
                    Vector2 tangentRouteCoordinates = puzzleAreaMap.ProgressionPuzzleRoute[i + 1].Coordinates;
                    endTangent = routeItem.Coordinates - (tangentRouteCoordinates - routeItem.Coordinates) / 5;
                }
                else
                {
                    endTangent = routeItem.Coordinates;
                }

                Handles.DrawBezier(
                    lastRouteItem.Coordinates,
                    routeItem.Coordinates,
                    startTangent,
                    endTangent,
                    Color.white,
                    null,
                    8.0f);
            }

            Handles.DrawSolidDisc(routeItem.Coordinates, Vector3.forward, 0.045f);

            EditorGUI.BeginChangeCheck();
            Vector2 newRouteCoordinatesPosition = Handles.PositionHandle(routeItem.Coordinates, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(puzzleAreaMap, "Change route coordinates");
                routeItem.Coordinates = newRouteCoordinatesPosition;
            }
        }
    }
}