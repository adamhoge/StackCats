using Tofuwu.StackCats.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MedalFlashEffectUI))]
public class MedalFlashEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MedalFlashEffectUI medalFlashEffect = (MedalFlashEffectUI)target;

        medalFlashEffect.NumPuzzlesCompleted = EditorGUILayout.IntField("Num Puzzles Completed:", medalFlashEffect.NumPuzzlesCompleted);
        if (medalFlashEffect.NumPuzzlesCompleted < 1) medalFlashEffect.NumPuzzlesCompleted = 1;

        medalFlashEffect.HasOutline = EditorGUILayout.Toggle("Has Outline:", medalFlashEffect.HasOutline);

        if (GUILayout.Button("Flash"))
        {
            medalFlashEffect.Flash();
        }
    }
}
