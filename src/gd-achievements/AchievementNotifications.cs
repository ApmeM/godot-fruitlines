using Godot;
using GodotAnalysers;
using Antilines.Presentation.Utils;

[SceneReference("AchievementNotifications.tscn")]
public partial class AchievementNotifications
{
    [Export]
    public float ShowTime{get; set;} = 1;

    [Export]
    public float MoveTime{get; set;} = 1;

    [Export]
    public AudioStream GlobalSound
    {
        get { return this.audioStreamPlayer.Stream; }
        set { this.audioStreamPlayer.Stream = value; }
    }

    [Export]
    public float GlobalSoundVolume
    {
        get { return this.audioStreamPlayer.VolumeDb; }
        set { this.audioStreamPlayer.VolumeDb = value; }
    }
    [Export]
    public PackedScene AchievementNotificationScene;

    private IAchievementRepository achievementRepository = new LocalAchievementRepository();

    public override void _Ready()
    {
        base._Ready();
        FillMembers();
        this.tween.Start();
    }

    public void ProgressAchievement(string key, int progress)
    {
        ProcessAchievement(key, achievementRepository.ProgressAchievement(key, progress));
    }

    public void UnlockAchievement(string key)
    {
        ProcessAchievement(key, achievementRepository.UnlockAchievement(key));
    }

    private void ProcessAchievement(string key, bool isOperationSuccess)
    {
        if (!isOperationSuccess)
        {
            return;
        }

        var data = achievementRepository.GetAchievement(key);
        CreateAchievementPanel(data);
    }

    private async void CreateAchievementPanel(Achievement data)
    {
        GD.Print($"Achievement System: Show achievement '{data.Name}'");

        var notification = (AchievementNotification)AchievementNotificationScene.Instance();
        this.achievementsContainer.AddChild(notification);
        notification.SetAchievement(data);

        this.audioStreamPlayer.Play();

        this.tween.InterpolateProperty(notification, new NodePath("rect_position:x"), -500, 0, this.MoveTime);
        this.tween.Start();
        await ToSignal(GetTree().CreateTimer(this.MoveTime), "timeout");
        await ToSignal(GetTree().CreateTimer(ShowTime), "timeout");
        this.tween.InterpolateProperty(notification, new NodePath("rect_position:x"), 0, -500, this.MoveTime);
        this.tween.Start();
        await ToSignal(GetTree().CreateTimer(this.MoveTime), "timeout");

        notification.QueueFree();
    }
}
