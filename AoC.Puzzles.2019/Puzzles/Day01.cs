using System.Runtime.CompilerServices;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 1, "Calculate fuel")]
public class Day01 : IPuzzle<int[]>
{
    public int[] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine).Select(int.Parse).ToArray();
    }

    public string Part1(int[] input)
    {
        var mass = 0;
        foreach (var item in input)
        {
            mass += CalculateFuel(item);
        }

        return mass.ToString();
    }

    public string Part2(int[] input)
    {
        var mass = 0;
        foreach(var item in input)
        {
            int fuelMass = CalculateFuel(item);

            while (fuelMass > 0)
            {
                mass += fuelMass;
                fuelMass = CalculateFuel(fuelMass);
            }
        }

        return mass.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateFuel(int mass) => mass / 3 - 2;
}