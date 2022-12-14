using Godot;
using GodotAnalysers;
using System;
using System.Linq;
using Antilines.Logic.ScriptHelpers;
using BrainAI.Pathfinding.AStar;
using System.Collections.Generic;
using Antilines.Presentation.Utils;

[SceneReference("BaseLinesGame.tscn")]
public abstract partial class BaseLinesGame
{
    protected abstract int LineLength { get; }
    protected abstract int IncreaseMultiplier { get; }
    protected abstract Fruit.FruitTypes[] UsedColors { get; }

    private Fruit selectedFruit;

    private enum MoveType
    {
        Drop,
        Turn,
    }
    private MoveType moveType;
    private bool isReturnMoveType = false;

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    protected override void RestartInternal()
    {
        this.moveType = MoveType.Drop;
        this.isReturnMoveType = false;
        this.selectedFruit = null;
        AddFruitsToStart();
        TurnStart();
    }

    protected override void LoadInternal(GameRepository.GameState state)
    {
        this.moveType = MoveType.Drop;
        this.isReturnMoveType = false;
        this.selectedFruit = null;
        AddFruitsToStart();
    }

    private void AddFruitsToStart()
    {
        this.AddFruitToStart(this.item1);
        this.AddFruitToStart(this.item2);
        this.AddFruitToStart(this.item3);
    }

    private void AddFruitToStart(Position2D position)
    {
        var fruit = CreateNewFruit(UsedColors[r.Next(UsedColors.Length)], null);
        fruit.Position = position.Position;
        fruit.AddToGroup(Groups.FruitsAtStart);
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

    protected override void FruitMovedInternal(List<Fruit> movedFruits)
    {
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

    protected override void FieldCellSelectedInternal(Vector2 cell)
    {
        var fruit = this.graph.Map[(int)cell.x, (int)cell.y];

        if (fruit != null)
        {
            this.selectedFruit?.DeselectFruit();
            this.selectedFruit = fruit;
            this.selectedFruit.SelectFruit();
            return;
        }

        if (this.selectedFruit == null)
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

    protected override bool IsGameOver()
    {
        return this.graph.Fruits.Count > Width * Height - 3;
    }
}
