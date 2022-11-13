using Godot;
using GodotAnalysers;
using IsometricGame.Presentation.Utils;
using System;


[SceneReference("AntiLinesGame.tscn")]
public partial class AntiLinesGame
{
    private AchievementManager achievementManager;

    protected override int LineLength => 3;

    protected override int IncreaseMultiplier => 0;

    protected override Fruit.FruitTypes[] UsedColors => new[] {
                Fruit.FruitTypes.Apple,
                Fruit.FruitTypes.Grape,
                Fruit.FruitTypes.Lemon,
                Fruit.FruitTypes.Pear
            };

    protected override string GameName => "AntiLines";

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
        this.achievementManager = GetNode<AchievementManager>("/root/AchievementManager");
    }

    protected override int GetBestScoreInternal(int bestScore, int currentScore)
    {
        var newBest = currentScore == 0 ? bestScore : Math.Min(bestScore, currentScore);

        if (newBest <= 90)
        {
            achievementManager.UnlockAchievement(Achievements.AntilinesScore90.ToString());
        }
        if (newBest <= 30)
        {
            achievementManager.UnlockAchievement(Achievements.AntilinesScore30.ToString());
        }
        if (newBest <= 9)
        {
            achievementManager.UnlockAchievement(Achievements.AntilinesScore9.ToString());
        }
        return newBest;
    }
}
