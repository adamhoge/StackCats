using System;

namespace RockhopperGames.StackCats.Data
{
    public interface IFlagData
    {
        bool IsFlagSet(string flagLabel);
        void SetFlag(string flagLabel, bool value);
    }
}
