using UnityEngine;

namespace Tofuwu.StackCats
{
    public enum AudioReverbDuration
    {
        None,
        Short,
        Medium,
        Long
    }

    public enum AudioEchoDuration
    {
        None,
        Short,
        Medium,
        Long
    }

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioReverbFilter))]
    [RequireComponent(typeof(AudioEchoFilter))]
    public class StandardAudioSource : MonoBehaviour
    {
        /// <summary>
        /// The amount of reverb applied to audio clips.
        /// </summary>
        public AudioReverbDuration AudioReverbDuration { set { SetAudioReverbDuration(value); } }

        /// <summary>
        /// The amount of echo applied to audio clips.
        /// </summary>
        public AudioEchoDuration AudioEchoDuration { set { SetAudioEchoDuration(value); } }

        /// <summary>
        /// The audio source associated with the standard audio source.
        /// </summary>
        public AudioSource AudioSource { get { return _audioSource; } }

        /// <summary>
        /// The volume of the audio source.
        /// </summary>
        public float Volume { get { return _audioSource.volume; } set { _audioSource.volume = value; } }

        /// <summary>
        /// Flag indicating whether or not the audio source is currently playing.
        /// </summary>
        public bool IsPlaying { get { return _audioSource.isPlaying; } }

        /// <summary>
        /// The length of the audio clip in seconds.
        /// </summary>
        public float ClipLength { get { return _audioSource.clip.length; } }

        /// <summary>
        /// Playback position in seconds.
        /// </summary>
        public float Time { get { return _audioSource.time; } }

        private AudioSource _audioSource;
        private AudioReverbFilter _audioReverbFilter;
        private AudioEchoFilter _audioEchoFilter;
        private AudioReverbDuration _audioReverbDuration;
        private AudioEchoDuration _audioEchoDuration;

        protected void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioReverbFilter = GetComponent<AudioReverbFilter>();
            _audioEchoFilter = GetComponent<AudioEchoFilter>();
        }

        private void SetAudioReverbDuration(AudioReverbDuration duration)
        {
            if (_audioReverbDuration == duration) return;

            _audioReverbDuration = duration;
            if (_audioEchoDuration == AudioEchoDuration.None)
            {
                _audioReverbFilter.enabled = false;
            }
            else
            {
                _audioReverbFilter.enabled = true;
                switch (_audioReverbDuration)
                {
                    case AudioReverbDuration.Short:
                        break;
                    case AudioReverbDuration.Medium:
                        break;
                    case AudioReverbDuration.Long:
                        break;
                }
            }
        }

        private void SetAudioEchoDuration(AudioEchoDuration duration)
        {
            if (_audioEchoDuration == duration) return;

            _audioEchoDuration = duration;
            if (_audioEchoDuration == AudioEchoDuration.None)
            {
                _audioEchoFilter.enabled = false;
            }
            else
            {
                _audioEchoFilter.enabled = true;
                switch (_audioEchoDuration)
                {
                    case AudioEchoDuration.Short:
                        break;
                    case AudioEchoDuration.Medium:
                        break;
                    case AudioEchoDuration.Long:
                        break;
                }
            }
        }
    }
}