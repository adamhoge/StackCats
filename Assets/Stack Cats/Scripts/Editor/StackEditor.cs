using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tofuwu.StackCats;

[CustomEditor(typeof(Stack), true)]
[CanEditMultipleObjects]
public class StackEditor : Editor
{
    private float _blockHeight;
    private int _maxBlocks;

    protected void Awake()
    {
        Stack stack = (Stack) target;

        _blockHeight = stack.BlockHeight;
        _maxBlocks = stack.MaxBlocks;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        _maxBlocks = EditorGUILayout.IntField("Max Blocks", _maxBlocks);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (Object obj in targets)
            {
                Stack stack = ((Stack)obj);
                while (stack.Blocks.Count >= _maxBlocks)
                {
                    Block topBlock = stack.TopBlock;
                    stack.RemoveBlock(topBlock);
                    DestroyImmediate(topBlock.gameObject);
                }

                ((Stack)obj).MaxBlocks = _maxBlocks;
            }
        }

        EditorGUI.BeginChangeCheck();
        _blockHeight = EditorGUILayout.FloatField("Block Height", _blockHeight);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (Object obj in targets)
            {
                ((Stack)obj).BlockHeight = _blockHeight;
            }
        }
    }
}