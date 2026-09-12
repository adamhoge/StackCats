namespace Tofuwu.StackCats
{
    public static class CatPersonalityHelper
    {
        public static string ToCatPersonalityString(this CatPersonality catPersonality)
        {
            switch (catPersonality)
            {
                case CatPersonality.MildMannered: return "Mild-Mannered";
                default: return catPersonality.ToString(); 
            }
        }
    }
}
