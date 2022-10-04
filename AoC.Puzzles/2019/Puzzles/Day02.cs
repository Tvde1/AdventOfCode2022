using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using AoC.Puzzles._2019.Shared;

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
        
        tempmem[1] = 12;
        tempmem[2] = 2;
        
        var computer = new Computer(tempmem);

        computer.Execute();

        return computer.FirstInteger.ToString();
    }

    public string Part2(int[] input)
    {
        for (var verb = 0; verb <= 99; verb++)
            for (var noun = 0; noun <= 99; noun++)
            {
                var tempmem = (int[])input.Clone();

                tempmem[1] = noun;
                tempmem[2] = verb;
                
                var computer = new Computer(tempmem);
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