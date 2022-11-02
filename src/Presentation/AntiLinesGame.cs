using Godot;
using GodotAnalysers;
using IsometricGame.Presentation.Utils;
using System;


[SceneReference("AntiLinesGame.tscn")]
public partial class AntiLinesGame
{
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
    }

    protected override int GetBestScoreInternal(int bestScore, int currentScore)
    {
        var newBest = currentScore == 0 ? bestScore : Math.Min(bestScore, currentScore);

        if (newBest <= 90)
        {
            AchievementRepository.UnlockAchievement(Achievement.AntilinesScore90);
        }
        if (newBest <= 30)
        {
            AchievementRepository.UnlockAchievement(Achievement.AntilinesScore30);
        }
        if (newBest <= 9)
        {
            AchievementRepository.UnlockAchievement(Achievement.AntilinesScore9);
        }
        return newBest;
    }
}
