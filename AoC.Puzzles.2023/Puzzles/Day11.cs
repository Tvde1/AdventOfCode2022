using System.Collections.Frozen;
using System.Numerics;
using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 11, "Cosmic Expansion")]
public partial class Day11 : IPuzzle<Day11.SpaceMap>
{
    public partial record SpaceMap
    {
        public string Map { get; }

        private SpaceMap(string map)
        {
            Map = map;
        }
        
        public static SpaceMap Parse(string input)
        {
            return new SpaceMap(input);
        }
    }

    public SpaceMap Parse(string rawInput)
    {
        // rawInput = """
        //            ...#......
        //            .......#..
        //            #.........
        //            ..........
        //            ......#...
        //            .#........
        //            .........#
        //            ..........
        //            .......#..
        //            #...#.....
        //            """;
        return SpaceMap.Parse(rawInput);
    }

    public string Part1(SpaceMap input)
    {
        return CalculateDistances(input, 2);
    }

    public string Part2(SpaceMap input)
    {
        return CalculateDistances(input, 1_000_000);
    }

    private string CalculateDistances(SpaceMap input, int expansionFactor)
    {
        var mapRows = input.Map.Split(Environment.NewLine);

        var emptyRows = mapRows
            .Select((x, idx) => (x, idx))
            .Where(x => !x.x.AsSpan().ContainsAnyExcept('.'))
            .Select(x => x.idx)
            .ToList();

        var emptyColumns = Enumerable.Range(0, mapRows[0].Length)
            .Where(col =>
                Enumerable.Range(0, mapRows.Length)
                    .All(x => mapRows[x][col] is '.')
            )
            .ToList();

        var points = new List<(int X, int Y)>();
        for (var x = 0; x < mapRows[0].Length; x++)
        {
            for (var y = 0; y < mapRows.Length; y++)
            {
                if (mapRows[y][x] is '#')
                {
                    points.Add((x, y));
                }
            }
        }

        var pairs = points.SelectMany((left, index) =>
                points.Skip(index + 1)
                    .Select(right => (left, right)))
            .ToList();

        return pairs.Select(pair => GetDistance(pair, emptyRows, emptyColumns, expansionFactor))
            .Sum()
            .ToString();
    }
    
    private long GetDistance(((int X, int Y) left, (int X, int Y) right) pair, List<int> emptyRows, List<int> emptyColumns, int expansionFactor)
    {
        var left = Math.Min(pair.left.X, pair.right.X);
        var right = Math.Max(pair.left.X, pair.right.X);
        var top = Math.Min(pair.left.Y, pair.right.Y);
        var bottom = Math.Max(pair.left.Y, pair.right.Y);

        var amountOfEmptyRows = emptyRows.Count(x => x > top && x < bottom);
        var amountOfEmptyColumns = emptyColumns.Count(y => y > left && y < right);

        var normalRows = right - left - amountOfEmptyColumns;
        var normalColumns = bottom - top - amountOfEmptyRows;

        return normalRows + (amountOfEmptyColumns * expansionFactor) +
            normalColumns + (amountOfEmptyRows * expansionFactor);
    }
}