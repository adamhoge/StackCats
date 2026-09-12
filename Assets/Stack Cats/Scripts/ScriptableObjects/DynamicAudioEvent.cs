using System.Linq;
using UnityEngine;

namespace Tofuwu.StackCats
{
    [System.Serializable]
    public class AudioClipCandidate
    {
        public AudioClip AudioClip;
        public double Probability;
    }

    /// <summary>
    /// Uses a list of audio clips and a range for volume and pitch to create variety for a particular sound event.
    /// </summary>
    [CreateAssetMenu(fileName = "Dynamic Audio", menuName = "Stack Cats/Audio/Dynamic Audio")]
    public class DynamicAudioEvent : AudioEvent
    {
        public AudioClipCandidate[] AudioClipCandidates;

        public float Pitch = 1.0f;

        [Range(0.0f, 1.0f)]
        public float Volume = 1.0f;

        public float PitchVariance = 0.0f;

        public float VolumeVariance = 0.0f;

        /// <summary>
        /// Play the audio.
        /// </summary>
        /// <param name="audioSource">The audio source used to play the audio.</param>
        /// <param name="panning">The desired panning of the audio clip.</param>
        public override void Play(AudioSource audioSource, float panning = 0.0f)
        {
            if (audioSource)
            {
                audioSource.pitch = Pitch - PitchVariance / 2.0f + Random.Range(0, PitchVariance);
                audioSource.volume *= Volume - VolumeVariance + Random.Range(0, VolumeVariance);
                audioSource.clip = GetAudioClip();
                audioSource.panStereo = panning;
                audioSource.Play();
            }
        }

        private AudioClip GetAudioClip()
        {
            double probabilitySum = AudioClipCandidates.Sum(c => c.Probability);
            double pickValue = Random.value * probabilitySum;
            AudioClip selectedAudioClip = null;
            double pickTotal = 0.0f;
            foreach (AudioClipCandidate candidate in AudioClipCandidates)
            {
                pickTotal += candidate.Probability;
                if (pickValue <= pickTotal)
                {
                    selectedAudioClip = candidate.AudioClip;
                    break;
                }
            }

            return selectedAudioClip;
        }
    }
}