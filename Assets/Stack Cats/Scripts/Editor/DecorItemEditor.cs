using Tofuwu.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DecorItem))]
[CanEditMultipleObjects]
public class DecorItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DecorItem decorationItem = (DecorItem) target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", decorationItem.GetId());


        if (GUILayout.Button("Regenerate ID"))
        {
            decorationItem.RegenerateId();
            EditorUtility.SetDirty(decorationItem);
        }
    }
}