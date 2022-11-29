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

    private void AnimationFinishedInternal(string name)
    {
        this.EmitSignal(nameof(AnimationFinished));
    }

    public void SetAchievement(Achievement data)
    {
        if (data.Achieved)
        {
            this.mainTitle.Text = "Achievement Unlocked!";
        }
        else
        {
            this.mainTitle.Text = "Achievement LOCKED!";
            this.description.AddColorOverride("font_color", Color.Color8(150, 150, 150));
            this.mainTitle.AddColorOverride("font_color", Color.Color8(150, 150, 150));
        }

        this.description.Text = data.Name;
        this.textureRect.Texture = ResourceLoader.Load<Texture>(data.IconPath);
    }
}
