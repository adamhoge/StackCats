using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockhopperGames.StackCats
{
    [CreateAssetMenu(fileName = "Audio Loop", menuName = "Stack Cats/Audio/Audio Loop")]
    public class AudioLoop : ScriptableObject
    {
        public AudioClip Audio;

        public float LoopBeginTime;

        public float LoopEndTime;
    }
}
