using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 10, "Cathode-Ray Tube")]
public class Day10 : IPuzzle<Instruction[]>
{
    public Instruction[] Parse(string inputText)
    {
        // inputText = """
        //     addx 15
        //     addx -11
        //     addx 6
        //     addx -3
        //     addx 5
        //     addx -1
        //     addx -8
        //     addx 13
        //     addx 4
        //     noop
        //     addx -1
        //     addx 5
        //     addx -1
        //     addx 5
        //     addx -1
        //     addx 5
        //     addx -1
        //     addx 5
        //     addx -1
        //     addx -35
        //     addx 1
        //     addx 24
        //     addx -19
        //     addx 1
        //     addx 16
        //     addx -11
        //     noop
        //     noop
        //     addx 21
        //     addx -15
        //     noop
        //     noop
        //     addx -3
        //     addx 9
        //     addx 1
        //     addx -3
        //     addx 8
        //     addx 1
        //     addx 5
        //     noop
        //     noop
        //     noop
        //     noop
        //     noop
        //     addx -36
        //     noop
        //     addx 1
        //     addx 7
        //     noop
        //     noop
        //     noop
        //     addx 2
        //     addx 6
        //     noop
        //     noop
        //     noop
        //     noop
        //     noop
        //     addx 1
        //     noop
        //     noop
        //     addx 7
        //     addx 1
        //     noop
        //     addx -13
        //     addx 13
        //     addx 7
        //     noop
        //     addx 1
        //     addx -33
        //     noop
        //     noop
        //     noop
        //     addx 2
        //     noop
        //     noop
        //     noop
        //     addx 8
        //     noop
        //     addx -1
        //     addx 2
        //     addx 1
        //     noop
        //     addx 17
        //     addx -9
        //     addx 1
        //     addx 1
        //     addx -3
        //     addx 11
        //     noop
        //     noop
        //     addx 1
        //     noop
        //     addx 1
        //     noop
        //     noop
        //     addx -13
        //     addx -19
        //     addx 1
        //     addx 3
        //     addx 26
        //     addx -30
        //     addx 12
        //     addx -1
        //     addx 3
        //     addx 1
        //     noop
        //     noop
        //     noop
        //     addx -9
        //     addx 18
        //     addx 1
        //     addx 2
        //     noop
        //     noop
        //     addx 9
        //     noop
        //     noop
        //     noop
        //     addx -1
        //     addx 2
        //     addx -37
        //     addx 1
        //     addx 3
        //     noop
        //     addx 15
        //     addx -21
        //     addx 22
        //     addx -6
        //     addx 1
        //     noop
        //     addx 2
        //     addx 1
        //     noop
        //     addx -10
        //     noop
        //     noop
        //     addx 20
        //     addx 1
        //     addx 2
        //     addx 2
        //     addx -6
        //     addx -11
        //     noop
        //     noop
        //     noop
        //     """;

        List<Instruction> instructions = new();
        var readOnlySpan = inputText.AsSpan();
        foreach (var line in readOnlySpan.EnumerateLines())
        {
            instructions.Add(Instruction.Parse(line));
        }

        return instructions.ToArray();
    }

    public string Part1(Instruction[] input)
    {
        long solution = 0;

        RunComputer(input, (cycleNumber, xRegister) =>
        {
            if (cycleNumber is 20 or 60 or 100 or 140 or 180 or 220)
            {
                solution += cycleNumber * xRegister;
            }
        });

        return solution.ToString();
    }

    public string Part2(Instruction[] input)
    {
        var solution = new StringBuilder();

        RunComputer(input, (cycleNumber, xRegister) =>
        {
            var cycleIndex = cycleNumber % 40;
            solution.Append((xRegister - cycleIndex) is -2 or -1 or 0
                ? '#'
                : '.');

            if (cycleIndex == 0)
            {
                solution.AppendLine();
            }
        });

        return solution.ToString();
    }

    void RunComputer(IEnumerable<Instruction> instructions, Action<int, long> callback)
    {
        var cycleNumber = 0;
        long xRegister = 1;
        foreach (var instruction in instructions)
        {
            switch (instruction)
            {
                case AddXInstruction addXInstruction:
                {
                    cycleNumber++;
                    callback(cycleNumber, xRegister);
                    cycleNumber++;
                    callback(cycleNumber, xRegister);
                    xRegister += addXInstruction.Value;
                    break;
                }
                case NoopInstruction:
                {
                    cycleNumber++;
                    callback(cycleNumber, xRegister);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(instruction));
            }
        }
    }
}

public abstract record Instruction
{
    public static Instruction Parse(ReadOnlySpan<char> input)
    {
        if (input.StartsWith("noop"))
        {
            return new NoopInstruction();
        }

        if (input.StartsWith("addx"))
        {
            return new AddXInstruction(long.Parse(input[5..]));
        }

        throw new InvalidOperationException($"Unknown instruction: {input}");
    }
}

public record NoopInstruction : Instruction;

public record AddXInstruction(long Value) : Instruction;