using Godot;

namespace IsometricGame.Presentation.Utils
{
    public class FloodItGame : IGame
    {
        public string Name => "Flood It";

        public bool IsAvailable => true;

        public Node BuildScreen()
        {
            var scene = ResourceLoader.Load<PackedScene>("res://Presentation/FloodIt.tscn");
            var result = scene.Instance<FloodIt>();
            result.UsedColors = new[] {
                Fruit.FruitTypes.Apple,
                Fruit.FruitTypes.Banana,
                Fruit.FruitTypes.Cherry,
                Fruit.FruitTypes.Grape,
                Fruit.FruitTypes.Lemon,
                Fruit.FruitTypes.Pear,
                Fruit.FruitTypes.Pineaple
            };

            return result;
        }
    }
}
