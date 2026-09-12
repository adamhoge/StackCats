using Tofuwu.StackCats;
using UnityEditor;

[CustomEditor(typeof(GiftItem))]
[CanEditMultipleObjects]
public class GiftItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GiftItem giftItem = (GiftItem) target;

        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.TextField("ID:", giftItem.GetId());
    }
}