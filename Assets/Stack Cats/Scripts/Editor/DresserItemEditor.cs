using Tofuwu.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DresserItem))]
[CanEditMultipleObjects]
public class DresserItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DresserItem dresserItem = (DresserItem)target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", dresserItem.GetId());


        if (GUILayout.Button("Regenerate ID"))
        {
            dresserItem.RegenerateId();
            EditorUtility.SetDirty(dresserItem);
        }
    }
}