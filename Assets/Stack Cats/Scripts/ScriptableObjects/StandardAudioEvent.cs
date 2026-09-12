using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public class StandardAudioEvent : AudioEvent
    {
        public AudioClip AudioClip;

        /// <summary>
        /// Play the audio.
        /// </summary>
        /// <param name="audioSource">The audio source used to play the audio.</param>
        /// <param name="panning">The desired panning of the audio clip.</param>
        public override void Play(AudioSource audioSource, float panning = 0.0f)
        {
            audioSource.clip = AudioClip;
            audioSource.Play();
        }
    }
}