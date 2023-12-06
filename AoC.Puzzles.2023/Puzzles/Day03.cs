using System.Buffers;
using System.Diagnostics;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 3, "Gear Ratios")]
public class Day03 : IPuzzle<string[]>
{
    public string[] Parse(string rawInput)
    {
        // rawInput = """
        //            467..114..
        //            ...*......
        //            ..35..633.
        //            ......#...
        //            617*......
        //            .....+.58.
        //            ..592.....
        //            ......755.
        //            ...$.*....
        //            .664.598..
        //            """;

        // rawInput = """
        //            .......
        //            446%647
        //            .......
        //            """;
        return rawInput.Split(Environment.NewLine);
    }

    public string Part1(string[] input)
    {
        var validNumbers = new List<int>();

        var firstRow = input[0];
        var secondRow = input[1];

        var startNums = ExtractNumbers(firstRow)
            .Where(x => ContainsSymbolTouching(firstRow, x.Range).HasValue ||
                ContainsSymbolTouching(secondRow, x.Range).HasValue)
            .Select(x => x.Number)
            .ToList();
        validNumbers.AddRange(startNums);

        for (int i = 1; i < input.Length - 1; i++)
        {
            var nums = ExtractNumbers(input[i])
                .Where(x =>
                    ContainsSymbolTouching(input[i - 1], x.Range).HasValue ||
                    ContainsSymbolTouching(input[i], x.Range).HasValue ||
                    ContainsSymbolTouching(input[i + 1], x.Range).HasValue)
                .Select(x => x.Number)
                .ToList();
            validNumbers.AddRange(nums);
        }

        var lastRow = input[^1];
        var secondToLastRow = input[^2];

        var endNums = ExtractNumbers(lastRow)
            .Where(x => ContainsSymbolTouching(lastRow, x.Range).HasValue ||
                ContainsSymbolTouching(secondToLastRow, x.Range).HasValue)
            .Select(x => x.Number)
            .ToList();
        validNumbers.AddRange(endNums);

        return validNumbers.Select(x => (long)x).Sum().ToString();
    }

    public string Part2(string[] input)
    {
        var bag = new List<(int Number, int StarRow, int StarPos)>();

        {
            var firstRow = input[0];
            var secondRow = input[1];

            var startNums = ExtractNumbers(firstRow);

            bag.AddRange(startNums.Select(x =>
                {
                    var co = ContainsSymbolTouching(firstRow, x.Range);
                    return co is not ('*', var idx) ? (0, -1, 0) : (x.Number, 0, idx);
                })
                .Where(x => x.Item2 is not -1));
            bag.AddRange(startNums.Select(x =>
                {
                    var co = ContainsSymbolTouching(secondRow, x.Range);
                    return co is not ('*', var idx) ? (0, -1, 0) : (x.Number, 1, idx);
                })
                .Where(x => x.Item2 is not -1));
        }

        {
            for (var i = 1; i < input.Length - 1; i++)
            {
                var nums = ExtractNumbers(input[i]);

                for (var j = -1; j <= 1; j++)
                {
                    bag.AddRange(nums.Select(x =>
                        {
                            var co = ContainsSymbolTouching(input[i + j], x.Range);
                            return co is not ('*', var idx) ? (0, -1, 0) : (x.Number, i + j, idx);
                        })
                        .Where(x => x.Item2 is not -1));
                }
            }
        }


        var endNums = ExtractNumbers(input[^1]);

        {
            bag.AddRange(endNums.Select(x =>
                {
                    var co = ContainsSymbolTouching(input[^1], x.Range);
                    return co is not ('*', var idx) ? (0, -1, 0) : (x.Number, input.Length - 1, idx);
                })
                .Where(x => x.Item2 is not -1));
            bag.AddRange(endNums.Select(x =>
                {
                    var co = ContainsSymbolTouching(input[^2], x.Range);
                    return co is not ('*', var idx) ? (0, -1, 0) : (x.Number, input.Length - 2, idx);
                })
                .Where(x => x.Item2 is not -1));
        }

        var ratios = bag.ToLookup(x => (x.StarPos, x.StarRow))
            .Where(x => x.Count() == 2)
            .Select(x => x.First().Number * x.Last().Number);

        return ratios.Sum().ToString();
    }

    private static IReadOnlyList<(int Number, Range Range)> ExtractNumbers(ReadOnlySpan<char> row)
    {
        var l = new List<(int, Range)>();

        var searchIdx = 0;

        int? startIdx = null;
        int? num = null;

        while (true)
        {
            if (row.Length == searchIdx)
            {
                if (num is not null)
                {
                    l.Add((num.Value, startIdx!.Value..(row.Length - 1)));
                }

                return l;
            }

            var c = row[searchIdx];

            if (c is >= '0' and <= '9')
            {
                startIdx ??= searchIdx == 0 ? searchIdx : searchIdx - 1;
                num ??= 0;

                var digit = c - '0';
                num *= 10;
                num += digit;
            }
            else if (num.HasValue)
            {
                var endIdx = searchIdx == row.Length ? searchIdx : searchIdx + 1;
                l.Add((num.Value, startIdx!.Value..endIdx));
                num = null;
                startIdx = null;
            }

            searchIdx++;
        }
    }

    private const string SearchSpace = "-#=*+@$&/%";
    private static readonly SearchValues<char> SymbolSearchValues = SearchValues.Create(SearchSpace);

    private static (char Value, int Index)? ContainsSymbolTouching(ReadOnlySpan<char> row, Range range)
    {
        var idx = row[range].IndexOfAny(SymbolSearchValues);
        return idx is -1 ? null : (row[range][idx], range.Start.Value + idx);
    }
}