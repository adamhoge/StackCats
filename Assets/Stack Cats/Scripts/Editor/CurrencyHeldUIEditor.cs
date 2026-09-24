using RockhopperGames.StackCats;
using RockhopperGames.StackCats.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CurrencyHeldUI))]
public class CurrencyHeldUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CurrencyHeldUI currencyHeldUI = (CurrencyHeldUI)target;

        currencyHeldUI.CurrencyType = (Currency)
            EditorGUILayout.EnumPopup("Currency", currencyHeldUI.CurrencyType);
    }
}
