using Godot;
using System;

using GodotAnalysers;
using DodgeTheCreeps.Utils;

[SceneReference("CustomTextPopup.tscn")]
[Tool]
public partial class CustomTextPopup
{
    [Export(PropertyHint.MultilineText)] 
    public string Text { get; set; }

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();

        this.popupBackButton.Connect(CommonSignals.Pressed, this, nameof(BackButtonPressed));
    }

    private void BackButtonPressed()
    {
        this.Hide();
    }

    public override void _Process(float delta)
    {
        this.popupLabel.Text = this.Text;
    }

}
