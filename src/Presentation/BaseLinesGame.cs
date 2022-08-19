using Godot;
using GodotAnalysers;
using System;
using System.Linq;
using IsometricGame.Logic.ScriptHelpers;
using BrainAI.Pathfinding.AStar;
using System.Collections.Generic;
using IsometricGame.Presentation.Utils;
using DodgeTheCreeps.Utils;

[Tool]
[SceneReference("BaseLinesGame.tscn")]
public partial class BaseLinesGame
{
    public int LineLength;
    public int IncreaseMultiplier;
    public Fruit.FruitTypes[] UsedColors;

    private const int Width = 9;
    private const int Height = 9;

    private Random r = new Random();
    private MapGraphData graph = new MapGraphData(Width, Height);
    private Fruit selectedFruit;

    private enum MoveType
    {
        Drop,
        Turn,
    }
    private MoveType moveType = MoveType.Drop;
    private bool isReturnMoveType = false;

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

    private int multiplier = 1;
    private int Multiplier
    {
        get => multiplier;
        set
        {
            this.multiplier = value;
            if (value > 1)
            {
                this.multiplierLabel.Text = $"x{value}";
            }
            else
            {
                this.multiplierLabel.Text = string.Empty;
            }
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

        this.field.Connect(nameof(Field.CellSelected), this, nameof(FieldCellSelected));
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
        this.Multiplier = 1;
        this.graph.ClearMap();
        this.moveType = MoveType.Drop;
        this.isReturnMoveType = false;
        this.selectedFruit = null;
        foreach (Fruit fruit in this.GetTree().GetNodesInGroup(Groups.Fruits))
        {
            fruit.QueueFree();
            this.RemoveChild(fruit);
        }

        AddFruitsToStart();
        TurnStart();
    }

    private void AddFruitsToStart()
    {
        this.AddFruitToStart(this.item1);
        this.AddFruitToStart(this.item2);
        this.AddFruitToStart(this.item3);
    }

    private void AddFruitToStart(Position2D position)
    {
        var fruit = this.FruitScene.Instance<Fruit>();
        fruit.FruitType = UsedColors[r.Next(UsedColors.Length)];
        fruit.Position = position.Position;
        fruit.AddToGroup(Groups.Fruits);
        fruit.AddToGroup(Groups.FruitsAtStart);
        fruit.Connect(nameof(Fruit.FruitMoved), this, nameof(FruitMoved));
        fruit.Connect(nameof(Fruit.FruitClicked), this, nameof(FruitClicked));
        this.AddChild(fruit);
    }

    private void TurnStart()
    {
        System.Diagnostics.Debug.Assert(this.GetTree().GetNodesInGroup(Groups.FruitsMoving).Count == 0);

        var fruits = this.GetTree().GetNodesInGroup(Groups.FruitsAtStart);
        foreach (Fruit fruit in fruits)
        {
            Vector2 mapPos;
            do
            {
                mapPos = new Vector2(r.Next(Width), r.Next(Height));
            } while (this.graph.Map[(int)mapPos.x, (int)mapPos.y] != null);

            this.graph.AddFruit(fruit, mapPos);

            fruit.Drop(this.field.MapToWorld(mapPos));
            fruit.RemoveFromGroup(Groups.FruitsAtStart);
        }

        AddFruitsToStart();
    }

    private void FruitClicked(Fruit fruit)
    {
        if (fruit.IsInGroup(Groups.FruitsAtStart))
        {
            return;
        }

        if (this.GetTree().GetNodesInGroup(Groups.FruitsMoving).Count > 0)
        {
            return;
        }

        this.selectedFruit?.DeselectFruit();
        this.selectedFruit = fruit;
        fruit.SelectFruit();
    }

    private void FruitMoved(Fruit fruit)
    {
        fruit.AddToGroup(Groups.FruitsMoved);
        if (this.GetTree().GetNodesInGroup(Groups.FruitsMoving).Count != 0)
        {
            return;
        }

        var movedFruits = this.GetTree().GetNodesInGroup(Groups.FruitsMoved);
        foreach (Fruit movedFruit in movedFruits)
        {
            movedFruit.RemoveFromGroup(Groups.FruitsMoved);
        }

        if (this.CheckGameOver())
        {
            return;
        }

        if (isReturnMoveType)
        {
            foreach (Fruit movedFruit in movedFruits)
            {
                movedFruit.QueueFree();
            }

            this.moveType = MoveType.Turn;
            this.isReturnMoveType = false;
            this.Multiplier += this.IncreaseMultiplier;
        }
        else
        {
            var fruitsToRemove = new HashSet<Fruit>();
            foreach (Fruit movedFruit in movedFruits)
            {
                foreach (var fruitToRemove in this.graph.FindLines(movedFruit, this.LineLength))
                {
                    fruitsToRemove.Add(fruitToRemove);
                }
            }

            if (fruitsToRemove.Any())
            {
                foreach (var fruitToRemove in fruitsToRemove)
                {
                    this.graph.RemoveFruit(fruitToRemove);
                    fruitToRemove.Drop(this.basket.RectGlobalPosition + this.basket.RectSize / 2);
                }
                this.isReturnMoveType = true;
                this.CurrentScore += fruitsToRemove.Count * this.Multiplier;
            }
            else
            {
                this.moveType = this.moveType == MoveType.Turn ? MoveType.Drop : MoveType.Turn;
                this.isReturnMoveType = false;
                if (this.moveType == MoveType.Drop)
                {
                    TurnStart();
                    this.Multiplier = 1;
                }
            }
        }
    }

    private void FieldCellSelected(Vector2 cell)
    {
        if (this.GetTree().GetNodesInGroup(Groups.FruitsMoving).Count > 0 || this.selectedFruit == null)
        {
            return;
        }

        var path = AStarPathfinder.Search(this.graph, this.graph.Fruits[this.selectedFruit], cell);
        if (path == null || path.Count == 1)
        {
            return;
        }

        this.graph.MoveFruit(this.selectedFruit, cell);
        this.selectedFruit.DeselectFruit();
        this.selectedFruit.Turn(path.Select(a => this.field.MapToWorld(a)).ToArray());
        this.selectedFruit = null;
    }

    private bool CheckGameOver()
    {
        return this.graph.Fruits.Count > Width * Height - 3;
    }
}
