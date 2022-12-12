using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 12, "Hill Climbing Algorithm")]
public class Day12 : IPuzzle<char[,]>
{
    public char[,] Parse(string inputText)
    {
        var span = inputText.AsSpan();

        var width = span.IndexOf(Environment.NewLine, StringComparison.Ordinal);
        var height = (span.Length / (width + 1));

        var arr = new char[width, height];

        var y = 0;
        foreach (var line in span.EnumerateLines())
        {
            var x = 0;
            foreach (var rune in line.EnumerateRunes())
            {
                arr[x, y] = (char)rune.Value;
                x++;
            }

            y++;
        }

        return arr;
    }

    public string Part1(char[,] input)
    {
        var startingPosition = FindPosition(input, 'S');
        var endPosition = FindPosition(input, 'E');

        return PathfindThing(input, startingPosition, endPosition, out var pathLength)
            ? pathLength.ToString()
            : "No valid path.";
    }

    public string Part2(char[,] input)
    {
        var endPosition = FindPosition(input, 'E');

        var starts = new List<(int X, int Y)>();

        for (var x = 0; x < input.GetLength(0); x++)
        {
            for (var y = 0; y < input.GetLength(1); y++)
            {
                if (input[x, y] is 'a' or 'S')
                {
                    starts.Add((x, y));
                }
            }
        }

        return starts
            .Select(x => PathfindThing(input, x, endPosition, out var s) ? s : -1)
            .Where(x => x > 0)
            .Min()
            .ToString();
    }

    private bool PathfindThing(char[,] input,
        (int X, int Y) startingPosition,
        (int X, int Y) endPosition,
        out int shortestCount)
    {
        var toVisit = new PriorityQueue<((int X, int Y) Point, char PreviousHeight), int>();
        var visited = new HashSet<(int X, int Y)>();

        toVisit.Enqueue(((startingPosition), 'S'), 0);

        while (toVisit.TryDequeue(out var currentStep, out var currentStepCount))
        {
            if (currentStep.PreviousHeight == 'z' && currentStep.Point == endPosition)
            {
                shortestCount = currentStepCount;
                return true;
            }

            var currentHeight = input[currentStep.Point.X, currentStep.Point.Y];

            if (currentHeight == 'E')
            {
                continue;
            }

            if (char.IsLower(currentStep.PreviousHeight) && currentHeight - 1 > currentStep.PreviousHeight)
            {
                continue;
            }

            if (!visited.Add(currentStep.Point))
            {
                continue;
            }

            var newStepCount = currentStepCount + 1;

            if (currentStep.Point.X > 0)
            {
                var newPoint = currentStep.Point with
                {
                    X = currentStep.Point.X - 1,
                };
                toVisit.Enqueue((newPoint, currentHeight), newStepCount);
            }

            if (currentStep.Point.X < input.GetLength(0) - 1)
            {
                var newPoint = currentStep.Point with
                {
                    X = currentStep.Point.X + 1,
                };
                toVisit.Enqueue((newPoint, currentHeight), newStepCount);
            }

            if (currentStep.Point.Y > 0)
            {
                var newPoint = currentStep.Point with
                {
                    Y = currentStep.Point.Y - 1,
                };
                toVisit.Enqueue((newPoint, currentHeight), newStepCount);
            }

            if (currentStep.Point.Y < input.GetLength(1) - 1)
            {
                var newPoint = currentStep.Point with
                {
                    Y = currentStep.Point.Y + 1,
                };
                toVisit.Enqueue((newPoint, currentHeight), newStepCount);
            }
        }

        shortestCount = -1;
        return false;
    }

    private static (int X, int Y) FindPosition(char[,] input, char c)
    {
        for (var x = 0; x < input.GetLength(0); x++)
        {
            for (var y = 0; y < input.GetLength(1); y++)
            {
                if (input[x, y] == c)
                {
                    return (x, y);
                }
            }
        }

        throw new ArgumentOutOfRangeException($"No point with char '{c}' found.");
    }
}