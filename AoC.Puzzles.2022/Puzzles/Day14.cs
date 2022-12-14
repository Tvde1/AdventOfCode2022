using System.Diagnostics;
using System.Drawing;
using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 14, "Regolith Reservoir")]
public class Day14 : IPuzzle<Range[]>
{
    public Range[] Parse(string inputText)
    {
        // inputText = """
        //     498,4 -> 498,6 -> 496,6
        //     503,4 -> 502,4 -> 502,9 -> 494,9
        //     """;

        return inputText.Split(Environment.NewLine).Select(x => x.Split("->", StringSplitOptions.TrimEntries)
                .Select(x => Point.Parse(x))
                .ToList())
            .SelectMany(x => x.Zip(x.Skip(1), (a, b) => new Range(a, b)))
            .ToArray();
    }

    public string Part1(Range[] input)
    {
        var map = CreateMap(input);

        var sandsSpawned = 0;
        while (MoveSandDownOrStay(ref map, 500, 0))
        {
            sandsSpawned++;
        }

        return sandsSpawned.ToString();
    }

    public string Part2(Range[] input)
    {
        var lowestY = input.SelectMany(x => new[]
        {
            x.Start, x.End,
        }).Max(x => x.Y);

        var floorY = lowestY + 2;

        var floorRange = new Range(new Point(200, floorY), new Point(700, floorY));

        var map = CreateMap(input.Append(floorRange).ToArray());

        var sandsSpawned = 0;
        while (MoveSandDownOrStay(ref map, 500, 0))
        {
            sandsSpawned++;
        }

        return sandsSpawned.ToString();
    }

    private static bool MoveSandDownOrStay(ref char[,] map, int sandX, int sandY)
    {
        if (map[sandX, sandY] != '.')
        {
            return false;
        }
        
        try
        {
            while (true)
            {
                var belowUs = map[sandX, sandY + 1];

                if (belowUs == '.')
                {
                    sandY += 1;
                    continue;
                }

                var belowLeft = map[sandX - 1, sandY + 1];
                if (belowLeft == '.')
                {
                    sandX -= 1;
                    sandY += 1;
                    continue;
                }

                var belowRight = map[sandX + 1, sandY + 1];
                if (belowRight == '.')
                {
                    sandX += 1;
                    sandY += 1;
                    continue;
                }

                map[sandX, sandY] = 'O';
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    private static char[,] CreateMap(Range[] input)
    {
        var points = input.SelectMany(x => new[]
        {
            x.Start, x.End,
        }).ToArray();

        var maxX = points.Max(x => x.X);
        var maxY = points.Max(x => x.Y);

        var map = new char[maxX + 1, maxY + 1];

        for (var x = 0; x < map.GetLength(0); x++)
        {
            for (var y = 0; y < map.GetLength(1); y++)
            {
                map[x, y] = '.';
            }
        }

        foreach (var range in input)
        {
            range.Fill(ref map);
        }

        return map;
    }

    private static string PrintMap(char[,] map)
    {
        var sb = new StringBuilder();
        for (var y = 0; y < map.GetLength(1); y++)
        {
            for (var x = 0; x < map.GetLength(0); x++)
            {
                sb.Append(map[x, y]);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public record Point(int X, int Y)
{
    public static Point Parse(ReadOnlySpan<char> span)
    {
        var commaIndex = span.IndexOf(',');
        var x = int.Parse(span[..commaIndex]);
        var y = int.Parse(span[(commaIndex + 1)..]);
        return new(x, y);
    }
}

public record Range(Point Start, Point End)
{
    public void Fill(ref char[,] map)
    {
        if (Start.X == End.X)
        {
            int min, max;

            if (Start.Y > End.Y)
            {
                min = End.Y;
                max = Start.Y;
            }
            else
            {
                min = Start.Y;
                max = End.Y;
            }

            for (var y = min; y <= max; y++)
            {
                map[Start.X, y] = '#';
            }
        }
        else
        {
            int min, max;

            if (Start.X > End.X)
            {
                min = End.X;
                max = Start.X;
            }
            else
            {
                min = Start.X;
                max = End.X;
            }

            for (var x = min; x <= max; x++)
            {
                map[x, Start.Y] = '#';
            }
        }
    }
}