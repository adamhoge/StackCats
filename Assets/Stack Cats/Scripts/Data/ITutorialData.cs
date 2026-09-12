namespace Tofuwu.StackCats.Data
{
    public interface ITutorialData
    {
        TutorialSceneState TutorialState { get; set; }
        void SetTypeKnown(string typeName);
        bool IsTypeKnown(string typeName);
    }
}