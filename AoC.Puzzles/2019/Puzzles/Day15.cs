using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using AoC.Puzzles._2019.Shared;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 15, "Repair drone")]
public class Day15 : IPuzzle<long[]>
{
    public long[] Parse(string inputText)
    {
        return inputText.Split(',').Select(long.Parse).ToArray();
    }

    public string Part1(long[] input)
    {
        var computer = new Computer(input);

        if (!PathFindToOxygen(computer, out _, out var finalStepCount, out _))
        {
            return "No valid path";
        }

        return finalStepCount.ToString();
    }

    public string Part2(long[] input)
    {
        var computer = new Computer(input);
        
        if (!PathFindToOxygen(computer, out _, out _, out _))
        {
            return "No valid path";
        }
        
        if (PathFindToOxygen(computer, out _, out var stepCount, out var map))
        {
            return $"Found oxygen again??\r\n{map}";
        }

        return (stepCount - 1).ToString();
    }

    private static bool PathFindToOxygen(Computer computer, [NotNullWhen(true)] out MovementCommand[]? path,
        out int stepCount, [NotNullWhen(true)] out string? map)
    {
        var visited = new HashSet<(int N, int E)>();

        var toVisitNext = new List<(MovementCommand[] Path, MovementCommand NewStop, int n, int e)>
        {
            (Array.Empty<MovementCommand>(), MovementCommand.North, 0, 0),
            (Array.Empty<MovementCommand>(), MovementCommand.South, 0, 0),
            (Array.Empty<MovementCommand>(), MovementCommand.West, 0, 0),
            (Array.Empty<MovementCommand>(), MovementCommand.East, 0, 0),
        };

        var debugLog = new List<(int X, int Y, char C)>();
        debugLog.Add((0, 0, 'S'));

        stepCount = 0;

        while (toVisitNext.Count != 0)
        {
            stepCount++;
            var toVisit = toVisitNext.ToArray();

            toVisitNext.Clear();

            foreach (var queueItem in toVisit)
            {
                var (existingPath, newStep, n, e) = queueItem;

                switch (newStep)
                {
                    case MovementCommand.North:
                        n++;
                        break;
                    case MovementCommand.South:
                        n--;
                        break;
                    case MovementCommand.West:
                        e--;
                        break;
                    case MovementCommand.East:
                        e++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!visited.Add((n, e)))
                {
                    continue;
                }

                foreach (var command in existingPath)
                {
                    computer.ContinueWithInput((long)command, out _);
                }

                computer.ContinueWithInput((long)newStep, out var output);

                switch ((MoveResponse)output[0])
                {
                    case MoveResponse.WallHit:
                        debugLog.Add((e, n, '#'));
                        break;
                    case MoveResponse.Success:
                        debugLog.Add((e, n, '.'));

                        computer.ContinueWithInput((long)Reverse(newStep), out _);

                        var newExistingPath = new MovementCommand[existingPath.Length + 1];
                        existingPath.CopyTo(newExistingPath, 0);
                        newExistingPath[^1] = newStep;

                        if (newStep != MovementCommand.South)
                            toVisitNext.Add((newExistingPath, MovementCommand.North, n, e));
                        if (newStep != MovementCommand.North)
                            toVisitNext.Add((newExistingPath, MovementCommand.South, n, e));
                        if (newStep != MovementCommand.East)
                            toVisitNext.Add((newExistingPath, MovementCommand.West, n, e));
                        if (newStep != MovementCommand.West)
                            toVisitNext.Add((newExistingPath, MovementCommand.East, n, e));

                        break;
                    case MoveResponse.Reached:
                    {
                        path = new MovementCommand[existingPath.Length + 1];
                        existingPath.CopyTo(path, 0);
                        path[^1] = newStep;

                        debugLog.Add((e, n, '!'));
                        map = Print(debugLog);

                        return true;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var step in Reverse(existingPath))
                {
                    computer.ContinueWithInput((long)step, out _);
                }
            }
        }

        path = default;
        map = null;
        return false;
    }

    private static MovementCommand Reverse(MovementCommand command) => command switch
    {
        MovementCommand.North => MovementCommand.South,
        MovementCommand.South => MovementCommand.North,
        MovementCommand.West => MovementCommand.East,
        MovementCommand.East => MovementCommand.West,
        _ => throw new ArgumentOutOfRangeException(nameof(command), command, null),
    };

    private static IEnumerable<MovementCommand> Reverse(IEnumerable<MovementCommand> commands)
    {
        return commands.Reverse().Select(Reverse);
    }

    private static string Print(IList<(int X, int Y, char C)> log)
    {
        var minX = log.Min(x => x.X);
        var maxX = log.Max(x => x.X);
        var minY = log.Min(x => x.Y);
        var maxY = log.Max(x => x.Y);

        var sb = new StringBuilder();
        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                var c = log.FirstOrDefault(l => l.X == x && l.Y == y).C;
                sb.Append(c == default ? ' ' : c);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public enum MovementCommand
{
    North = 1,
    South = 2,
    West = 3,
    East = 4,
}

public enum MoveResponse
{
    WallHit = 0,
    Success = 1,
    Reached = 2,
}