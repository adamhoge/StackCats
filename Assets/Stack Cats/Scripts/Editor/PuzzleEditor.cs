using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tofuwu.StackCats;

[CustomEditor(typeof(Puzzle), true)]
public class PuzzleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Puzzle puzzle = (Puzzle)target;

        puzzle.StackSpacing = EditorGUILayout.FloatField("Stack Spacing", puzzle.StackSpacing);

        if (GUILayout.Button("Add New Stack"))
        {
            puzzle.AddNewStack();
        }
    }

    protected void OnSceneGUI()
    {
        Puzzle puzzle = (Puzzle)target;

        Stack<Stack> destroyStacks = new Stack<Stack>();
        float size = 0.25f;
        int numStacks = puzzle.Stacks.Count;
        for (int i = 0; i < numStacks; i++)
        {
            Stack stack = puzzle.Stacks[i];
            float xPosition = (-(float)(numStacks - 1) / 2 + i) * (1 + puzzle.StackSpacing);

            Handles.color = Color.white;

            Vector3 addPuzzleBlockPosition = puzzle.transform.position + new Vector3(xPosition - 0.25f, -1.0f, 0.0f);
            if (Handles.Button(addPuzzleBlockPosition, Quaternion.identity, size, size, Handles.RectangleHandleCap))
            {
                puzzle.AddNewPuzzleBlock(stack);
            }

            Handles.color = Color.yellow;

            Vector3 addCatBlockPosition = puzzle.transform.position + new Vector3(xPosition + 0.25f, -1.0f, 0.0f);
            if (Handles.Button(addCatBlockPosition, Quaternion.identity, size, size, Handles.RectangleHandleCap))
            {
                puzzle.AddNewCatBlock(stack);
            }

            Handles.color = Color.magenta;

            Vector3 removePuzzleBlockPosition = puzzle.transform.position + new Vector3(xPosition - 0.25f, -1.5f, 0.0f);
            if (Handles.Button(removePuzzleBlockPosition, Quaternion.identity, size, size, Handles.RectangleHandleCap))
            {
                Block removedBlock = stack.TopBlock;
                if (stack.RemoveBlock(stack.TopBlock))
                {
                    DestroyImmediate(removedBlock.gameObject);
                }
            }

            Handles.color = Color.red;

            Vector3 removestackPosition = puzzle.transform.position + new Vector3(xPosition + 0.25f, -1.5f, 0.0f);
            if (Handles.Button(removestackPosition, Quaternion.identity, size, size, Handles.RectangleHandleCap))
            {
                destroyStacks.Push(stack);
            }
        }

        while (destroyStacks.Count > 0)
        {
            Stack stack = destroyStacks.Pop();
            if (puzzle.RemoveStack(stack))
            {
                Undo.RecordObject(puzzle, "Remove Stack");
                DestroyImmediate(stack.gameObject);
            }
        }
    }
}