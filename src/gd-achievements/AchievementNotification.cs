using GodotAnalysers;
using Godot;

[SceneReference("AchievementNotification.tscn")]
public partial class AchievementNotification
{
    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    public void SetAchievement(Achievement data)
    {
        this.description.Text = data.Name;
        this.textureRect.Texture = ResourceLoader.Load<Texture>(data.IconPath);
    }

    public void on_show()
    {
        this.animationPlayer.Play("popup");
    }

    public void on_hide()
    {
        this.animationPlayer.Play("hide");
    }
}
