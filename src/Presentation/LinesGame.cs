using Godot;
using GodotAnalysers;
using IsometricGame.Presentation.Utils;
using System;
using System.Collections.Generic;

[SceneReference("LinesGame.tscn")]
public partial class LinesGame
{
    private AchievementNotifications achievementNotifications;

    protected override int LineLength => 5;

    protected override int IncreaseMultiplier => 1;

    protected override Fruit.FruitTypes[] UsedColors => new[] {
                Fruit.FruitTypes.Apple,
                Fruit.FruitTypes.Banana,
                Fruit.FruitTypes.Cherry,
                Fruit.FruitTypes.Grape,
                Fruit.FruitTypes.Lemon,
                Fruit.FruitTypes.Pear,
                Fruit.FruitTypes.Pineaple
            };

    protected override string GameName => "Lines";

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
        this.achievementNotifications = GetNode<AchievementNotifications>("/root/Main/AchievementNotifications");
    }

    protected override void FruitMovedInternal(List<Fruit> movedFruits)
    {
        base.FruitMovedInternal(movedFruits);

        if (this.Multiplier == 3)
        {
            achievementNotifications.UnlockAchievement(Achievements.LinesMultiply3.ToString());
        }
        if (this.Multiplier == 5)
        {
            achievementNotifications.UnlockAchievement(Achievements.LinesMultiply5.ToString());
        }
        if (this.Multiplier == 7)
        {
            achievementNotifications.UnlockAchievement(Achievements.LinesMultiply7.ToString());
        }

        if (movedFruits.Count > 5)
        {
            achievementNotifications.UnlockAchievement(Achievements.LinesRow6.ToString());
        }
        if (movedFruits.Count > 6)
        {
            achievementNotifications.UnlockAchievement(Achievements.LinesRow7.ToString());
        }
        if (movedFruits.Count > 8)
        {
            achievementNotifications.UnlockAchievement(Achievements.LinesRow9.ToString());
        }

    }

    protected override int GetBestScoreInternal(int bestScore, int currentScore)
    {
        return Math.Max(bestScore, currentScore);
    }
}
