using GodotAnalysers;
using Godot;

[SceneReference("AchievementNotification.tscn")]
public partial class AchievementNotification
{
    [Signal]
    public delegate void AnimationFinished();

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    private void AnimationFinishedInternal(string name) {
        this.EmitSignal(nameof(AnimationFinished));
    }

    public void SetAchievement(Achievement data)
    {
        this.description.Text = data.Name;
        this.textureRect.Texture = ResourceLoader.Load<Texture>(data.IconPath);
    }
}
