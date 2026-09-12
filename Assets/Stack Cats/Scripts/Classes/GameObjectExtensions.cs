using UnityEngine;

namespace Tofuwu.StackCats
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Clone the entire heirarchy of a given game object for components of type "T"
        /// </summary>
        /// <typeparam name="T">The cloned component type</typeparam>
        /// <param name="clonedObject">The cloned gameobject</param>
        /// <returns>The clone of the provided gameobject or null if the component doesn't exist in the heirarchy.</returns>
        public static GameObject CloneComponentHeirarchy<T>(GameObject clonedObject) where T : Component
        {
            if (!clonedObject.GetComponentInChildren<T>()) return null;

            GameObject newGameObjectNode = new GameObject(clonedObject.name + "(" + typeof(T).Name + " Component Clone)");
            newGameObjectNode.transform.position = clonedObject.transform.position;
            newGameObjectNode.transform.rotation = clonedObject.transform.rotation;
            newGameObjectNode.transform.localScale = clonedObject.transform.localScale;
            
            foreach (T component in clonedObject.GetComponents<T>())
            {
                newGameObjectNode.AddComponent<T>(component);
            }

            for (int i = 0; i < clonedObject.transform.childCount; i++)
            {
                GameObject newGameObjectChildNode = CloneComponentHeirarchy<T>(clonedObject.transform.GetChild(i).gameObject);
                if(newGameObjectChildNode) newGameObjectChildNode.transform.SetParent(newGameObjectNode.transform);
            }

            return newGameObjectNode;
        }
    }
}
