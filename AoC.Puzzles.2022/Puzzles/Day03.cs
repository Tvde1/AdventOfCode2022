using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 3, "Rucksack Reorganization")]
public class Day03 : IPuzzle<string[]>
{
    public string[] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine);
    }

    public string Part1(string[] input)
    {
        var duplicates = input.Select(x =>
        {
            var left = x.AsSpan(0, x.Length / 2);
            var right = x.AsSpan(x.Length / 2);

            foreach (var c in left)
            {
                if (right.Contains(c))
                {
                    return c;
                }
            }

            throw new InvalidOperationException("Left and right do not share a character");
        });

        return duplicates.Select(CalculateScore).Sum().ToString();
    }

    public string Part2(string[] input)
    {
        var duplicates = input
            .Chunk(3)
            .Select(x =>
        {
            var counts = new Dictionary<char, int>();

            foreach (var bp in x)
            {
                foreach (var cha in new HashSet<char>(bp))
                {
                    if (counts.ContainsKey(cha))
                    {
                        counts[cha]++;
                    }
                    else
                    {
                        counts[cha] = 1;
                    }
                }
            }

            return counts.First(count => count.Value == 3).Key;
        });

        return duplicates.Select(CalculateScore).Sum().ToString();
    }

    private static int CalculateScore(char c)
    {
        if (char.IsLower(c))
        {
            return c - 'a' + 1;
        }

        return c - 'A' + 26 + 1;
    }
}