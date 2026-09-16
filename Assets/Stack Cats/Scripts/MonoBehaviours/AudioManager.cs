using System;
using System.Collections.Generic;
using System.Linq;
using Tofuwu.StackCats.Data;
using UnityEditor;
using UnityEngine;

namespace Tofuwu.StackCats
{
    // TODO: Possibly handle fade out logic here, possibly handle it in the GameSceneTransitioner, possibly in the GameSceneManager
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// The base modifier for background music level. Used to control the maximum volume.
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float BaseBackgroundMusicVolumeModifier = 1.0f;

        /// <summary>
        /// The prefab used to create the audio source channels.
        /// </summary>
        [Tooltip("The prefab used to create the audio source channels.")]
        public StandardAudioSource StandardAudioSourcePrefab;

        /// <summary>
        /// The maximum number of sounds that can be played before clipping occurs.
        /// </summary>
        [Tooltip("The maximum number of sounds that can be played before clipping occurs.")]
        public int MaxSimultaneousSoundEffects = 16;

        /// <summary>
        /// The audio source used to play background music.
        /// </summary>
        public AudioLooper BackgroundMusic
        {
            get { return _backgroundMusic; }
        }

        /// <summary>
        /// A flag for whether or not overall sound is enabled.
        /// </summary>
        public bool IsMasterEnabled
        {
            get { return _preferenceData.IsMasterEnabled; }
            set { SetMasterEnabled(value); }
        }

        /// <summary>
        /// A flag for whether or not background music is enabled.
        /// </summary>
        public bool IsBackgroundMusicEnabled
        {
            get { return _preferenceData.IsBackgroundMusicEnabled; }
            set { SetBackgroundMusicEnabled(value); }
        }

        /// <summary>
        /// A flag for whether or not sound effects are enabled.
        /// </summary>
        public bool IsSoundEffectsEnabled
        {
            get { return _preferenceData.IsSoundEffectsEnabled; }
            set { SetSoundEffectsEnabled(value); }
        }

        /// <summary>
        /// The current overall sound volume.
        /// </summary>
        public float MasterVolume
        {
            get { return _preferenceData.MasterVolume; }
            set { SetMasterVolume(value); }
        }

        /// <summary>
        /// The current background music volume.
        /// </summary>
        public float BackgroundMusicVolume
        {
            get { return _preferenceData.BackgroundMusicVolume; }
            set { SetBackgroundMusicVolume(value); }
        }

        /// <summary>
        /// The current sound effects volume.
        /// </summary>
        public float SoundEffectsVolume
        {
            get { return _preferenceData.SoundEffectsVolume; }
            set { SetSoundEffectsVolume(value); }
        }

        private IPreferencesData _preferenceData;
        private AudioLooper _backgroundMusic;
        private Dictionary<AudioLoop, float> _backgroundMusicTimes =
            new Dictionary<AudioLoop, float>();
        private Dictionary<Guid, float> _backgroundMusicVolumeModifiers =
            new Dictionary<Guid, float>();
        private readonly List<StandardAudioSource> _audioSourceChannels =
            new List<StandardAudioSource>();

        protected void Awake()
        {
            GameObject channels = new GameObject("Audio Channels");
            channels.transform.SetParent(transform);

            // Get the preference data handler.
            _preferenceData = GameManager.Instance.Data.PreferencesData;

            // Create the background music channel.
            AudioLooper backgroundMusicChannel = new GameObject(
                "Background Music"
            ).AddComponent<AudioLooper>();
            backgroundMusicChannel.transform.SetParent(channels.transform);
            _backgroundMusic = backgroundMusicChannel;
            UpdateBackgroundMusicVolume();

            // Create the sound effect channels.
            for (int i = 0; i < MaxSimultaneousSoundEffects; i++)
            {
                StandardAudioSource audioSourceChannel = Instantiate(
                    StandardAudioSourcePrefab,
                    channels.transform
                );
                audioSourceChannel.name = "Sound Effects Channel " + i;
                _audioSourceChannels.Add(audioSourceChannel);
            }
        }

        /// <summary>
        /// Plays the specified ambience.
        /// </summary>
        /// <param name="ambience">The ambience to play.</param>
        public void PlayAmbience(AudioClip ambience) { }

        /// <summary>
        /// Plays the specified background music.
        /// </summary>
        /// <param name="backgroundMusic">The background music to play.</param>
        public void PlayBackgroundMusic(AudioLoop backgroundMusic, bool resumePosition = true)
        {
            AudioLoop previousBackgroundMusic = _backgroundMusic.CurrentAudioLoop;
            if (previousBackgroundMusic != null)
            {
                if (_backgroundMusicTimes.ContainsKey(previousBackgroundMusic))
                {
                    _backgroundMusicTimes[previousBackgroundMusic] = _backgroundMusic.Time;
                }
                else
                {
                    _backgroundMusicTimes.Add(previousBackgroundMusic, _backgroundMusic.Time);
                }
            }

            _backgroundMusic.PlayAudioLoop(backgroundMusic);

            if (resumePosition && backgroundMusic != null)
            {
                if (_backgroundMusicTimes.ContainsKey(backgroundMusic))
                {
                    _backgroundMusic.Time = _backgroundMusicTimes[backgroundMusic];
                }
            }
        }

        /// <summary>
        /// Add a new volume modifier to background music.
        /// </summary>
        /// <returns>ID of the new modifier.</returns>
        public Guid AddBackgroundMusicVolumeModifier(float volumeModifier = 1.0f)
        {
            Guid modifierGuid = Guid.NewGuid();
            _backgroundMusicVolumeModifiers.Add(modifierGuid, volumeModifier);

            UpdateBackgroundMusicVolume();

            return modifierGuid;
        }

        /// <summary>
        /// Remove a volume modifier from the background music.
        /// </summary>
        /// <param name="volumeModifierGuid"></param>
        public void RemoveBackgroundMusicVolumeModifier(Guid volumeModifierGuid)
        {
            _backgroundMusicVolumeModifiers.Remove(volumeModifierGuid);

            UpdateBackgroundMusicVolume();
        }

        /// <summary>
        /// Set the level of a background music volume modifier
        /// </summary>
        /// <param name="guid">The ID of the modifier.</param>
        /// <param name="volumeModifier">The new value of the modifier.</param>
        public void SetBackgroundMusicVolumeModifier(Guid guid, float volumeModifier)
        {
            if (!_backgroundMusicVolumeModifiers.ContainsKey(guid))
                return;

            _backgroundMusicVolumeModifiers[guid] = volumeModifier;

            UpdateBackgroundMusicVolume();
        }

        /// <summary>
        /// Play a sound effect.
        /// </summary>
        /// <param name="audioEvent">The source audio event to play.</param>
        /// <param name="panning">The stereo panning of the sound effect.</param>
        public StandardAudioSource PlaySoundEffect(AudioEvent audioEvent, float panning = 0.0f)
        {
            if (!audioEvent)
                return null;

            if (IsSoundEffectsEnabled && IsMasterEnabled)
            {
                StandardAudioSource channel = GetOpenSoundEffectsChannel();
                channel.Volume = SoundEffectsVolume * MasterVolume;
                audioEvent.Play(channel.AudioSource, panning);

                return channel;
            }

            return null;
        }

        private void SetMasterEnabled(bool isEnabled)
        {
            _preferenceData.IsMasterEnabled = isEnabled;
            UpdateBackgroundMusicVolume();
        }

        private void SetBackgroundMusicEnabled(bool isEnabled)
        {
            _preferenceData.IsBackgroundMusicEnabled = isEnabled;
            UpdateBackgroundMusicVolume();
        }

        private void SetSoundEffectsEnabled(bool isEnabled)
        {
            _preferenceData.IsSoundEffectsEnabled = isEnabled;
            if (!isEnabled)
            {
                foreach (StandardAudioSource channel in _audioSourceChannels)
                {
                    channel.AudioSource.Stop();
                }
            }
        }

        private void SetMasterVolume(float volume)
        {
            _preferenceData.MasterVolume = volume;
            UpdateBackgroundMusicVolume();
        }

        private void SetBackgroundMusicVolume(float volume)
        {
            _preferenceData.BackgroundMusicVolume = volume;
            UpdateBackgroundMusicVolume();
        }

        private void SetSoundEffectsVolume(float volume)
        {
            _preferenceData.SoundEffectsVolume = volume;
        }

        private void UpdateBackgroundMusicVolume()
        {
            bool isEnabled =
                _preferenceData.IsMasterEnabled && _preferenceData.IsBackgroundMusicEnabled;
            _backgroundMusic.Volume = isEnabled
                ? _preferenceData.BackgroundMusicVolume
                    * BaseBackgroundMusicVolumeModifier
                    * _preferenceData.MasterVolume
                : 0.0f;

            if (_backgroundMusic.Volume != 0.0f)
            {
                foreach (float multiplier in _backgroundMusicVolumeModifiers.Values)
                {
                    _backgroundMusic.Volume *= multiplier;
                }
            }
        }

        private StandardAudioSource GetOpenSoundEffectsChannel()
        {
            StandardAudioSource openChannel = _audioSourceChannels.FirstOrDefault(channel =>
                !channel.IsPlaying
            );

            if (!openChannel)
            {
                foreach (StandardAudioSource channel in _audioSourceChannels)
                {
                    if (
                        !openChannel
                        || openChannel.ClipLength - openChannel.Time
                            > channel.ClipLength - channel.Time
                    )
                    {
                        openChannel = channel;
                    }
                }
            }

            return openChannel;
        }
    }
}
