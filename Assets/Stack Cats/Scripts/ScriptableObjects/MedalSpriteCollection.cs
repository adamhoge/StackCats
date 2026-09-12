using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [CreateAssetMenu(fileName = "Medal Collection", menuName = "Stack Cats/Medal Collection")]
    public class MedalSpriteCollection : ScriptableObject
    {
        public List<Sprite> MedalSprites;
        public List<Sprite> MedalOutlineSprites;
    }
}