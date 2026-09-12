using Tofuwu.StackCats;
using UnityEditor;

[CustomEditor(typeof(JungleFlavoredPuzzleArea))]
public class JungleFlavoredPuzzleAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        JungleFlavoredPuzzleArea puzzleArea = (JungleFlavoredPuzzleArea) target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", puzzleArea.GetId());
    }
}