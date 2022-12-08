using System.Diagnostics;
using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 8, "Treetop Tree House")]
public class Day08 : IPuzzle<byte[,]>
{
    public byte[,] Parse(string inputText)
    {
        // inputText = """
        //     30373
        //     25512
        //     65332
        //     33549
        //     35390
        //     """;

        var sp = inputText.AsSpan();

        var width = sp.IndexOf('\n') - 1;
        var height = sp.Length / (width + 1);

        var data = new byte[width, height];

        var x = 0;
        foreach (var line in sp.EnumerateLines())
        {
            var y = 0;
            foreach (var rune in line.EnumerateRunes())
            {
                data[x, y] = (byte)(rune.Value - '0');
                y++;
            }

            x++;
        }

        return data;
    }

    public string Part1(byte[,] input)
    {
        var sb = new StringBuilder();
        var visibleTrees = 0;
        for (byte x = 0; x < input.GetLength(0); x++)
        {
            for (byte y = 0; y < input.GetLength(1); y++)
            {
                var isVisible = IsTreeVisibleFromEdge(input, x, y);

                if (isVisible)
                {
                    visibleTrees++;

                    sb.Append("[green]");
                    sb.Append(input[x, y]);
                    sb.Append("[/]");
                }
                else
                {
                    sb.Append(input[x, y]);
                }
            }

            sb.AppendLine();
        }

        sb.Append(" (");
        sb.Append(visibleTrees);
        sb.Append(')');

        return sb.ToString();
    }

    public string Part2(byte[,] input)
    {
        var highestScenicScore = 0;
        for (byte x = 0; x < input.GetLength(0); x++)
        {
            for (byte y = 0; y < input.GetLength(1); y++)
            {
                var scenicScore = CalculateScenicScore(input, x, y);
                
                if (scenicScore > highestScenicScore)
                {
                    highestScenicScore = scenicScore;
                }
            }
        }

        return highestScenicScore.ToString();
    }

    private static bool IsTreeVisibleFromEdge(byte[,] input, byte x, byte y)
    {
        var treeValue = input[x, y];

        return IsVisibleLeft(input, treeValue, x, y, out _) ||
            IsVisibleRight(input, treeValue, x, y, out _) ||
            IsVisibleUp(input, treeValue, x, y, out _) ||
            IsVisibleDown(input, treeValue, x, y, out _);
    }

    private static int CalculateScenicScore(byte[,] input, byte x, byte y)
    {
        var treeValue = input[x, y];

        IsVisibleLeft(input, treeValue, x, y, out var scoreLeft);
        IsVisibleRight(input, treeValue, x, y, out var scoreRight);
        IsVisibleUp(input, treeValue, x, y, out var scoreUp);
        IsVisibleDown(input, treeValue, x, y, out var scoreDown);

        return scoreLeft * scoreRight * scoreUp * scoreDown;
    }

    private static bool IsVisibleLeft(byte[,] input, byte treeValue, byte x, byte y, out int iterations)
    {
        iterations = 0;
        for (var leftX = (x - 1); leftX >= 0; leftX--)
        {
            iterations++;
            if (input[leftX, y] >= treeValue)
                return false;
        }

        return true;
    }

    private static bool IsVisibleRight(byte[,] input, byte treeValue, byte x, byte y, out int iterations)
    {
        iterations = 0;
        for (var rightX = (x + 1); rightX <= input.GetLength(0) - 1; rightX++)
        {
            iterations++;
            if (input[rightX, y] >= treeValue)
                return false;
        }

        return true;
    }

    private static bool IsVisibleUp(byte[,] input, byte treeValue, byte x, byte y, out int iterations)
    {
        iterations = 0;
        for (var upY = (y - 1); upY >= 0; upY--)
        {
            iterations++;
            if (input[x, upY] >= treeValue)
                return false;
        }

        return true;
    }

    private static bool IsVisibleDown(byte[,] input, byte treeValue, byte x, byte y, out int iterations)
    {
        iterations = 0;
        for (var downY = (y + 1); downY <= input.GetLength(1) - 1; downY++)
        {
            iterations++;
            if (input[x, downY] >= treeValue)
                return false;
        }

        return true;
    }
}