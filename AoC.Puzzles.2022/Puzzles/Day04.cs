using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 4, "Camp cleanup")]
public class Day04 : IPuzzle<CleanupRequest[]>
{
    public CleanupRequest[] Parse(string inputText)
    {
        var requests = new List<CleanupRequest>();
        var enumerator = inputText.AsSpan().EnumerateLines();

        while (enumerator.MoveNext())
        {
            requests.Add(CleanupRequest.Parse(enumerator.Current));
        }

        return requests.ToArray();
    }

    public string Part1(CleanupRequest[] input)
    {
        var count = 0;
        foreach (var request in input)
        {
            if (IsInside(request.First, request.Second)) count++;
        }

        return count.ToString();
    }

    public string Part2(CleanupRequest[] input)
    {
        var count = 0;
        foreach (var request in input)
        {
            if (IsOverlapping(request.First, request.Second)) count++;
        }

        return count.ToString();
    }

    private static bool IsInside((int Start, int End) left, (int Start, int End) right)
    {
        return left.Start >= right.Start && left.End <= right.End ||
            right.Start >= left.Start && right.End <= left.End;
    }

    private static bool IsOverlapping((int Start, int End) left, (int Start, int End) right)
    {
        return left.Start <= right.Start && left.End >= right.Start ||
            right.Start <= left.Start && right.End >= left.Start;
    }
}

public readonly record struct CleanupRequest((int Start, int End) First, (int Start, int End) Second)
{
    public static CleanupRequest Parse(ReadOnlySpan<char> arg)
    {
        var firstDashAt = arg.IndexOf('-');
        var commaAt = arg.IndexOf(',');
        var secondDashAt = arg[commaAt..].IndexOf('-') + commaAt;

        return new CleanupRequest(
            (int.Parse(arg[..firstDashAt]), int.Parse(arg[(firstDashAt + 1)..commaAt])),
            (int.Parse(arg[(commaAt + 1)..secondDashAt]), int.Parse(arg[(secondDashAt + 1)..])));
    }
}