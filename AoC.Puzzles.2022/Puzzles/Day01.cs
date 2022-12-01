using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 1, "Puzzle one")]
public class Day01 : IPuzzle<int[][]>
{
    public int[][] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine + Environment.NewLine).Select(x => x.Split(Environment.NewLine).Select(int.Parse).ToArray()).ToArray();
    }

    public string Part1(int[][] input)
    {
        return input.Select(x => x.Sum()).Max().ToString();
    }

    public string Part2(int[][] input)
    {
        return input.Select(x => x.Sum()).OrderDescending().Take(3).Sum().ToString();
    }
}