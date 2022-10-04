using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 2, "First intcode instructions")]
public class Day02 : IPuzzle<int[]>
{
    public int[] Parse(string inputText)
    {
        return inputText.Split(',').Select(int.Parse).ToArray();
    }

    public string Part1(int[] input)
    {
        int[] tempmem = (int[])input.Clone();
        var computer = new Computer(tempmem, 12, 2);

        computer.Execute();

        return computer.FirstInteger.ToString();
    }

    public string Part2(int[] input)
    {
        for (int verb = 0; verb <= 99; verb++)
            for (int noun = 0; noun <= 99; noun++)
            {
                int[] tempmem = (int[])input.Clone();
                var computer = new Computer(tempmem, noun, verb);
                try
                {
                    computer.Execute();
                }
                catch { continue; }

                if (computer.FirstInteger == 19690720)
                {
                    return (noun * 100 + verb).ToString();
                }
            }


        return "No pair found.";
    }
}