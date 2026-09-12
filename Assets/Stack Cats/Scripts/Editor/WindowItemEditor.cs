using Tofuwu.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WindowItem))]
[CanEditMultipleObjects]
public class WindowItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WindowItem windowItem = (WindowItem)target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", windowItem.GetId());


        if (GUILayout.Button("Regenerate ID"))
        {
            windowItem.RegenerateId();
            EditorUtility.SetDirty(windowItem);
        }
    }
}