using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 1, "Puzzle one")]
public class Day01 : IPuzzle<int[]>
{
    public int[] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine).Select(int.Parse).ToArray();
    }

    public string Part1(int[] input)
    {
        return "Not implemented.";
    }

    public string Part2(int[] input)
    {
        return "Not implemented.";
    }
}