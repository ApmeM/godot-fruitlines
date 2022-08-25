using Godot;
using GodotAnalysers;
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

    protected override int GetBestScore(int bestScore, int currentScore)
    {
        return Math.Min(bestScore, currentScore);
    }
}
