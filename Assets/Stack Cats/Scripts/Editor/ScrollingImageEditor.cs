using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tofuwu.StackCats.UI;

[CustomEditor(typeof(ScrollingImage), true)]
[CanEditMultipleObjects]
public class ScrollingImageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}