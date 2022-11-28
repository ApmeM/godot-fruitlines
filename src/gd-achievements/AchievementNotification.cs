using GodotAnalysers;
using Godot;

[SceneReference("AchievementNotification.tscn")]
public partial class AchievementNotification
{
    [Export]
    public float ShowTime {get;set;} = 2;
    
    [Signal]
    public delegate void AnimationFinished();

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.animationPlayer.Connect("animation_finished", this, nameof(AnimationFinishedInternal));
    }

    private void AnimationFinishedInternal(string name) {
        this.EmitSignal(nameof(AnimationFinished));
    }

    public void SetAchievement(Achievement data)
    {
        this.description.Text = data.Name;
        this.textureRect.Texture = ResourceLoader.Load<Texture>(data.IconPath);
    }

    public void ShowAchievement()
    {
        this.animationPlayer.Play("popup");
    }

    public void HideAchievement()
    {
        this.animationPlayer.Play("hide");
    }
}
