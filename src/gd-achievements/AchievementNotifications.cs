using Godot;
using GodotAnalysers;

[SceneReference("AchievementNotification.tscn")]
public partial class AchievementNotifications
{
    [Export]
    public float ShowTime = 4.7f;
    [Export]
    public float HideTime = 1.5f;
    [Export]
    AudioStream GlobalSound = ResourceLoader.Load<AudioStream>("res://gd-achievements/resources/sounds/achievement_earned.wav");

    [Export]
    public float GlobalSoundVolume = -20.0f;

    public enum GROW_DIRECTIONS
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }
    public enum POSITIONS
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT,
    }
    [Export]
    public GROW_DIRECTIONS grow_direction = GROW_DIRECTIONS.DOWN;
    [Export]
    public POSITIONS position = POSITIONS.TOP_LEFT;

    private int achievementCount = 0;
    private AudioStreamPlayer soundNode = new AudioStreamPlayer();

    PackedScene AchievementNotificationScene = ResourceLoader.Load<PackedScene>("res://gd-achievements/AchievementNotification.tscn");

    public void InitSoundNode()
    {
        soundNode.Stream = GlobalSound;
        soundNode.VolumeDb = GlobalSoundVolume;
        AddChild(soundNode);
    }

    public override void _Ready()
    {
        InitSoundNode();

        GetNode<AchievementManager>("/root/AchievementManager").Connect(nameof(AchievementManager.AchievementUnlocked), this, nameof(CreateAchievementPanel));
        SetAnchorsPreset(LayoutPreset.Wide, false);

        MarginTop = 0;
        MarginBottom = 0;
        MarginLeft = 0;
        MarginRight = 0;
    }

    public async void CreateAchievementPanel(Achievement data)
    {
        GD.Print($"AchievementSystem: Show achievement '{data.Name}'");

        var notification_instance = (AchievementNotification)AchievementNotificationScene.Instance();
        AddChild(notification_instance);

        achievementCount += 1;

        if (achievementCount > 1)
        {
            switch (grow_direction)
            {
                case GROW_DIRECTIONS.UP:
                    notification_instance.RectPosition = new Vector2(notification_instance.RectPosition.x, (1 - achievementCount) * (notification_instance.RectSize.y));
                    break;
                case GROW_DIRECTIONS.DOWN:
                    notification_instance.RectPosition = new Vector2(notification_instance.RectPosition.x, (achievementCount - 1) * (notification_instance.RectSize.y));
                    break;
                case GROW_DIRECTIONS.LEFT:
                    notification_instance.RectPosition = new Vector2((1 - achievementCount) * (notification_instance.RectSize.x), notification_instance.RectPosition.y);
                    break;
                case GROW_DIRECTIONS.RIGHT:
                    notification_instance.RectPosition = new Vector2((achievementCount - 1) * (notification_instance.RectSize.x), notification_instance.RectPosition.y);
                    break;
            }
        }

        switch (position)
        {
            case POSITIONS.TOP_RIGHT:
                notification_instance.RectPosition = new Vector2(notification_instance.RectPosition.x + RectSize.x - notification_instance.RectSize.x, notification_instance.RectPosition.y);
                break;
            case POSITIONS.BOTTOM_RIGHT:
                notification_instance.RectPosition = new Vector2(notification_instance.RectPosition.x + RectSize.x - notification_instance.RectSize.x, notification_instance.RectPosition.y + RectSize.y - notification_instance.RectSize.y);
                break;
            case POSITIONS.BOTTOM_LEFT:
                notification_instance.RectPosition = new Vector2(notification_instance.RectPosition.x, notification_instance.RectPosition.y + RectSize.y - notification_instance.RectSize.y);
                break;
        }
        notification_instance.SetAchievement(data);

        soundNode.Play();

        notification_instance.on_show();

        await ToSignal(GetTree().CreateTimer(ShowTime), "timeout");

        achievementCount -= 1;

        notification_instance.on_hide();

        await ToSignal(GetTree().CreateTimer(HideTime), "timeout");

        notification_instance.QueueFree();
    }
}
