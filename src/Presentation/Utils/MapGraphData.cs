using BrainAI.Pathfinding.AStar;
using Godot;
using System;
using System.Collections.Generic;

namespace Antilines.Logic.ScriptHelpers
{
    public class MapGraphData : IAstarGraph<Vector2>
    {
        public MapGraphData(int width, int height)
        {
            Map = new Fruit[width, height];
            Fruits = new Dictionary<Fruit, Vector2>();
        }

        private static readonly Vector2[] CardinalDirs = {
            new Vector2( 1, 0 ),
            new Vector2( 0, -1 ),
            new Vector2( -1, 0 ),
            new Vector2( 0, 1 ),
        };

        private static readonly Vector2[] DirsToFindLines = {
            new Vector2( 1, 0 ),
            new Vector2( 0, 1 ),
            new Vector2( 1, 1 ),
            new Vector2( 1, -1 ),
        };

        public Fruit[,] Map;
        public Dictionary<Fruit, Vector2> Fruits;

        private readonly List<Vector2> neighbors = new List<Vector2>(4);

        public bool IsNodeInBounds(Vector2 node)
        {
            return 0 <= node.x && node.x < this.Map.GetLength(0) && 0 <= node.y && node.y < this.Map.GetLength(1);
        }

        public bool IsNodePassable(Vector2 node)
        {
            return this.Map[(int)node.x, (int)node.y] == null;
        }

        public IEnumerable<Vector2> GetNeighbors(Vector2 node)
        {
            this.neighbors.Clear();

            foreach (var dir in CardinalDirs)
            {
                var next = new Vector2(node.x + dir.x, node.y + dir.y);
                if (this.IsNodeInBounds(next) && this.IsNodePassable(next))
                    this.neighbors.Add(next);
            }

            return this.neighbors;
        }

        public int Cost(Vector2 from, Vector2 to)
        {
            return 1;
        }

        public int Heuristic(Vector2 node, Vector2 goal)
        {
            return (int)Math.Abs(node.x - goal.x) + (int)Math.Abs(node.y - goal.y);
        }

        public void AddFruit(Fruit fruit, Vector2 newPos)
        {
            System.Diagnostics.Debug.Assert(!this.Fruits.ContainsKey(fruit));
            System.Diagnostics.Debug.Assert(this.Map[(int)newPos.x, (int)newPos.y] == null);

            this.Fruits[fruit] = newPos;
            this.Map[(int)newPos.x, (int)newPos.y] = fruit;
        }

        public void MoveFruit(Fruit fruit, Vector2 newPos)
        {
            System.Diagnostics.Debug.Assert(this.Fruits.ContainsKey(fruit));
            System.Diagnostics.Debug.Assert(this.Map[(int)newPos.x, (int)newPos.y] == null);

            var prevPos = this.Fruits[fruit];
            this.Fruits[fruit] = newPos;

            this.Map[(int)prevPos.x, (int)prevPos.y] = null;
            this.Map[(int)newPos.x, (int)newPos.y] = fruit;
        }

        public void RemoveFruit(Fruit fruit)
        {
            System.Diagnostics.Debug.Assert(this.Fruits.ContainsKey(fruit));

            var prevPos = this.Fruits[fruit];
            this.Fruits.Remove(fruit);
            this.Map[(int)prevPos.x, (int)prevPos.y] = null;
        }


        public void ClearMap()
        {
            for (var x = 0; x < this.Map.GetLength(0); x++)
                for (var y = 0; y < this.Map.GetLength(1); y++)
                    this.Map[x, y] = null;
            this.Fruits.Clear();
        }

        public List<Fruit> FindLines(Fruit fruit, int lineLength)
        {
            var ballsToRemove = new List<Fruit>();
            if (!this.Fruits.ContainsKey(fruit))
            {
                return ballsToRemove;
            }

            var typeToCheck = fruit.FruitType;
            Vector2 p;

            foreach (var dir in DirsToFindLines)
            {
                p = this.Fruits[fruit];
                ballsToRemove.Clear();
                do
                {
                    p = p + dir;
                }
                while (IsNodeInBounds(p) && this.Map[(int)p.x, (int)p.y]?.FruitType == typeToCheck);
                p = p - dir;
                do
                {
                    ballsToRemove.Add(this.Map[(int)p.x, (int)p.y]);
                    p = p - dir;
                }
                while (IsNodeInBounds(p) && this.Map[(int)p.x, (int)p.y]?.FruitType == typeToCheck);

                if (ballsToRemove.Count >= lineLength)
                {
                    return ballsToRemove;
                }
            }

            ballsToRemove.Clear();
            return ballsToRemove;
        }


        public HashSet<Vector2> GetArea(int x, int y)
        {
            var color = this.Map[x, y].FruitType;

            var frontier = new Queue<Vector2>();
            frontier.Enqueue(new Vector2(x, y));

            var visited = new HashSet<Vector2>();
            visited.Add(new Vector2(x,y));

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                foreach (var next in this.GetNeighborsFloodIt(current))
                {
                    if (!this.IsNodeInBounds(next))
                    {
                        continue;
                    }

                    if (Map[(int)next.x, (int)next.y] == null)
                    {
                        continue;
                    }

                    if (Map[(int)next.x, (int)next.y].FruitType != color)
                    {
                        continue;
                    }

                    if (!visited.Contains(next))
                    {
                        frontier.Enqueue(next);
                        visited.Add(next);
                    }
                }
            }
            
            return visited;
        }

        private IEnumerable<Vector2> GetNeighborsFloodIt(Vector2 node)
        {
            this.neighbors.Clear();

            foreach (var dir in CardinalDirs)
            {
                var next = new Vector2(node.x + dir.x, node.y + dir.y);
                if (this.IsNodeInBounds(next))
                    this.neighbors.Add(next);
            }

            return this.neighbors;
        }

    }
}
