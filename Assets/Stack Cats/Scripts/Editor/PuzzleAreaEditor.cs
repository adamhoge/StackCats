using Tofuwu.StackCats;
using UnityEditor;

[CustomEditor(typeof(PuzzleArea))]
public class PuzzleAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PuzzleArea puzzleArea = (PuzzleArea) target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", puzzleArea.GetId());
    }
}