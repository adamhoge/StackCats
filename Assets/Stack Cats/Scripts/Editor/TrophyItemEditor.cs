using Tofuwu.StackCats;
using UnityEditor;

[CustomEditor(typeof(TrophyItem))]
[CanEditMultipleObjects]
public class TrophyItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TrophyItem trophyItem = (TrophyItem) target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", trophyItem.GetId());
    }
}