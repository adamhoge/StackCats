using RockhopperGames.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PressureBlock))]
public class PressureBlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PressureBlock pressureBlock = (PressureBlock)target;

        pressureBlock.BreakingPoint = EditorGUILayout.IntField(
            "Breaking Point:",
            pressureBlock.BreakingPoint
        );
    }
}
