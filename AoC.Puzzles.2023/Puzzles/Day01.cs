using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 1, "Trebuchet?!")]
public class Day01 : IPuzzle<string[]>
{
    public string[] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine);
    }

    public string Part1(string[] input)
    {
        return input
            .Select(x => x.Where(char.IsDigit))
            .Select(x => (x.First() - '0') * 10 + (x.Last() - '0'))
            .Sum()
            .ToString();
    }

    public string Part2(string[] input)
    {
        return input.Select(GetValues)
            .Sum()
            .ToString();
    }
    
    private static readonly Dictionary<string, int> SearchTerms = new()
    {
        { "zero", 0 },
        { "one", 1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5 },
        { "six", 6 },
        { "seven", 7 },
        { "eight", 8 },
        { "nine", 9 },
        { "0", 0 },
        { "1", 1 },
        { "2", 2 },
        { "3", 3 },
        { "4", 4 },
        { "5", 5 },
        { "6", 6 },
        { "7", 7 },
        { "8", 8 },
        { "9", 9 },
    };
    
    private long GetValues(string inp)
    {
        var begin = SearchTerms.Select(x => (ix: inp.IndexOf(x.Key, StringComparison.Ordinal), val: x.Value))
            .Where(x => x.ix != -1)
            .MinBy(x => x.ix).val;

        var end = SearchTerms.Select(x => (ix: inp.LastIndexOf(x.Key, StringComparison.Ordinal), val: x.Value))
            .Where(x => x.ix != -1)
            .MaxBy(x => x.ix).val;

        return begin * 10 + end;
    }
}