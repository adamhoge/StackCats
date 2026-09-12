using Tofuwu.StackCats;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StoryPuzzle))]
public class ProgressionPuzzleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StoryPuzzle puzzleData = (StoryPuzzle) target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        
        EditorGUILayout.TextField("ID:", puzzleData.GetId());
    }
}