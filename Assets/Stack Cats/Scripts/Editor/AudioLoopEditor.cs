using UnityEngine;
using UnityEditor;
using Tofuwu.StackCats;

[CustomEditor(typeof(AudioLoop))]
public class AudioLoopEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AudioLoop audioLoop = (AudioLoop)target;

        base.OnInspectorGUI();

        GUI.enabled = audioLoop.Audio;
        if (GUILayout.Button("Set 'Loop End Time' to Max"))
        {
            audioLoop.LoopEndTime = audioLoop.Audio.length;
        }
        GUI.enabled = true;
    }
}