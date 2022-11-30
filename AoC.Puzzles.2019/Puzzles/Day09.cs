using System.Diagnostics;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using AoC.Puzzles._2019.Shared;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 9, "Sensor Boost")]
public class Day09 : IPuzzle<long[]>
{
    public long[] Parse(string inputText)
    {
        return inputText.Split(',').Select(long.Parse).ToArray();
    }

    public string Part1(long[] input)
    {
        var computer = new Computer(input);

        computer.ContinueWithInput(1, out var outputs);

        return string.Join(',', outputs);
    }

    public string Part2(long[] input)
    {
        var computer = new Computer(input);

        computer.ContinueWithInput(2, out var output);
        return string.Join(',', output);
    }
}