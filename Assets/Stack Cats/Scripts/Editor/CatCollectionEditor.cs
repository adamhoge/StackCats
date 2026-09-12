using Tofuwu.StackCats;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(CatCollection))]
public class CatCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CatCollection catCollection = (CatCollection)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Shuffle"))
        {
            var random = new System.Random();
            for (var i = catCollection.List.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);

                var temp = catCollection.List[i];
                catCollection.List[i] = catCollection.List[j];
                catCollection.List[j] = temp;
            }
        }

        if (GUILayout.Button("Number by Order"))
        {
            foreach(var cat in catCollection.List)
            {
                cat.Number = catCollection.List.IndexOf(cat);
            }
        }

        if (GUILayout.Button("Sort"))
        {
            catCollection.List = catCollection.List.OrderBy(c => c.Number != 0 ? c.Number : 999).ToList();
            EditorUtility.SetDirty(catCollection);
        }
    }
}