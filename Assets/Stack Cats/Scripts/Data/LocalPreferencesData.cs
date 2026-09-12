using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalPreferencesData : IPreferencesData
    {
        /// <summary>
        /// Flag indicating whether or not overall sound is currently enabled.
        /// </summary>
        public bool IsMasterEnabled { get { return _isMasterEnabled; } set { _isMasterEnabled = value; Save(); } }

        /// <summary>
        /// Flag indicating whether or not background music is currently enabled.
        /// </summary>
        public bool IsBackgroundMusicEnabled { get { return _isBackgroundMusicEnabled; } set { _isBackgroundMusicEnabled = value; Save(); } }

        /// <summary>
        /// Flag indicating whether or not sound is currently enabled.
        /// </summary>
        public bool IsSoundEffectsEnabled { get { return _isSoundEffectsEnabled; } set { _isSoundEffectsEnabled = value; Save(); } }

        /// <summary>
        /// The current overall sound volume.
        /// </summary>
        public float MasterVolume { get { return _masterVolume; } set { _masterVolume = value; Save(); } }

        /// <summary>
        /// The current background music volume.
        /// </summary>
        public float BackgroundMusicVolume { get { return _backgroundMusicVolume; } set { _backgroundMusicVolume = value; Save(); } }

        /// <summary>
        /// The current sound effects volume.
        /// </summary>
        public float SoundEffectsVolume { get { return _soundEffectsVolume; } set { _soundEffectsVolume = value; Save(); } }

        private readonly string _dataPath;
        private bool _isMasterEnabled = true;
        private bool _isBackgroundMusicEnabled = true;
        private bool _isSoundEffectsEnabled = true;
        private float _masterVolume = 1.0f;
        private float _backgroundMusicVolume = 0.5f;
        private float _soundEffectsVolume = 1.0f;

        public LocalPreferencesData(string dataPath)
        {
            _dataPath = dataPath;
            Save();
        }

        /// <summary>
        /// Load existing preferences.
        /// </summary>
        /// <param name="dataPath">The path of the preferences data</param>
        /// <returns>The preferences data, or null if it wasn't successfully loaded.</returns>
        public static LocalPreferencesData Load(string dataPath)
        {
            LocalPreferencesData data = null;

            if (File.Exists(dataPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream file = File.Open(dataPath, FileMode.Open);
                data = (LocalPreferencesData)formatter.Deserialize(file);
                file.Close();
            }

            return data;
        }

        /// <summary>
        /// Save the preferences data.
        /// </summary>
        /// <returns>A flag indicating whether or not the data was successfully saved.</returns>
        public bool Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Create(_dataPath);
            formatter.Serialize(file, this);
            file.Close();

            return false;
        }
    }
}