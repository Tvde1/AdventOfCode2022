using AoC.Common;
using AoC.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AoC.Puzzles._2019;

[Puzzle(2019, 2, "First intcode instructions")]
public class Day02 : IPuzzle<int[], Day02Input>
{
    public static int[] Parse(string inputText)
    {
        return inputText.Split(',').Select(int.Parse).ToArray();
    }

    public static string Part1(int[] input)
    {
        int[] tempmem = (int[])input.Clone();
        var computer = new Computer(tempmem, 12, 2);

        computer.Execute();

        return computer.FirstInteger.ToString();
    }

    public static string Part2(int[] input)
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

public class Day02TestInput : IPuzzleInput
{
    public static string Input => "1,9,10,3,2,3,11,0,99,30,40,50";
}

public class Day02Input : IPuzzleInput
{
    public static string Input => @"1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,10,1,19,1,19,5,23,1,23,9,27,2,27,6,31,1,31,6,35,2,35,9,39,1,6,39,43,2,10,43,47,1,47,9,51,1,51,6,55,1,55,6,59,2,59,10,63,1,6,63,67,2,6,67,71,1,71,5,75,2,13,75,79,1,10,79,83,1,5,83,87,2,87,10,91,1,5,91,95,2,95,6,99,1,99,6,103,2,103,6,107,2,107,9,111,1,111,5,115,1,115,6,119,2,6,119,123,1,5,123,127,1,127,13,131,1,2,131,135,1,135,10,0,99,2,14,0,0";
}