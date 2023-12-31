using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 15, "Lens Library")]
public class Day15 : IPuzzle<string[]>
{
    public string[] Parse(string rawInput)
    {
        var testRawInput = """
                   rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
                   """;

        // rawInput = testRawInput;

        return rawInput.Split(',');
    }

    public string Part1(string[] input)
    {
        return input.Select(Hash).Sum().ToString();
    }

    record Box(List<(string Label, int Strength)> Lenses);
    
    public string Part2(string[] input)
    {
        var boxes = Enumerable.Range(0, 256)
            .Select(_ => new Box(new()))
            .ToArray();

        foreach (var instruction in input)
        {
            if (instruction.Contains('-'))
            {
                var label = instruction[..instruction.IndexOf('-')];

                var hash = Hash(label);
                var containing = boxes[hash].Lenses.Select((x, i) => (contains: x.Label == label, i:i))
                    .Where(x => x.contains).ToList();
                if (containing is [var item, ..])
                {
                    boxes[hash].Lenses.RemoveAt(item.i);
                }
            }
            else if (instruction.Contains('='))
            {
                var equalsIndex = instruction.IndexOf('=');
                var label = instruction[..equalsIndex];
                var number = int.Parse(instruction[(equalsIndex + 1)..]);
                var hash = Hash(label);
                var containing = boxes[hash].Lenses.Select((x, i) => (contains: x.Label == label, i:i))
                    .Where(x => x.contains).ToList();
                if (containing is [var item, ..])
                {
                    boxes[hash].Lenses[item.i] = (label, number);
                }
                else
                {
                    boxes[hash].Lenses.Add((label, number));
                }
            }
            else
            {
                throw new UnreachableException();
            }
        }

        return boxes.SelectMany((box, boxIndex) =>
                    box.Lenses.Select(
                        (lens, lensIndex) => (boxIndex + 1) * (lensIndex + 1) * lens.Strength))
                .Sum()
                .ToString();
    }

    private static int Hash(string s)
    {
        int curr = 0;
        foreach (var c in s)
        {
            var asciiCode = (int)c;
            curr += asciiCode;
            curr *= 17;
            curr %= 256;
        }

        return curr;
    }
}