namespace Tofuwu.StackCats
{
    public static class MathHelpers
    {
        public static float TakePercent(ref float currentPercent, float amount)
        {
            float takenPercent = currentPercent * amount;
            currentPercent -= takenPercent;
            return takenPercent;
        }
    }
}
