using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class IDCollection<T> : ScriptableObject where T : IIdentifiable
    {
        public List<T> List;

        public T GetById(string id)
        {
            return List.FirstOrDefault(i => i.GetId() == id);
        }
    }
}