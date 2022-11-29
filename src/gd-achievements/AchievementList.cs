using Godot;
using GodotAnalysers;
using Antilines.Presentation.Utils;

[SceneReference("AchievementList.tscn")]
public partial class AchievementList
{
    [Export]
    public PackedScene AchievementNotificationScene;

    private IAchievementRepository achievementRepository = new LocalAchievementRepository();

    public override void _Ready()
    {
        base._Ready();
        FillMembers();
    }

    public void ReloadList()
    {
        var achievements = achievementRepository.GetForList();
        
        foreach (Node item in this.achievementsContainer.GetChildren())
        {
            item.QueueFree();
        }

        foreach (var data in achievements)
        {
            var notification = (AchievementNotification)AchievementNotificationScene.Instance();
            this.achievementsContainer.AddChild(notification);
            notification.SetAchievement(data);
        }
    }
}
