using System;
using System.Collections.Generic;
using Godot;
using GodotAnalysers;
using Antilines.Presentation.Utils;
using MazeGenerators;
using MazeGenerators.Utils;

[SceneReference("LifeGame.tscn")]
public partial class LifeGame
{
    protected override string GameName => nameof(LifeGame);

    private readonly Fruit.FruitTypes[] UsedColors = new[] {
                Fruit.FruitTypes.Grape,
            };
    private AchievementNotifications achievementNotifications;
    private Fluent fluent;
    private Random random = new Random();

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
        this.achievementNotifications = GetNode<AchievementNotifications>("/root/Main/AchievementNotifications");
        this.startButton.Connect(CommonSignals.Pressed, this, nameof(StartLife));
        this.pauseButton.Connect(CommonSignals.Pressed, this, nameof(StopLife));
        this.randomButton.Connect(CommonSignals.Pressed, this, nameof(RandomLife));
        this.timer.Connect(CommonSignals.Timeout, this, nameof(TickLife));

        RestartInternal();
    }

    private void StartLife()
    {
        this.startButton.Visible = false;
        this.pauseButton.Visible = true;
        this.timer.Start();
    }

    private void StopLife()
    {
        this.startButton.Visible = true;
        this.pauseButton.Visible = false;
        this.timer.Stop();
    }

    private void RandomLife()
    {
        var fruits = this.GetTree().GetNodesInGroup(Groups.Fruits);
        foreach (Node fruit in fruits)
        {
            fruit.QueueFree();
        }
        this.graph.ClearMap();

        RestartInternal();

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                if (random.NextDouble() > 0.5)
                {
                    var fruit = this.FruitScene.Instance<Fruit>();
                    fruit.FruitType = UsedColors[r.Next(UsedColors.Length)];
                    fruit.Position = this.field.MapToWorld(new Godot.Vector2(x, y));
                    fruit.AddToGroup(Groups.Fruits);
                    this.AddChild(fruit);
                    this.fluent.result.Paths[x, y] = 1;
                    this.graph.AddFruit(fruit, new Godot.Vector2(x, y));
                }
            }
        }
        this.SaveState();
    }

    private void TickLife()
    {
        var fruits = this.GetTree().GetNodesInGroup(Groups.Fruits);
        foreach (Node fruit in fruits)
        {
            fruit.QueueFree();
        }
        this.graph.ClearMap();

        fluent.Life(1, 1, 0, (n) => n == 3, (n) => n < 3 || n > 4);

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                if (fluent.result.Paths[x, y] == 1)
                {
                    var fruit = this.FruitScene.Instance<Fruit>();
                    fruit.FruitType = UsedColors[r.Next(UsedColors.Length)];
                    fruit.Position = this.field.MapToWorld(new Godot.Vector2(x, y));
                    fruit.AddToGroup(Groups.Fruits);
                    this.AddChild(fruit);
                    this.graph.AddFruit(fruit, new Godot.Vector2(x, y));
                }
            }
        }
        this.SaveState();
    }

    protected override void FieldCellSelectedInternal(Godot.Vector2 cell)
    {
        if (this.fluent.result.Paths[(int)cell.x, (int)cell.y] == 0)
        {
            var fruit = this.FruitScene.Instance<Fruit>();
            fruit.FruitType = UsedColors[r.Next(UsedColors.Length)];
            fruit.Position = this.field.MapToWorld(cell);
            fruit.AddToGroup(Groups.Fruits);
            this.AddChild(fruit);
            this.fluent.result.Paths[(int)cell.x, (int)cell.y] = 1;
            this.graph.AddFruit(fruit, new Godot.Vector2((int)cell.x, (int)cell.y));
            this.SaveState();
        }
    }

    protected override void FruitMovedInternal(List<Fruit> movedFruits)
    {
    }

    protected override int GetBestScoreInternal(int bestScore, int currentScore)
    {
        return 0;
    }

    protected override void LoadInternal(GameRepository.GameState state)
    {
    }

    protected override void RestartInternal()
    {
        var settings = new GeneratorSettings
        {
            Height = Height,
            Width = Width,
        };

        this.fluent = Fluent.Build(settings).GenerateField();
    }

    protected override bool IsGameOver()
    {
        return false;
    }
}
