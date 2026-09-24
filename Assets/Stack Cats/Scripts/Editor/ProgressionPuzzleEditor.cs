using RockhopperGames.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StoryPuzzle))]
public class ProgressionPuzzleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StoryPuzzle puzzleData = (StoryPuzzle)target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();

        EditorGUILayout.TextField("ID:", puzzleData.GetId());
    }
}
