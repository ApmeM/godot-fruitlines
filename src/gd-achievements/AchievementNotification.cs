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
        this.description.Text = data.Description;

        if (data.Achieved)
        {
            this.mainTitle.Text = data.Name + " Unlocked";
            this.textureRect.Texture = ResourceLoader.Load<Texture>(data.IconPath);
            this.description.Text += "\n\nUnlocked at " + data.UnlockDate?.ToString("dd-MMM-yyyy");
        }
        else
        {
            this.mainTitle.Text = data.Name + " Locked";
            this.description.AddColorOverride("font_color", Color.Color8(150, 150, 150));
            this.mainTitle.AddColorOverride("font_color", Color.Color8(150, 150, 150));
        }
    }
}
