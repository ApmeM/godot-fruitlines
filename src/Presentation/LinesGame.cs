using Godot;
using GodotAnalysers;
using IsometricGame.Presentation.Utils;
using System;
using System.Collections.Generic;

[SceneReference("LinesGame.tscn")]
public partial class LinesGame
{
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
    }

    protected override void FruitMovedInternal(List<Fruit> movedFruits)
    {
        base.FruitMovedInternal(movedFruits);

        if (this.Multiplier == 3)
        {
            AchievementRepository.UnlockAchievement(Achievement.LinesMultiply3);
        }
        if (this.Multiplier == 5)
        {
            AchievementRepository.UnlockAchievement(Achievement.LinesMultiply5);
        }
        if (this.Multiplier == 7)
        {
            AchievementRepository.UnlockAchievement(Achievement.LinesMultiply7);
        }

        if (movedFruits.Count > 5)
        {
            AchievementRepository.UnlockAchievement(Achievement.LinesRow6);
        }
        if (movedFruits.Count > 6)
        {
            AchievementRepository.UnlockAchievement(Achievement.LinesRow7);
        }
        if (movedFruits.Count > 8)
        {
            AchievementRepository.UnlockAchievement(Achievement.LinesRow9);
        }

    }

    protected override int GetBestScoreInternal(int bestScore, int currentScore)
    {
        return Math.Max(bestScore, currentScore);
    }
}
