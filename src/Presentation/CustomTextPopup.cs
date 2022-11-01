using Godot;
using System;

using GodotAnalysers;
using IsometricGame.Presentation.Utils;

[SceneReference("CustomTextPopup.tscn")]
[Tool]
public partial class CustomTextPopup
{
    [Export(PropertyHint.MultilineText)] 
    public string Text { get; set; }

    [Signal]
    public delegate void PopupClosed();

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.popupBackButton.Connect(CommonSignals.Pressed, this, nameof(BackButtonPressed));
    }

    private void BackButtonPressed()
    {
        this.Hide();
        this.EmitSignal(nameof(PopupClosed));
    }

    public override void _Process(float delta)
    {
        this.popupLabel.Text = this.Text;
    }

}
