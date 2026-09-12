using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public abstract class LocalData<T> where T : class
    {
        private readonly string _dataPath;

        protected LocalData(string dataPath)
        {
            _dataPath = dataPath;
        }

        /// <summary>
        /// Load cat data.
        /// </summary>
        /// <param name="dataPath">The path of the cat data object.</param>
        /// <returns>The cat data, or null if it wasn't succesfully loaded.</returns>
        public static T Load(string dataPath)
        {
            T data = null;

            if (File.Exists(dataPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream file = File.Open(dataPath, FileMode.Open);
                data = (T)formatter.Deserialize(file);
                file.Close();
            }

            return data;
        }

        /// <summary>
        /// Save the data.
        /// </summary>
        /// <returns>A flag indicating whether or not the data was succesfully saved.</returns>
        public bool Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Create(_dataPath);
            formatter.Serialize(file, this);
            file.Close();

            return true;
        }
    }
}