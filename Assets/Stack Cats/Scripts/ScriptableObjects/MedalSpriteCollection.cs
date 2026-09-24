using System.Collections.Generic;
using UnityEngine;

namespace RockhopperGames.StackCats
{
    [CreateAssetMenu(fileName = "Medal Collection", menuName = "Stack Cats/Medal Collection")]
    public class MedalSpriteCollection : ScriptableObject
    {
        public List<Sprite> MedalSprites;
        public List<Sprite> MedalOutlineSprites;
    }
}
