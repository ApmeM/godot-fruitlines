using System.Collections.Generic;
using Godot;
using GodotAnalysers;
using Antilines.Presentation.Utils;

[SceneReference("FloodItGame.tscn")]
public partial class FloodItGame
{
    protected override string GameName => nameof(FloodItGame);

    private readonly Fruit.FruitTypes[] UsedColors = new[] {
                Fruit.FruitTypes.Apple,
                Fruit.FruitTypes.Banana,
                Fruit.FruitTypes.Cherry,
                Fruit.FruitTypes.Grape,
                Fruit.FruitTypes.Lemon,
                Fruit.FruitTypes.Pear,
                Fruit.FruitTypes.Pineaple
            };

    private AchievementNotifications achievementNotifications;

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
        this.achievementNotifications = GetNode<AchievementNotifications>("/root/Main/AchievementNotifications");
    }

    protected override void FieldCellSelectedInternal(Vector2 cell)
    {
        var fruit = this.graph.Map[(int)cell.x, (int)cell.y];
        if (fruit.FruitType == this.graph.Map[0, 0].FruitType)
        {
            return;
        }

        var area = this.graph.GetArea(0, 0);
        foreach (var item in area)
        {
            this.graph.Map[(int)item.x, (int)item.y].FruitType = fruit.FruitType;
        }

        this.CurrentScore++;

        this.CheckGameOver();
    }

    protected override void FruitMovedInternal(List<Fruit> movedFruits)
    {
    }

    protected override int GetBestScoreInternal(int bestScore, int currentScore)
    {
        if (currentScore <= 20)
        {
            achievementNotifications.UnlockAchievement(Achievements.FloodIt20.ToString());
        }
        if (currentScore <= 18)
        {
            achievementNotifications.UnlockAchievement(Achievements.FloodIt18.ToString());
        }
        if (currentScore <= 15)
        {
            achievementNotifications.UnlockAchievement(Achievements.FloodIt15.ToString());
        }

        return Mathf.Max(bestScore, currentScore);
    }

    protected override void LoadInternal(GameRepository.GameState state)
    {
        AddFruitsToStart();
    }

    protected override void RestartInternal()
    {
        AddFruitsToStart();

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var cell = new Vector2(x, y);
                var fruit = CreateNewFruit(UsedColors[r.Next(UsedColors.Length)], cell);
                fruit.Position = this.field.MapToWorld(cell) + Vector2.Up * 300;
                fruit.Drop(this.field.MapToWorld(cell));
            }
    }

    private void AddFruitsToStart()
    {
        for (var i = 0; i < UsedColors.Length; i++)
        {
            this.AddFruitToStart(UsedColors[i], new Vector2(60 + i * 60, 140));
        }
    }

    private void AddFruitToStart(Fruit.FruitTypes fruitType, Vector2 position)
    {
        var fruit = CreateNewFruit(fruitType, null);
        fruit.Position = position;
    }

    protected override bool IsGameOver()
    {
        return this.graph.GetArea(0, 0).Count == Width * Height;
    }
}
