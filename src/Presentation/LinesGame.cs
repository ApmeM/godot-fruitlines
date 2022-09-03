using Godot;
using GodotAnalysers;
using System;


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

    protected override int GetBestScoreInternal(int bestScore, int currentScore)
    {
        return Math.Max(bestScore, currentScore);
    }
}
