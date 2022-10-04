using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 3, "Wires that meet")]
public class Day03 : IPuzzle<WireOperation[][]>
{
    public WireOperation[][] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine).Select(l => l.Split(',').Select(WireOperation.Parse).ToArray()).ToArray();
    }

    public string Part1(WireOperation[][] input)
    {
        var coordinatesHitBy1 = new HashSet<(int X, int Y)>();
        {
            var x1 = 0;
            var y1 = 0;
            foreach (var movesBy1 in input[0])
            {
                for (var i = 0; i < movesBy1.Count; i++)
                {
                    switch (movesBy1.Direction)
                    {
                        case WireDirection.Up:
                            {
                                y1++;
                            }
                            break;
                        case WireDirection.Right:
                            {
                                x1++;
                            }
                            break;
                        case WireDirection.Down:
                            {
                                y1--;
                            }
                            break;
                        case WireDirection.Left:
                            {
                                x1--;
                            }
                            break;
                    }
                    coordinatesHitBy1.Add((x1, y1));
                }
            }
        }

        (int X, int Y) lowestHit = default;
        int manhattanDist = int.MaxValue;

        {
            var x2 = 0;
            var y2 = 0;
            foreach (var movesBy2 in input[1])
            {
                for (var i = 0; i < movesBy2.Count; i++)
                {
                    switch (movesBy2.Direction)
                    {
                        case WireDirection.Up:
                            {
                                y2++;
                            }
                            break;
                        case WireDirection.Right:
                            {
                                x2++;
                            }
                            break;
                        case WireDirection.Down:
                            {
                                y2--;
                            }
                            break;
                        case WireDirection.Left:
                            {
                                x2--;
                            }
                            break;
                    }
                    Calc(coordinatesHitBy1, ref lowestHit, ref manhattanDist, x2, y2);
                }
            }
        }

        return $"Lowest pair: {manhattanDist}: ({lowestHit.X}, {lowestHit.Y})";

        static void Calc(HashSet<(int X, int Y)> coordinatesHitBy1, ref (int X, int Y) lowestHit, ref int manhattanDist, int x2, int y2)
        {
            var pair = (x2, y2);
            if (coordinatesHitBy1.Contains(pair))
            {
                var dist = Math.Abs(x2) + Math.Abs(y2);
                if (dist < manhattanDist)
                {
                    manhattanDist = dist;
                    lowestHit = pair;
                }
            }
        }
    }

    public string Part2(WireOperation[][] input)
    {
        var location = new (int X, int Y)[] {
            (0, 0),
            (0, 0)
        };

        var count = new[]
        {
            0,
            0
        };

        var visitedBy = new[] {
            new Dictionary<(int X, int Y), int>(),
            new Dictionary<(int X, int Y), int>()
        };

        int operation = 0;
        int whichToStep = 1;
        while (true)
        {
            whichToStep = (whichToStep + 1) % 2;

            var currentOperation = input[whichToStep][operation];

            for (var i = 0; i < currentOperation.Count; i++)
            {
                switch (currentOperation.Direction)
                {
                    case WireDirection.Up:
                        {
                            location[whichToStep].Y++;
                        }
                        break;
                    case WireDirection.Right:
                        {
                            location[whichToStep].X++;
                        }
                        break;
                    case WireDirection.Down:
                        {
                            location[whichToStep].Y--;
                        }
                        break;
                    case WireDirection.Left:
                        {
                            location[whichToStep].X--;
                        }
                        break;
                }

                count[whichToStep]++;

                if (!visitedBy[whichToStep].ContainsKey((location[whichToStep].X, location[whichToStep].Y)))
                {
                    visitedBy[whichToStep].Add((location[whichToStep].X, location[whichToStep].Y), count[whichToStep]);
                }

                if (visitedBy[(whichToStep + 1) % 2].TryGetValue((location[whichToStep].X, location[whichToStep].Y), out var meetValue))
                {
                    return (count[whichToStep] + meetValue).ToString();
                }
            }

            operation += whichToStep;
        }
    }
}

public enum WireDirection : byte
{
    Up = 85,
    Right = 82,
    Down = 68,
    Left = 76,
}

public readonly record struct WireOperation(WireDirection Direction, int Count)
{
    public static WireOperation Parse(string input)
    {
        var direction = (WireDirection)input[0];
        var count = int.Parse(input[1..]);
        return new(direction, count);
    }
}