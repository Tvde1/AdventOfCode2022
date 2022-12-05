using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 5, "Crane operator")]
public class Day05 : IPuzzle<CraneOperatorInstructions>
{
    public CraneOperatorInstructions Parse(string inputText)
    {
//         inputText = """
//             [D]    
//         [N] [C]    
//         [Z] [M] [P]
//          1   2   3 
//
//         move 1 from 2 to 1
//         move 3 from 1 to 3
//         move 2 from 2 to 1
//         move 1 from 1 to 2
//         """;

        return CraneOperatorInstructions.Parse(inputText);
    }

    public string Part1(CraneOperatorInstructions input)
    {
        var configuration = input.Configuration;

        foreach (var instruction in input.Instructions)
        {
            for (var i = 0; i < instruction.Count; i++)
            {
                configuration[instruction.To].Push(configuration[instruction.From].Pop());
            }
        }

        var sb = new StringBuilder();
        foreach (var stack in configuration)
        {
            sb.Append(stack.Value.Pop());
        }

        return sb.ToString();
    }

    public string Part2(CraneOperatorInstructions input)
    {
        var configuration = input.Configuration;


        foreach (var instruction in input.Instructions)
        {
            var onCrane = new char[instruction.Count];
            for (var i = 0; i < instruction.Count; i++)
            {
                onCrane[i] = configuration[instruction.From].Pop();
            }

            for (var i = 1; i <= instruction.Count; i++)
            {
                configuration[instruction.To].Push(onCrane[^i]);
            }
        }

        var sb = new StringBuilder();
        foreach (var stack in configuration)
        {
            sb.Append(stack.Value.Pop());
        }

        return sb.ToString();
    }
}

public record CraneOperatorInstructions(Dictionary<int, Stack<char>> Configuration,
    List<(int Count, int From, int To)> Instructions)
{
    // private static Regex _instructionRegex =
    //     new(@"move (?<count>\d{1,2}) from (?<from>\d) to (?<to>\d)", RegexOptions.Compiled | RegexOptions.NonBacktracking);

    public static CraneOperatorInstructions Parse(ReadOnlySpan<char> arg)
    {
        var configuration = new Dictionary<int, List<char>>();
        var instructions = new List<(int Count, int From, int To)>();

        bool hasSetupConfiguration = false;
        bool hasCompletedConfiguration = false;
        foreach (var line in arg.EnumerateLines())
        {
            if (!hasSetupConfiguration)
            {
                for (var i = 1; i <= (line.Length + 1) / 4; i++)
                {
                    configuration[i] = new List<char>();
                }

                hasSetupConfiguration = true;
            }

            if (!hasCompletedConfiguration)
            {
                if (!line.Contains('['))
                {
                    foreach (var c in configuration)
                    {
                        c.Value.Reverse();
                    }

                    hasCompletedConfiguration = true;
                    continue;
                }

                for (var i = 0; i < (line.Length + 1) / 4; i++)
                {
                    var c = line[(i * 4) + 1];
                    if (c != ' ')
                    {
                        configuration[i + 1].Add(c);
                    }
                }

                continue;
            }

            if (line.IsEmpty || line.IsWhiteSpace())
            {
                continue;
            }

            var fromIndex = line.IndexOf("from");

            const int moveLength = 5; // "move "
            const int fromLength = 5; // "from "
            const int spaceLength = 1; // " "
            const int charLength = 1; // "X"
            const int toLength = 4; // " to "
            // FromIndex + "from ".Length
            var count = int.Parse(line[moveLength..(fromIndex - spaceLength)]);
            // MoveIndex + " "
            var from = line[fromIndex + fromLength] - '0';
            // MoveIndex + space + char + " to "
            var to = line[fromIndex + fromLength + charLength + toLength] - '0';

            instructions.Add((count, from, to));
        }

        return new(configuration.ToDictionary(x => x.Key, x => new Stack<char>(x.Value)), instructions);
    }
}