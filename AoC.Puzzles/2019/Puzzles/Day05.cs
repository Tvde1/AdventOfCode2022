using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using AoC.Puzzles._2019.Shared;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 5, "Advanced intcode")]
public class Day05 : IPuzzle<int[]>
{
    public int[] Parse(string inputText)
    {
        return inputText.Split(',').Select(int.Parse).ToArray();
    }

    public string Part1(int[] input)
    {
        int[] tempmem = (int[])input.Clone();
        var computer = new Computer(tempmem);

        var numbers = computer.Execute(new long[] { 1 });

        return string.Join(string.Empty, numbers);
    }

    public string Part2(int[] input)
    {
        int[] tempmem = (int[])input.Clone();
        var computer = new Computer(tempmem);

        var numbers = computer.Execute(new long[] { 5 });

        return string.Join(string.Empty, numbers);
    }
}