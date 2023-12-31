using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 17, "Clumsy Crucible")]
public class Day17 : IPuzzle<int[,]>
{
    public int[,] Parse(string rawInput)
    {
        var testRawInput = """
                           2413432311323
                           3215453535623
                           3255245654254
                           3446585845452
                           4546657867536
                           1438598798454
                           4457876987766
                           3637877979653
                           4654967986887
                           4564679986453
                           1224686865563
                           2546548887735
                           4322674655533
                           """;

        testRawInput = """
                       111111111111
                       999999999991
                       999999999991
                       999999999991
                       999999999991
                       """;
        
        // rawInput = testRawInput;

        
        return ParseMap(rawInput);
    }

    private static int[,] ParseMap(string rawInput)
    {
        var split = rawInput.Split(Environment.NewLine);

        var stuff = new int[split[0].Length, split.Length];

        for (var y = 0; y < split.Length; y++)
        {
            for (var x = 0; x < split[0].Length; x++)
            {
                stuff[x, y] = split[y][x] - '0';
            }
        }

        return stuff;
    }

    public string Part1(int[,] input)
    {
        return GetLowestEnergyLoss(input, PathCrucibleState, (x, _) => x.TotalHeatLoss).ToString();
    }

    public string Part2(int[,] input)
    {
        return GetLowestEnergyLoss(input, PathUltraCrucibleState, GetHeuristic).ToString();
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private record struct PathState(int X,
        int Y,
        Direction? Direction,
        int DirectionCount,
        int TotalHeatLoss);

    private static int GetLowestEnergyLoss(int[,] input,
        Func<PathState, int, Direction, Direction, Direction, IEnumerable<PathState>> calculateNextStates,
        Func<PathState,(int X, int Y),int> heuristic)
    {
        var cursors = new PriorityQueue<PathState, int>();

        var visited = new HashSet<(int X, int Y, Direction? Direction, int DirectionCount)>();

        cursors.Enqueue(new PathState(0, 0, null, 0, -input[0, 0]), 0);

        var target = (input.GetLength(0) - 1, input.GetLength(1) - 1);

        while (cursors.TryDequeue(out var cursor, out _))
        {
            if (!visited.Add((cursor.X, cursor.Y, cursor.Direction, cursor.DirectionCount)))
            {
                continue;
            }

            int current;
            try
            {
                current = input[cursor.X, cursor.Y];
            }
            catch
            {
                continue;
            }

            var newHeatLoss = cursor.TotalHeatLoss + current;

            if ((cursor.X, cursor.Y) == target)
            {
                return newHeatLoss;
            }

            var next = GoFurther(cursor, newHeatLoss, calculateNextStates)
                .Select(x => (x, heuristic(x, target)))
                .ToList();
            cursors.EnqueueRange(next);
        }

        throw new UnreachableException();
    }

    private static int GetHeuristic(PathState pathState, (int X, int Y) input)
    {
        var xHeuristic = input.X - pathState.X;
        var yHeuristic = input.Y - pathState.Y;

        if (xHeuristic is 0 && yHeuristic is 0)
        {
            if (pathState.DirectionCount < 4) return int.MaxValue;
        }

        return pathState.TotalHeatLoss; //+ ((xHeuristic + yHeuristic) * 1);
    }

    private static IEnumerable<PathState> GoFurther(PathState current,
        int totalHeatLoss,
        Func<PathState, int, Direction, Direction, Direction, IEnumerable<PathState>> calculateNextStates)
    {
        return current.Direction switch
        {
            Direction.Up => calculateNextStates(current, totalHeatLoss, Direction.Left, Direction.Up, Direction.Right),
            Direction.Down => calculateNextStates(current, totalHeatLoss, Direction.Right, Direction.Down, Direction.Left),
            Direction.Left => calculateNextStates(current, totalHeatLoss, Direction.Down, Direction.Left, Direction.Up),
            Direction.Right => calculateNextStates(current, totalHeatLoss, Direction.Up, Direction.Right, Direction.Down),
            null => calculateNextStates(current, totalHeatLoss, Direction.Left, Direction.Up, Direction.Right)
                .Concat(calculateNextStates(current, totalHeatLoss, Direction.Right, Direction.Down, Direction.Left))
                .Concat(calculateNextStates(current, totalHeatLoss, Direction.Down, Direction.Left, Direction.Up))
                .Concat(calculateNextStates(current, totalHeatLoss, Direction.Up, Direction.Right, Direction.Down))
                .Distinct(),
        };
    }

    private static IEnumerable<PathState> PathCrucibleState(PathState current,
        int totalHeatLoss,
        Direction leftTurn,
        Direction straight,
        Direction rightTurn)
    {
        if (current.DirectionCount is not 3)
        {
            var (x, y, dir) = IncrementPosition(current.X, current.Y, straight);
            yield return new PathState(x, y, dir, current.DirectionCount + 1, totalHeatLoss);
        }

        var (lX, lY, lDir) = IncrementPosition(current.X, current.Y, leftTurn);
        yield return new PathState(lX, lY, lDir, 1, totalHeatLoss);

        var (rX, rY, rDir) = IncrementPosition(current.X, current.Y, rightTurn);
        yield return new PathState(rX, rY, rDir, 1, totalHeatLoss);
    }

    private static IEnumerable<PathState> PathUltraCrucibleState(PathState current,
        int totalHeatLoss,
        Direction leftTurn,
        Direction straight,
        Direction rightTurn)
    {
        if (current.DirectionCount < 10)
        {
            var (x, y, dir) = IncrementPosition(current.X, current.Y, straight);
            yield return new PathState(x, y, dir, current.DirectionCount + 1, totalHeatLoss);

            if (current.DirectionCount < 4)
            {
                yield break;
            }
        }

        var (lX, lY, lDir) = IncrementPosition(current.X, current.Y, leftTurn);
        yield return new PathState(lX, lY, lDir, 1, totalHeatLoss);

        var (rX, rY, rDir) = IncrementPosition(current.X, current.Y, rightTurn);
        yield return new PathState(rX, rY, rDir, 1, totalHeatLoss);
    }

    private static (int X, int Y, Direction Direction) IncrementPosition(int x, int y, Direction direction)
    {
        var nextX = direction switch
        {
            Direction.Left => x - 1,
            Direction.Right => x + 1,
            _ => x,
        };
        var nextY = direction switch
        {
            Direction.Up => y - 1,
            Direction.Down => y + 1,
            _ => y,
        };

        return (nextX, nextY, direction);
    }
}