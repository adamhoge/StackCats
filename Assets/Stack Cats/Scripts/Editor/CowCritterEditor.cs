using UnityEngine;
using UnityEditor;
using Tofuwu.StackCats;

[CustomEditor(typeof(CowCritter))]
public class CowCritterEditor : Editor
{
    protected void OnSceneGUI()
    {
        CowCritter cowCritter = (CowCritter)target;

        Vector3 moveAreaMin = cowCritter.transform.position + Vector3.left * cowCritter.MovementArea / 2 * cowCritter.transform.localScale.x;
        Vector3 moveAreaMax = cowCritter.transform.position + Vector3.right * cowCritter.MovementArea / 2 * cowCritter.transform.localScale.x;
        Handles.DrawLine(moveAreaMin, moveAreaMax);
    }
}