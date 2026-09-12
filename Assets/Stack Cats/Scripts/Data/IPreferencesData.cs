namespace Tofuwu.StackCats.Data
{
    public interface IPreferencesData
    {
        bool IsMasterEnabled { get; set; }
        bool IsBackgroundMusicEnabled { get; set; }
        bool IsSoundEffectsEnabled { get; set; }
        float MasterVolume { get; set; }
        float BackgroundMusicVolume { get; set; }
        float SoundEffectsVolume { get; set; }
    }
}