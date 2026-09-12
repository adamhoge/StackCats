using System;

namespace Tofuwu.StackCats
{
    [Serializable]
    public class CatBlockContents
    {
        public int NumSilverPaws { get; set; }
        public Cat Cat { get; set; }
    }
}
