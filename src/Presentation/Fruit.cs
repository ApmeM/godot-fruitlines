using Godot;
using GodotAnalysers;
using IsometricGame.Presentation.Utils;
using System.Collections.Generic;

[Tool]
[SceneReference("Fruit.tscn")]
public partial class Fruit
{
    public enum FruitTypes
    {
        Apple,
        Banana,
        Cherry,
        Grape,
        Lemon,
        Pear,
        Pineaple
    }

    private Queue<Vector2> currentPath = new Queue<Vector2>();

    [Export]
    public int MoveSpeed = 400;
    [Export]
    public int DropSpeed = 800;

    private int currentSpeed = 0;

    private FruitTypes fruitType;

    private bool fruitTypeDirty;

    [Export]
    public FruitTypes FruitType
    {
        get => fruitType;
        set
        {
            fruitType = value;
            fruitTypeDirty = true;
        }
    }

    [Signal]
    public delegate void FruitMoved(Fruit fruit);

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (this.fruitTypeDirty)
        {
            this.fruitTypeDirty = false;
            this.animatedSprite.Animation = this.fruitType.ToString().ToLower();
        }

        if (currentPath.Count > 0)
        {
            var currentPosition = this.Position;
            var currentTarget = currentPath.Peek();
            var direction = (currentTarget - currentPosition);
            if (direction.LengthSquared() < 4)
            {
                currentPath.Dequeue();
                if (currentPath.Count == 0)
                {
                    this.animatedSprite.Stop();
                    this.animatedSprite.Frame = 0;
                    this.RemoveFromGroup(Groups.FruitsMoving);
                    this.EmitSignal(nameof(FruitMoved), this);
                }
            }
            else
            {
                var motion = direction / delta;
                if (motion.Length() > currentSpeed)
                {
                    motion = motion.Normalized() * currentSpeed;
                }

                this.Position += motion * delta;
            }
        }
    }

    public void Drop(params Vector2[] worldMovePath)
    {
        MoveTo(worldMovePath);
        this.currentSpeed = this.DropSpeed;
        this.AddToGroup(Groups.FruitsMoving);
    }

    public void Turn(params Vector2[] worldMovePath)
    {
        MoveTo(worldMovePath);
        this.currentSpeed = this.MoveSpeed;
        this.animatedSprite.Play();
        this.AddToGroup(Groups.FruitsMoving);
    }

    private void MoveTo(params Vector2[] worldMovePath)
    {
        if (currentPath.Count > 0)
        {
            var current = currentPath.Peek();
            currentPath.Clear();
            currentPath.Enqueue(current);
        }

        for (var i = 0; i < worldMovePath.Length; i++)
        {
            currentPath.Enqueue(worldMovePath[i]);
        }
    }

    public void SelectFruit()
    {
        this.animatedSprite.Play();
    }

    public void DeselectFruit()
    {
        this.animatedSprite.Stop();
        this.animatedSprite.Frame = 0;
    }
}
