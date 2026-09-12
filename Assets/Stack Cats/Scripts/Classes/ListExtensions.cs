using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public static class ListExtensions
    {
        public static T SelectRandom<T>(this List<T> list) where T : class
        {
            return list != null && list.Count > 0 ? list[Random.Range(0, list.Count)] : null;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            List<T> shuffledList = new List<T>(list);
            for (int i = 0; i < shuffledList.Count; i++)
            {
                T temp = shuffledList[i];
                int randomIndex = Random.Range(i, shuffledList.Count);
                shuffledList[i] = shuffledList[randomIndex];
                shuffledList[randomIndex] = temp;
            }

            return shuffledList;
        }
    }
}
