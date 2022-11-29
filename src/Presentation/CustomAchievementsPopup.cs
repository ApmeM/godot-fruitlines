using Godot;

using GodotAnalysers;

[SceneReference("CustomAchievementsPopup.tscn")]
public partial class CustomAchievementsPopup
{
    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    public void ReloadList()
    {
        this.achievementList.ReloadList();
    }
}
