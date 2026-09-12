using UnityEngine;

namespace Tofuwu.StackCats
{
    public class AudioLooper : MonoBehaviour
    {
        public float Volume { get { return _track1.volume; } set { _track1.volume = value; _track2.volume = value; } }

        public AudioLoop CurrentAudioLoop { get { return _currentAudioLoop; } }

        public float Time { get { return _currentTrack.time; } set { _currentTrack.time = value; } }

        private AudioSource _track1;
        private AudioSource _track2;
        private AudioSource _currentTrack;
        private AudioLoop _currentAudioLoop;

        public void PlayAudioLoop(AudioLoop audioLoop)
        {
            _currentAudioLoop = audioLoop;

            if (!_currentAudioLoop)
            {
                Stop();
                return;
            }

            _track1.clip = audioLoop.Audio;
            _track2.clip = audioLoop.Audio;

            _track2.Stop();

            _currentTrack = _track1;
            _currentTrack.time = 0.0f;
            _currentTrack.Play();

        }

        public void Stop()
        {
            _track1.Stop();
            _track2.Stop();
        }

        protected void Awake()
        {
            _track1 = new GameObject().AddComponent<AudioSource>();
            _track1.transform.SetParent(transform);

            _track2 = new GameObject().AddComponent<AudioSource>();
            _track2.transform.SetParent(transform);
        }

        protected void Update()
        {
            if (_currentAudioLoop && _currentTrack.time >= _currentAudioLoop.LoopEndTime)
            {
                _currentTrack = _currentTrack == _track1 ? _track2 : _track1;
                _currentTrack.time = _currentAudioLoop.LoopBeginTime;
                _currentTrack.Play();
            }
        }
    }
}