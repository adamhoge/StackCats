using Tofuwu.StackCats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CutScenePlayer))]
public class CutScenePlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CutScenePlayer cutScenePlayer = (CutScenePlayer)target;

        if (GUILayout.Button("Restart"))
        {
            cutScenePlayer.Restart();
        }
    }
}
