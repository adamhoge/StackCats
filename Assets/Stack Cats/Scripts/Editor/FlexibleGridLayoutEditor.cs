using UnityEditor;

[CustomEditor(typeof(FlexibleGridLayout), true)]
[CanEditMultipleObjects]
public class FlexibleGridLayoutEditor : Editor
{
    SerializedProperty _padding;
    SerializedProperty _spacing;
    SerializedProperty _startCorner;
    SerializedProperty _startAxis;
    SerializedProperty _childAlignment;
    SerializedProperty _columnCount;
    SerializedProperty _rowCount;

    protected virtual void OnEnable()
    {
        _padding = serializedObject.FindProperty("m_Padding");
        _spacing = serializedObject.FindProperty("m_Spacing");
        _startCorner = serializedObject.FindProperty("m_StartCorner");
        _startAxis = serializedObject.FindProperty("m_StartAxis");
        _childAlignment = serializedObject.FindProperty("m_ChildAlignment");
        _columnCount = serializedObject.FindProperty("ColumnCount");
        _rowCount = serializedObject.FindProperty("RowCount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_padding, true);
        EditorGUILayout.PropertyField(_spacing, true);
        EditorGUILayout.PropertyField(_startCorner, true);
        EditorGUILayout.PropertyField(_startAxis, true);
        EditorGUILayout.PropertyField(_childAlignment, true);
        EditorGUILayout.PropertyField(_columnCount, true);
        EditorGUILayout.PropertyField(_rowCount, true);
        serializedObject.ApplyModifiedProperties();
    }
}