using System;

namespace Tofuwu.StackCats.Data
{
    public interface IFlagData
    {
        bool IsFlagSet(string flagLabel);
        void SetFlag(string flagLabel, bool value);
    }
}