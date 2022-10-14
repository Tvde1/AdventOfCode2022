using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using AoC.Puzzles._2019.Shared;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 11, "Painting robot")]
public class Day11 : IPuzzle<long[]>
{
    public long[] Parse(string inputText)
    {
        return inputText.Split(',').Select(long.Parse).ToArray();
    }

    public string Part1(long[] input)
    {
        var computer = new Computer(input);

        var canvas = new Dictionary<(int X, int Y), bool>();
   
        DoRobotStuff(computer, canvas);

        return canvas.Count.ToString();
    }

    public string Part2(long[] input)
    {
        var computer = new Computer(input);

        var canvas = new Dictionary<(int X, int Y), bool>
        {
            [(0, 0)] = true
        };

        DoRobotStuff(computer, canvas);

        return $"{PrintMessage(canvas)} ({canvas.Count})";
    }

    private static void DoRobotStuff(Computer computer, Dictionary<(int X, int Y), bool> canvas)
    {

        var currentX = 0;
        var currentY = 0;

        var direction = Direction.Up;

        while (true)
        {
            var currentTileIsWhite = canvas.TryGetValue((currentX, currentY), out var r) && r
                ? 1
                : 0;

            var output = computer.ContinueWithInput(currentTileIsWhite);

            canvas[(currentX, currentY)] = output[0].Value == 1;

            if (output.Count > 2 && output[2].IsExit)
            {
                break;
            }

            direction = Calculate(direction, output[1].Value == 0);

            switch (direction)
            {
                case Direction.Up:
                    currentY--;
                    break;
                case Direction.Right:
                    currentX++;
                    break;
                case Direction.Down:
                    currentY++;
                    break;
                case Direction.Left:
                    currentX--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private static Direction Calculate(Direction direction, bool goLeft)
    {
        return (direction, goLeft) switch
        {
            (Direction.Up, true) => Direction.Left,
            (Direction.Up, false) => Direction.Right,
            (Direction.Right, true) => Direction.Up,
            (Direction.Right, false) => Direction.Down,
            (Direction.Left, true) => Direction.Down,
            (Direction.Left, false) => Direction.Up,
            (Direction.Down, true) => Direction.Right,
            (Direction.Down, false) => Direction.Left,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static string PrintMessage(Dictionary<(int X, int Y), bool> canvas)
    {
        var minX = canvas.Where(x => x.Value)
            .Min(x => x.Key.X);
        var minY = canvas.Where(x => x.Value)
            .Min(x => x.Key.Y);
        var maxX = canvas.Where(x => x.Value)
            .Max(x => x.Key.X);
        var maxY = canvas.Where(x => x.Value)
            .Max(x => x.Key.Y);

        var sb = new StringBuilder();

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                var isPainted = canvas.TryGetValue((x, y), out var isPaintedRes) && isPaintedRes;
                sb.Append(isPainted ? '#' : '.');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
}