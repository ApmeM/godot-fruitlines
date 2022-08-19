using Godot;
using GodotAnalysers;

[SceneReference("Field.tscn")]
public partial class Field
{
    [Signal]
    public delegate void CellSelected();

    public override void _Ready()
    {
        base._Ready();
        this.FillMembers();
    }

    public Vector2 MapToWorld(Vector2 mapPos)
    {
        var localPosition = this.tileMap.MapToWorld(mapPos) + this.tileMap.CellSize / 2;
        return this.tileMap.ToGlobal(localPosition);
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        base._UnhandledInput(inputEvent);
        if (!IsVisibleInTree())
        {
            return;
        }

        if (!(inputEvent is InputEventScreenTouch mouseEvent) || !mouseEvent.Pressed)
        {
            return;
        }

        var position = this.GetGlobalMousePosition(); // eventMouseButton.Position;
        var localPosition = this.tileMap.ToLocal(position);
        var cell = this.tileMap.WorldToMap(localPosition);

        EmitSignal(nameof(CellSelected), cell);
    }
}
