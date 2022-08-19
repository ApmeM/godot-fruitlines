using Godot;

namespace IsometricGame.Presentation.Utils
{
    public interface IGame
    {
        string Name { get; }
        Node BuildScreen();
        bool IsAvailable { get; }
    }
}
