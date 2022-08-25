using Godot;

namespace IsometricGame.Presentation.Utils
{
    public class GameRepository
    {
        public struct GameState
        {
            public string GameName;
            public Fruit.FruitTypes?[,] Map;
            public int CurrentScore;
            public int BestScore;
            public int Multiplier;
        }

        public static void Save(GameState state)
        {
            var saveGame = new File();
            saveGame.Open($"user://Game{state.GameName}.save", File.ModeFlags.Write);
            saveGame.Store32((uint)state.CurrentScore);
            saveGame.Store32((uint)state.BestScore);
            saveGame.Store32((uint)state.Multiplier);
            saveGame.Store32((uint)state.Map.GetLength(0));
            saveGame.Store32((uint)state.Map.GetLength(1));
            for (var x = 0; x < state.Map.GetLength(0); x++)
                for (var y = 0; y < state.Map.GetLength(1); y++)
                {
                    saveGame.Store8((byte)(state.Map[x, y].HasValue ? 1 : 0));
                    if (state.Map[x, y].HasValue)
                    {
                        saveGame.Store32((uint)state.Map[x, y]);
                    }
                }

            saveGame.Close();

        }

        public static GameState Load(string gameName)
        {
            var result = new GameState{
                GameName = gameName
            };

            var saveGame = new File();
            if (!saveGame.FileExists($"user://Game{gameName}.save"))
                return result;

            saveGame.Open($"user://Game{gameName}.save", File.ModeFlags.Read);

            result.CurrentScore = (int)saveGame.Get32();
            result.BestScore = (int)saveGame.Get32();
            result.Multiplier = (int)saveGame.Get32();
            var width = (int)saveGame.Get32();
            var height = (int)saveGame.Get32();
            result.Map = new Fruit.FruitTypes?[width, height];

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    var hasValue = saveGame.Get8() == 1;
                    if (hasValue)
                    {
                        result.Map[x, y] = (Fruit.FruitTypes?)saveGame.Get32();
                    }
                }

            saveGame.Close();

            return result;
        }
    }
}
