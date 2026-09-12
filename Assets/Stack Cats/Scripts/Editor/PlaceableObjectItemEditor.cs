using Tofuwu.StackCats;
using UnityEditor;

[CustomEditor(typeof(PlaceableObjectItem))]
[CanEditMultipleObjects]
public class PlaceableObjectItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlaceableObjectItem placeableItem = (PlaceableObjectItem)target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", placeableItem.GetId());
    }
}