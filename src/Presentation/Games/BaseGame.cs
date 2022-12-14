using Godot;
using GodotAnalysers;
using System;
using System.Linq;
using Antilines.Logic.ScriptHelpers;
using System.Collections.Generic;
using Antilines.Presentation.Utils;

[SceneReference("BaseGame.tscn")]
public abstract partial class BaseGame
{
    protected abstract string GameName { get; }

    public const int Width = 9;
    public const int Height = 9;

    protected Random r = new Random();
    protected MapGraphData graph = new MapGraphData(Width, Height);

    protected int CurrentScore
    {
        get => this.currentScoreLabel.Value;
        set
        {
            this.currentScoreLabel.Value = value;
        }
    }

    private int bestScore = 0;
    protected int BestScore
    {
        get => this.bestScore;
        set
        {
            this.bestScore = value;
            this.bestScoreLabel.Text = $"{value}";
        }
    }

    private int multiplier = 1;
    protected int Multiplier
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
        this.gameOverPopup.Connect(nameof(CustomTextPopup.PopupClosed), this, nameof(Restart));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        this.hUD.Visible = this.Visible;
    }

    public void Start()
    {
        var state = GameRepository.Load(this.GameName);
        if (state.Map == null)
        {
            Restart();
        }
        else
        {
            Load(state);
        }
    }

    private void Back()
    {
        this.EmitSignal(nameof(Close));
        this.SaveState();
    }

    protected abstract void RestartInternal();

    private void Restart()
    {
        this.CurrentScore = 0;
        this.Multiplier = 1;
        this.graph.ClearMap();
        foreach (Fruit fruit in this.GetTree().GetNodesInGroup(Groups.Fruits))
        {
            fruit.QueueFree();
            this.RemoveChild(fruit);
        }

        this.RestartInternal();
        this.SaveState();
    }

    protected abstract void LoadInternal(GameRepository.GameState state);

    private void Load(GameRepository.GameState state)
    {
        this.CurrentScore = state.CurrentScore;
        this.BestScore = state.BestScore;
        this.Multiplier = state.Multiplier;
        this.graph.ClearMap();
        foreach (Fruit fruit in this.GetTree().GetNodesInGroup(Groups.Fruits))
        {
            fruit.QueueFree();
            this.RemoveChild(fruit);
        }

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                if (!state.Map[x, y].HasValue)
                {
                    continue;
                }

                var position = new Vector2(x, y);
                var fruit = CreateNewFruit(state.Map[x, y].Value, position);
                fruit.Position = this.field.MapToWorld(position) + Vector2.Up * 300;
                fruit.Drop(this.field.MapToWorld(position));
            }

        LoadInternal(state);
    }

    protected abstract void FruitMovedInternal(List<Fruit> movedFruits);
    protected void FruitMoved(Fruit fruit)
    {
        fruit.AddToGroup(Groups.FruitsMoved);
        if (this.GetTree().GetNodesInGroup(Groups.FruitsMoving).Count != 0)
        {
            return;
        }

        var movedFruits = this.GetTree().GetNodesInGroup(Groups.FruitsMoved).Cast<Fruit>().ToList();
        foreach (var movedFruit in movedFruits)
        {
            movedFruit.RemoveFromGroup(Groups.FruitsMoved);
        }

        if (this.CheckGameOver())
        {
            this.ClearState();
            return;
        }

        FruitMovedInternal(movedFruits);
        this.SaveState();
    }

    protected abstract void FieldCellSelectedInternal(Vector2 cell);

    private void FieldCellSelected(Vector2 cell)
    {
        if (this.GetTree().GetNodesInGroup(Groups.FruitsMoving).Count > 0)
        {
            return;
        }

        FieldCellSelectedInternal(cell);
    }

    protected abstract bool IsGameOver();
    protected abstract int GetBestScoreInternal(int bestScore, int currentScore);

    public bool CheckGameOver()
    {
        var isGameOver = this.IsGameOver();

        if (!isGameOver)
        {
            return isGameOver;

        }

        this.gameOverPopup.Show();
        this.gameOverPopup.Text = $@"
               Game over
        
            
           your score is {this.CurrentScore}";

        this.BestScore = GetBestScoreInternal(this.BestScore, this.CurrentScore);

        return isGameOver;
    }

    protected Fruit CreateNewFruit(Fruit.FruitTypes fruitType, Vector2? cellPosition)
    {
        var fruit = this.FruitScene.Instance<Fruit>();
        fruit.FruitType = fruitType;
        fruit.AddToGroup(Groups.Fruits);
        fruit.Connect(nameof(Fruit.FruitMoved), this, nameof(FruitMoved));
        this.AddChild(fruit);
        if (cellPosition != null)
        {
            fruit.Position = this.field.MapToWorld(cellPosition.Value);
            this.graph.AddFruit(fruit, cellPosition.Value);
        }
        return fruit;
    }

    protected void SaveState()
    {
        var types = new Fruit.FruitTypes?[Width, Height];
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                types[x, y] = this.graph.Map[x, y]?.FruitType;
            }
        GameRepository.Save(new GameRepository.GameState
        {
            GameName = this.GameName,
            Map = types,
            CurrentScore = this.CurrentScore,
            BestScore = this.BestScore,
            Multiplier = this.Multiplier
        });
    }

    protected void ClearState()
    {
        GameRepository.Save(new GameRepository.GameState
        {
            GameName = this.GameName,
            Map = null,
            CurrentScore = this.CurrentScore,
            BestScore = this.BestScore,
            Multiplier = this.Multiplier
        });
    }
}
