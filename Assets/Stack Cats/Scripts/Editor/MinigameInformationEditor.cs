using Tofuwu.StackCats;
using UnityEditor;

[CustomEditor(typeof(MinigameInformation))]
public class MinigameInformationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MinigameInformation minigameInformation = (MinigameInformation)target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", minigameInformation.GetId());
    }
}