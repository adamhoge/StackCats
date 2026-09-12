using Tofuwu.StackCats;
using UnityEditor;

[CustomEditor(typeof(JigsawPuzzleObject))]
public class JigsawPuzzleObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        JigsawPuzzleObject jigsawPuzzleObject = (JigsawPuzzleObject) target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", jigsawPuzzleObject.GetId());
    }
}