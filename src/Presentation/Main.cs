using System;
using DodgeTheCreeps.Utils;
using Godot;
using GodotAnalysers;
using IsometricGame.Presentation.Utils;

[SceneReference("Main.tscn")]
public partial class Main
{
    private IGame[] availableGames = new IGame[]{
        new AntiLinesGame(),
        new LinesGame(),
        new FloodItGame()
    };

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.popupBackButton.Connect(CommonSignals.Pressed, this, nameof(ClosePopup));
        this.credentialsButton.Connect(CommonSignals.Pressed, this, nameof(ShowCredentials));
        this.helpButton.Connect(CommonSignals.Pressed, this, nameof(ShowHelp));


        var theme = ResourceLoader.Load<Theme>("res://Presentation/UITheme.tres");

        for (int i = 0; i < availableGames.Length; i++)
        {
            IGame game = availableGames[i];
            var button = new Button();
            button.Text = game.Name;
            button.Theme = theme;
            button.Disabled = !game.IsAvailable;
            button.Connect(CommonSignals.Pressed, this, nameof(StartGameDone), new Godot.Collections.Array { i });
            this.buttonsContainer.AddChild(button);
        }
    }

    private void ShowCredentials()
    {
        this.popupLabel.Text =  @"





Developer: Artem Votintcev
Design: Alexandra Votintceva 
Game Images: Loading.io
UI Images: Gameart2d.com
Idea: Color lines 1998




Created at 2017";
        this.popupDialog.ShowModal();
    }
    
    private void ShowHelp()
    {
        this.popupLabel.Text =  @"
  In 'Lines' game you need to assemble a row
with 5 fruits of the same type horizontally, 
vertically or diagonally to get scores. 
  Goal: maximize your score.
  In opposite 'Anti-lines' has different goal -
you need to fill the field with fruits, but
it is harder then you may think. Here each 
line with 3 fruits in a row disappear and
grant you scores. Unfortunately it is not 
what you need to do.
  Goal: minimize your score.";
        this.popupDialog.ShowModal();
    }

    private void ClosePopup()
    {
        this.popupDialog.Hide();
    }

    public void StartGameDone(int gameId)
    {
        var game = availableGames[gameId].BuildScreen();
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
