using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 9, "Rope Bridge")]
public class Day09 : IPuzzle<RopeCommand[]>
{
    public RopeCommand[] Parse(string inputText)
    {
        inputText = """
            R 5
            U 8
            L 8
            D 3
            R 17
            D 10
            L 25
            U 20
            """;

        List<RopeCommand> commands = new();
        var readOnlySpan = inputText.AsSpan();
        foreach (var line in readOnlySpan.EnumerateLines())
        {
            commands.Add(RopeCommand.Parse(line));
        }

        return commands.ToArray();
    }

    public string Part1(RopeCommand[] input)
    {
        var tail = (X: 0, Y: 0);
        var head = (X: 0, Y: 0);

        var tailVisited = new HashSet<(int X, int Y)>
        {
            tail,
        };

        foreach (var command in input)
        {
            for (var i = 0; i < command.Count; i++)
            {
                switch (command.Direction)
                {
                    case Direction.Up:
                        head.Y--;
                        break;
                    case Direction.Down:
                        head.Y++;
                        break;
                    case Direction.Left:
                        head.X--;
                        break;
                    case Direction.Right:
                        head.X++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var tailUpdate = CalculateTailVector(head, tail);
                tail.X += tailUpdate.X;
                tail.Y += tailUpdate.Y;
                tailVisited.Add(tail);
            }
        }

        return tailVisited.Count.ToString();
    }

    public string Part2(RopeCommand[] input)
    {
        var knots = new (int X, int Y)[9];

        var tailVisited = new HashSet<(int X, int Y)>
        {
            (0, 0),
        };

        foreach (var command in input)
        {
            for (var i = 0; i < command.Count; i++)
            {
                switch (command.Direction)
                {
                    case Direction.Up:
                        knots[0].Y--;
                        break;
                    case Direction.Down:
                        knots[0].Y++;
                        break;
                    case Direction.Left:
                        knots[0].X--;
                        break;
                    case Direction.Right:
                        knots[0].X++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                for (var knotI = 1; knotI < knots.Length; knotI++)
                {
                    var tailUpdate = CalculateTailVector(knots[knotI - 1], knots[knotI]);
                    knots[knotI].X += tailUpdate.X;
                    knots[knotI].Y += tailUpdate.Y;
                }

                tailVisited.Add(knots[8]);
            }
        }

        return tailVisited.Count.ToString();
    }

    private static void Print((int X, int Y) head, (int X, int Y) tail, HashSet<(int X, int Y)> visited)
    {
        // var minX = Math.Min(head.X, Math.Min(tail.X, visited.Min(v => v.X))) - 1;
        // var maxX = Math.Max(head.X, Math.Max(tail.X, visited.Max(v => v.X))) + 1;
        // var minY = Math.Min(head.Y, Math.Min(tail.Y)) + 1;
        // var maxY = Math.Max(head.Y, tail.Y) - 1;

        var minX = 0;
        var minY = -5;
        var maxX = 5;
        var maxY = 0;

        var sb = new StringBuilder();
        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                if (x == head.X && y == head.Y)
                {
                    sb.Append('H');
                }
                else if (x == tail.X && y == tail.Y)
                {
                    sb.Append('T');
                }
                else if (x == 0 && y == 0)
                {
                    sb.Append('s');
                }
                else if (visited.Contains((x, y)))
                {
                    sb.Append('#');
                }
                else
                {
                    sb.Append('.');
                }
            }

            sb.AppendLine();
        }

        sb.AppendLine("==========");

        Console.WriteLine(sb.ToString());
    }

    private static (int X, int Y) CalculateTailVector((int X, int Y) headPosition, (int X, int Y) tailPosition)
    {
        return (headPosition.X - tailPosition.X, headPosition.Y - tailPosition.Y) switch
        {
            (2, 1) => (1, 1),
            (2, -1) => (1, -1),
            (-2, 1) => (-1, 1),
            (-2, -1) => (-1, -1),

            (1, 2) => (1, 1),
            (-1, 2) => (-1, 1),
            (1, -2) => (1, -1),
            (-1, -2) => (-1, -1),

            (2, _) => (1, 0),
            (-2, _) => (-1, 0),

            (_, 2) => (0, 1),
            (_, -2) => (0, -1),

            (0, 0) => (0, 0),

            (1, _) => (0, 0),
            (-1, _) => (0, 0),
            (_, 1) => (0, 0),
            (_, -1) => (0, 0),

            _ => throw new ArgumentOutOfRangeException($"Invalid Tail vector? {headPosition} {tailPosition}"),
        };
    }
}

public readonly record struct RopeCommand(Direction Direction, int Count)
{
    public static RopeCommand Parse(ReadOnlySpan<char> input)
    {
        var direction = input[0] switch
        {
            'R' => Direction.Right,
            'L' => Direction.Left,
            'U' => Direction.Up,
            'D' => Direction.Down,
            _ => throw new ArgumentException($"Invalid direction: {input[0]}"),
        };

        var count = int.Parse(input[1..]);

        return new RopeCommand(direction, count);
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}