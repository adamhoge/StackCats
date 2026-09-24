using System.Collections.Generic;
using RockhopperGames.StackCats.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScrollingImage), true)]
[CanEditMultipleObjects]
public class ScrollingImageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
