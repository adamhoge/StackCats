using UnityEngine;

namespace Tofuwu.StackCats
{
    public static class Constants
    {
        public const float MinAspect = 0.45f;
        public const float PreferredAspect = 0.5625f;
        public const float MaxAspect = 1.0f;

        public static Color ClearWhite = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        public static Color ColorBonded = new Color(1.0f, 0.22f, 0.37f, .93f);
        public static Color ColorPositive = new Color(0.79f, 1.0f, 0.5f, 1.0f); //#C9FF80
        public static Color ColorNegative = new Color(1.0f, 0.15f, 0.3f); //#FF264C
        public static Color ColorCommon = Color.white;
        public static Color ColorUncommon = new Color(0.0f, 0.75f, 1.0f);
        public static Color ColorRare = new Color(1.0f, 0.85f, 0.0f);
    }
}
