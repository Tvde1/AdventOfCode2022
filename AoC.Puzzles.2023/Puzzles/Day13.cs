using System.Diagnostics;
using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 13, "Point of Incidence")]
public class Day13 : IPuzzle<List<Day13.RockPattern>>
{
    public record RockPattern(string Input, string[] AsRows, string[] AsColumns)
    {
        public static RockPattern Parse(string input)
        {
            var asRows = input.Split(Environment.NewLine);
            var asColumns = new string[asRows[0].Length];
            asColumns.AsSpan().Fill(string.Empty);

            foreach (var row in asRows)
            {
                var i = 0;
                foreach (var c in row)
                {
                    asColumns[i] += c;
                    i++;
                }
            }

            return new RockPattern(input, asRows, asColumns);
        }
    }

    public List<RockPattern> Parse(string rawInput)
    {
//          rawInput = """
//                     #.##..##.
//                     ..#.##.#.
//                     ##......#
//                     ##......#
//                     ..#.##.#.
//                     ..##..##.
//                     #.#.##.#.
//
//                     #...##..#
//                     #....#..#
//                     ..##..###
//                     #####.##.
//                     #####.##.
//                     ..##..###
//                     #....#..#
//                     """;
        return rawInput.Split(Environment.NewLine + Environment.NewLine).Select(RockPattern.Parse).ToList();
    }

    public string Part1(List<RockPattern> input)
    {
        const int mistakesAllowed = 0;
        return input.Sum(map => SummarizeFlip(map, mistakesAllowed)).ToString();
    }

    public string Part2(List<RockPattern> input)
    {
        const int mistakesAllowed = 1;
        return input.Sum(map => SummarizeFlip(map, mistakesAllowed)).ToString();
    }

    private static int SummarizeFlip(RockPattern map, int mistakesAllowed)
    {
        var flipColumnIndex = FindPerfectFlipIndex(map.AsColumns, mistakesAllowed);
        if (flipColumnIndex.HasValue)
        {
            return flipColumnIndex.Value;
        }

        var flipRowIndex = FindPerfectFlipIndex(map.AsRows, mistakesAllowed);
        if (flipRowIndex.HasValue)
        {
            return flipRowIndex.Value * 100;
        }
        
        Console.WriteLine();
        Console.WriteLine(map.Input);
        Console.WriteLine();
        throw new UnreachableException();
    }
    
    private static int? FindPerfectFlipIndex(IReadOnlyList<string> map, int mistakesAllowed)
    {
        for (var checkingRow = 0; checkingRow < map.Count - 1; checkingRow++)
        {
            var differences = GetDifferencesOutwards(map, checkingRow, checkingRow+1, mistakesAllowed);

            if (differences == mistakesAllowed)
            {
                return checkingRow + 1;
            }
        }

        return null;
    }

    private static int GetDifferencesOutwards(IReadOnlyList<string> map, int rowOne, int rowTwo, int diffsAllowed)
    {
        var differences = GetRowDifferences(map[rowOne], map[rowTwo]);

        if (rowOne == 0 || rowTwo == map.Count - 1)
        {
            return differences;
        }

        if (differences > diffsAllowed) return differences;

        return differences + GetDifferencesOutwards(map, rowOne - 1, rowTwo + 1, diffsAllowed - differences);
    }

    private static int GetRowDifferences(ReadOnlySpan<char> one, ReadOnlySpan<char> two)
    {
        var c = 0;
        for (var i = 0; i < one.Length; i++)
        {
            if (one[i] != two[i])
            {
                c++;
            }
        }        
        return c;
    }
}