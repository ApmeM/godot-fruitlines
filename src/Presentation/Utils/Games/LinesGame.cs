using Godot;

namespace IsometricGame.Presentation.Utils
{
    public class LinesGame : IGame
    {
        public string Name => "Lines";

        public bool IsAvailable => true;

        public Node BuildScreen()
        {
            var scene = ResourceLoader.Load<PackedScene>("res://Presentation/BaseLinesGame.tscn");
            var result = scene.Instance<BaseLinesGame>();
            result.UsedColors = new[] {
                Fruit.FruitTypes.Apple,
                Fruit.FruitTypes.Banana,
                Fruit.FruitTypes.Cherry,
                Fruit.FruitTypes.Grape,
                Fruit.FruitTypes.Lemon,
                Fruit.FruitTypes.Pear,
                Fruit.FruitTypes.Pineaple
            };

            result.LineLength = 5;
            result.IncreaseMultiplier = 1;

            return result;
        }
    }
}
