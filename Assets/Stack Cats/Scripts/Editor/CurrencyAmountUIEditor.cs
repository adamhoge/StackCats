using UnityEngine;
using UnityEditor;
using Tofuwu.StackCats;
using Tofuwu.StackCats.UI;

[CustomEditor(typeof(CurrencyAmountUI))]
public class CurrencyAmountUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CurrencyAmountUI currencyAmountUI = (CurrencyAmountUI)target;

        currencyAmountUI.CurrencyType = (Currency)EditorGUILayout.EnumPopup("Currency", currencyAmountUI.CurrencyType);

        currencyAmountUI.Amount = EditorGUILayout.IntField("Amount:", currencyAmountUI.Amount);
    }
}