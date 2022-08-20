using Godot;

namespace IsometricGame.Presentation.Utils
{
    public class AntiLinesGame : IGame
    {
        public string Name => "Anti Lines";

        public bool IsAvailable => true;

        public Node BuildScreen()
        {
            var scene = ResourceLoader.Load<PackedScene>("res://Presentation/BaseLinesGame.tscn");
            var result = scene.Instance<BaseLinesGame>();
            result.UsedColors = new[] {
                Fruit.FruitTypes.Apple,
                Fruit.FruitTypes.Grape,
                Fruit.FruitTypes.Lemon,
                Fruit.FruitTypes.Pear
            };
            result.LineLength = 3;
            result.IncreaseMultiplier = 0;

            return result;
        }
    }
}
