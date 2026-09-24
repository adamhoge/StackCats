using RockhopperGames.StackCats;
using RockhopperGames.StackCats.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CurrencyAmountUI))]
public class CurrencyAmountUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CurrencyAmountUI currencyAmountUI = (CurrencyAmountUI)target;

        currencyAmountUI.CurrencyType = (Currency)
            EditorGUILayout.EnumPopup("Currency", currencyAmountUI.CurrencyType);

        currencyAmountUI.Amount = EditorGUILayout.IntField("Amount:", currencyAmountUI.Amount);
    }
}
