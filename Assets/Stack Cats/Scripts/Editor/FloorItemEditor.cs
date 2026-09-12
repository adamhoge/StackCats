using Tofuwu.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloorItem))]
[CanEditMultipleObjects]
public class FloorItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FloorItem floorItem = (FloorItem)target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", floorItem.GetId());

        if(GUILayout.Button("Regenerate ID"))
        {
            floorItem.RegenerateId();
            EditorUtility.SetDirty(floorItem);
        }
    }
}