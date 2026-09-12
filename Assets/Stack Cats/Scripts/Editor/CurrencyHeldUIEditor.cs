using UnityEngine;
using UnityEditor;
using Tofuwu.StackCats;
using Tofuwu.StackCats.UI;

[CustomEditor(typeof(CurrencyHeldUI))]
public class CurrencyHeldUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CurrencyHeldUI currencyHeldUI = (CurrencyHeldUI)target;

        currencyHeldUI.CurrencyType = (Currency)EditorGUILayout.EnumPopup("Currency", currencyHeldUI.CurrencyType);
    }
}