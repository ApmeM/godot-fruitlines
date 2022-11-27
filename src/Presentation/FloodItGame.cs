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

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
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
                var position = new Vector2(x, y);

                var fruit = this.FruitScene.Instance<Fruit>();
                fruit.FruitType = UsedColors[r.Next(UsedColors.Length)];
                fruit.Position = this.field.MapToWorld(position) + Vector2.Up * 300;
                fruit.AddToGroup(Groups.Fruits);
                this.AddChild(fruit);
                this.graph.AddFruit(fruit, position);


                fruit.Drop(this.field.MapToWorld(position));
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
        var fruit = this.FruitScene.Instance<Fruit>();
        fruit.FruitType = fruitType;
        fruit.Position = position;
        fruit.AddToGroup(Groups.Fruits);
        this.AddChild(fruit);
    }

    protected override bool IsGameOver()
    {
        return this.graph.GetArea(0, 0).Count == Width * Height;
    }
}
