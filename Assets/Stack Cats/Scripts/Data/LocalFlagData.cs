using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalFlagData : LocalData<LocalFlagData>, IFlagData
    {
        private Dictionary<string, bool> _flags = new Dictionary<string, bool>();

        public LocalFlagData(string dataPath) : base(dataPath) { }

        public bool IsFlagSet(string flagLabel)
        {
            return _flags.ContainsKey(flagLabel) && _flags[flagLabel];
        }

        public void SetFlag(string flagLabel, bool isSet)
        {
            if (_flags.ContainsKey(flagLabel))
            {
                _flags[flagLabel] = isSet;
            }
            else
            {
                _flags.Add(flagLabel, isSet);
            }
            Save();
        }
    }
}