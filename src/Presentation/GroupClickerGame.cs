using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotAnalysers;
using IsometricGame.Presentation.Utils;

[SceneReference("GroupClickerGame.tscn")]
public partial class GroupClickerGame
{
    protected override string GameName => nameof(GroupClickerGame);

    private readonly Fruit.FruitTypes[] UsedColors = new[] {
                Fruit.FruitTypes.Apple,
                Fruit.FruitTypes.Grape,
                Fruit.FruitTypes.Lemon,
                Fruit.FruitTypes.Pear,
            };

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    protected override void FieldCellSelectedInternal(Vector2 cell)
    {
        var fruit = this.graph.Map[(int)cell.x, (int)cell.y];
        if (fruit == null)
        {
            return;
        }

        if (fruit.IsInGroup(Groups.SelectedFriut))
        {
            RemoveFromGroup();
        }
        else
        {
            SelectNewGroup(cell);
        }
    }

    private void SelectNewGroup(Vector2 cell)
    {
        var fruits = this.GetTree().GetNodesInGroup(Groups.SelectedFriut);

        foreach (Fruit selected in fruits)
        {
            selected.RemoveFromGroup(Groups.SelectedFriut);
            selected.DeselectFruit();
        }

        var clickedGroup = this.graph.GetArea((int)cell.x, (int)cell.y);
        if (clickedGroup.Count < 2)
        {
            return;
        }

        foreach (var item in clickedGroup)
        {
            var itemFruit = this.graph.Map[(int)item.x, (int)item.y];
            itemFruit.AddToGroup(Groups.SelectedFriut);
            itemFruit.SelectFruit();
        }
    }

    private void RemoveFromGroup()
    {
        var fruits = this.GetTree().GetNodesInGroup(Groups.SelectedFriut);

        foreach (Fruit selected in fruits)
        {
            selected.Drop(this.basket.RectGlobalPosition + this.basket.RectSize / 2);
            selected.RemoveFromGroup(Groups.SelectedFriut);
            this.graph.RemoveFruit(selected);
        }

        var ballsFallX = 0;
        for (var x = 0; x < Width; x++)
        {
            var ballsFallY = 0;
            for (var y = Height - 1; y >= 0; y--)
            {
                if (this.graph.Map[x, y] == null)
                {
                    ballsFallY++;
                    continue;
                }

                if (ballsFallX == 0 && ballsFallY == 0)
                {
                    continue;
                }

                var fallTo = new Vector2(x + ballsFallX, y + ballsFallY);

                var fruit = this.graph.Map[x, y];
                this.graph.RemoveFruit(fruit);
                this.graph.AddFruit(fruit, fallTo);
                fruit.Drop(this.field.MapToWorld(fallTo));
            }

            if (ballsFallY == Height)
            {
                ballsFallX--;
            }
        }

        if (this.graph.Map.Cast<Fruit>().All(a => a?.FruitType != ((Fruit)fruits[0]).FruitType))
        {
            AchievementRepository.UnlockAchievement(Achievement.GroupRemoveAllType);
        }

        if (fruits.Count >= 10)
        {
            AchievementRepository.UnlockAchievement(Achievement.GroupRemove10);
        }

        this.CurrentScore += Enumerable.Range(1, fruits.Count).Sum();
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
    }

    protected override void RestartInternal()
    {
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

    protected override bool IsGameOver()
    {
        var clearedAll = true;

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                if (this.graph.Map[x, y] == null)
                {
                    continue;
                }

                clearedAll = false;

                if (this.graph.GetArea(x, y).Count >= 2)
                {
                    return false;
                }
            }

        if (clearedAll)
        {
            AchievementRepository.UnlockAchievement(Achievement.GroupClearAll);
            this.CurrentScore = (int)(this.CurrentScore * 1.5);
        }

        return true;
    }
}
