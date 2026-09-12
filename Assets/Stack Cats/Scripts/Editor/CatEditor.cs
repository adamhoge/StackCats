using Tofuwu.StackCats;
using UnityEditor;

[CustomEditor(typeof(Cat))]
public class CatEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Cat cat = (Cat) target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", cat.GetId());
    }
}