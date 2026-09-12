using UnityEngine;

namespace Tofuwu.StackCats
{
    public abstract class AudioEvent : ScriptableObject
    {
        public abstract void Play(AudioSource audioSource, float panning = 0.0f);
    }
}