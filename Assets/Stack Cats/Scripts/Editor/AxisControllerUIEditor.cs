using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tofuwu.StackCats;
using Tofuwu.StackCats.UI;

[CustomEditor(typeof(AxisControllerUI), true)]
public class AxisControllerUIEditor : Editor
{
    private PuzzleController _puzzleController;

    protected void Awake()
    {
        AxisControllerUI axisControllerUi = (AxisControllerUI) target;

        _puzzleController = axisControllerUi.PuzzleController;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AxisControllerUI axisControllerUi = (AxisControllerUI)target;

        EditorGUI.BeginChangeCheck();
        _puzzleController = (PuzzleController)EditorGUILayout.ObjectField("Puzzle Controller", _puzzleController, typeof(PuzzleController), true);
        if (EditorGUI.EndChangeCheck())
        {
            axisControllerUi.PuzzleController = _puzzleController;
        }
    }
}