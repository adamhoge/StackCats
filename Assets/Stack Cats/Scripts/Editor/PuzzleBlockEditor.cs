using RockhopperGames.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PuzzleBlock))]
public class PuzzleBlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PuzzleBlock puzzleBlock = (PuzzleBlock)target;

        puzzleBlock.PrimaryNumber = EditorGUILayout.IntField(
            "Primary Number:",
            puzzleBlock.PrimaryNumber
        );

        puzzleBlock.SecondaryNumber = EditorGUILayout.IntField(
            "Secondary Number:",
            puzzleBlock.SecondaryNumber
        );
    }
}
