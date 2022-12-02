using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using AoC.Puzzles._2019.Shared;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 17, "Repair drone")]
public class Day17 : IPuzzle<long[]>
{
    public long[] Parse(string inputText)
    {
        return inputText.Split(',').Select(long.Parse).ToArray();
    }

    public string Part1(long[] input)
    {
        var computer = new Computer(input);

        return "Not implemented";
    }

    public string Part2(long[] input)
    {
        var computer = new Computer(input);

        return "Not implemented";
    }
}