using System;
using Godot;
using GodotAnalysers;
using Antilines.Presentation.Utils;

[SceneReference("Main.tscn")]
public partial class Main
{
    private Games[] availableGames = new Games[]{
        Games.Lines,
        Games.AntiLines,
        Games.FloodIt,
        Games.GroupClicker,
        Games.Life,
    };

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.credentialsButton.Connect(CommonSignals.Pressed, this, nameof(ShowCredentials));
        this.helpButton.Connect(CommonSignals.Pressed, this, nameof(ShowHelp));
        this.achievementsButton.Connect(CommonSignals.Pressed, this, nameof(ShowAchievements));

        var theme = ResourceLoader.Load<Theme>("res://Presentation/UITheme.tres");

        for (int i = 0; i < availableGames.Length; i++)
        {
            Games game = availableGames[i];
            var button = new Button();
            button.Text = game.ToString();
            button.Theme = theme;
            button.Connect(CommonSignals.Pressed, this, nameof(StartGameDone), new Godot.Collections.Array { i });
            this.buttonsContainer.AddChild(button);
        }
    }

    private void ShowCredentials()
    {
        this.credentialsPopup.Show();
    }

    private void ShowHelp()
    {
        this.helpPopup.Show();
    }

    private void ShowAchievements()
    {
        this.customAchievementsPopup.Show();
        this.customAchievementsPopup.ReloadList();
    }

    public void StartGameDone(int gameId)
    {
        var scene = ResourceLoader.Load<PackedScene>($"res://Presentation/Games/{availableGames[gameId].ToString()}Game.tscn");
        var game = scene.Instance();
        game.AddToGroup(Groups.Game);
        game.Connect("Close", this, nameof(GameOver));
        this.AddChild(game);
        game.Call("Start");
        this.hUD.Visible = false;
    }

    public void GameOver()
    {
        var games = this.GetTree().GetNodesInGroup(Groups.Game);
        foreach (Node2D game in games)
        {
            game.QueueFree();
        }

        this.hUD.Visible = true;
    }
}
