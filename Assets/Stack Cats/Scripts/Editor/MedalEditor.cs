using Tofuwu.StackCats.UI;
using UnityEditor;


[CustomEditor(typeof(MedalUI))]
public class MedalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MedalUI medal = (MedalUI)target;

        medal.NumPuzzlesCompleted = EditorGUILayout.IntField("Num Puzzles Completed:", medal.NumPuzzlesCompleted);
        if (medal.NumPuzzlesCompleted < 1) medal.NumPuzzlesCompleted = 1;

        medal.HasOutline = EditorGUILayout.Toggle("Has Outline:", medal.HasOutline);
    }
}
