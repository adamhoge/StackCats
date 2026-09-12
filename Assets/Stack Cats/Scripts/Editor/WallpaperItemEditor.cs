using Tofuwu.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WallpaperItem))]
[CanEditMultipleObjects]
public class WallpaperItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WallpaperItem wallpaperItem = (WallpaperItem)target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", wallpaperItem.GetId());


        if (GUILayout.Button("Regenerate ID"))
        {
            wallpaperItem.RegenerateId();
            EditorUtility.SetDirty(wallpaperItem);
        }
    }
}