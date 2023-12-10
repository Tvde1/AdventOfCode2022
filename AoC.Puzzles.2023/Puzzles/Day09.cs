using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 9, "Mirage Maintenance")]
public partial class Day09 : IPuzzle<List<long[]>>
{
    public List<long[]> Parse(string rawInput)
    {
        // rawInput = """
        //            0 3 6 9 12 15
        //            1 3 6 10 15 21
        //            10 13 16 21 30 45
        //            """;
        return rawInput.Split(Environment.NewLine).Select(x => x.Split(' ').Select(long.Parse).ToArray()).ToList();
    }

    public string Part1(List<long[]> input)
    {
        return input.Select(PredictNextValue)
            .Sum()
            .ToString();
    }

    public string Part2(List<long[]> input)
    {
        return input.Select(PredictPreviousValue)
            .Sum()
            .ToString();
    }

    private long PredictNextValue(long[] sequence)
    {
        if (sequence.All(x => x is 0))
        {
            return 0;
        }

        var diffs = sequence
            .WindowPairs()
            .Select(x => x.Item2 - x.Item1)
            .ToArray();
        var next = PredictNextValue(diffs);

        return next + sequence[^1];
    }
    
    private long PredictPreviousValue(long[] sequence)
    {
        if (sequence.All(x => x is 0))
        {
            return 0;
        }

        var diffs = sequence
            .WindowPairs()
            .Select(x => x.Item2 - x.Item1)
            .ToArray();
        var previous = PredictPreviousValue(diffs);

        return sequence[0] - previous;
    }
}

public static class Day9Ext
{
    public static IEnumerable<(T, T)> WindowPairs<T>(this IEnumerable<T> source)
    {
        var hasIterated = false;
        T prevItem = default!;
        foreach (var item in source)
        {
            if (!hasIterated)
            {
                prevItem = item;
                hasIterated = true;
                continue;
            }

            yield return (prevItem, item);

            prevItem = item;
        }
    }
}