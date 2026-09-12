namespace Tofuwu.StackCats
{
    public static class ChallengeRunDifficultyHelper
    {
        public static string ToChallengeRunDifficultyString(this ChallengeRunDifficulty challengeRunDifficulty)
        {
            switch (challengeRunDifficulty)
            {
                case ChallengeRunDifficulty.VeryEasy: return "Very Easy";
                case ChallengeRunDifficulty.VeryHard: return "Very Hard";
                default: return challengeRunDifficulty.ToString(); 
            }
        }
    }
}
