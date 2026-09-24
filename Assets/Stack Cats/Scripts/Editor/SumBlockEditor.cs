using RockhopperGames.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SumBlock))]
public class SumBlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SumBlock sumBlock = (SumBlock)target;

        sumBlock.SumValue = EditorGUILayout.IntField("Sum Value:", sumBlock.SumValue);
    }
}
