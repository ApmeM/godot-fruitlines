using Godot;
using GodotAnalysers;
using System;
using System.Linq;
using IsometricGame.Logic.ScriptHelpers;
using BrainAI.Pathfinding.AStar;
using System.Collections.Generic;
using IsometricGame.Presentation.Utils;
using DodgeTheCreeps.Utils;

[SceneReference("FloodIt.tscn")]
public partial class FloodIt
{
    private const int Width = 9;
    private const int Height = 9;

    public Fruit.FruitTypes[] UsedColors = new[] {
                Fruit.FruitTypes.Apple,
                Fruit.FruitTypes.Banana,
                Fruit.FruitTypes.Cherry,
                Fruit.FruitTypes.Grape,
                Fruit.FruitTypes.Lemon,
                Fruit.FruitTypes.Pear,
                Fruit.FruitTypes.Pineaple
            };
    private Random r = new Random();
    private MapGraphData graph = new MapGraphData(Width, Height);

    private int currentScore = 0;
    private int CurrentScore
    {
        get => this.currentScore;
        set
        {
            this.currentScore = value;
            this.currentScoreLabel.Text = $"{value}";
        }
    }

    [Export]
    public PackedScene FruitScene;

    [Signal]
    public delegate void Close();

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.restartButton.Connect(CommonSignals.Pressed, this, nameof(Restart));
        this.backButton.Connect(CommonSignals.Pressed, this, nameof(Back));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        this.hUD.Visible = this.Visible;
    }

    public void Start()
    {
        Restart();
    }

    private void Back()
    {
        this.EmitSignal(nameof(Close));
    }

    private void Restart()
    {
        this.CurrentScore = 0;
        this.graph.ClearMap();
        foreach (Fruit fruit in this.GetTree().GetNodesInGroup(Groups.Fruits))
        {
            fruit.QueueFree();
            this.RemoveChild(fruit);
        }

        AddFruitsToStart();
        InitializeField();
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
        fruit.Connect(nameof(Fruit.FruitClicked), this, nameof(FruitClicked));
        this.AddChild(fruit);
    }

    private void InitializeField()
    {
        System.Diagnostics.Debug.Assert(this.GetTree().GetNodesInGroup(Groups.FruitsMoving).Count == 0);

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var position = new Vector2(x, y);

                var fruit = this.FruitScene.Instance<Fruit>();
                fruit.FruitType = UsedColors[r.Next(UsedColors.Length)];
                fruit.Position = this.field.MapToWorld(position) + Vector2.Up * 300;
                fruit.AddToGroup(Groups.Fruits);
                fruit.Connect(nameof(Fruit.FruitClicked), this, nameof(FruitClicked));
                this.AddChild(fruit);
                this.graph.AddFruit(fruit, position);

                fruit.Drop(this.field.MapToWorld(position));
            }
    }

    private void FruitClicked(Fruit fruit)
    {
        if (this.GetTree().GetNodesInGroup(Groups.FruitsMoving).Count > 0)
        {
            return;
        }

        var area = this.graph.GetArea(0, 0);
        foreach (var cell in area)
        {
            this.graph.Map[(int)cell.x, (int)cell.y].FruitType = fruit.FruitType;
        }
        this.CurrentScore++;
    }

    private bool CheckGameOver()
    {
        return this.graph.GetArea(0, 0).Count > Width * Height;
    }
}
