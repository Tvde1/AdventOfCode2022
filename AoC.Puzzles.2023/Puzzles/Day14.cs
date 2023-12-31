using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 14, "Parabolic Reflector Dish")]
public class Day14 : IPuzzle<char[,]>
{
    public char[,] Parse(string rawInput)
    {
        var testRawInput = """
                   O....#....
                   O.OO#....#
                   .....##...
                   OO.#O....O
                   .O.....O#.
                   O.#..O.#.#
                   ..O..#O..O
                   .......O..
                   #....###..
                   #OO..#....
                   """;

        //rawInput = testRawInput;

        return ParseMap(rawInput);
    }

    private static char[,] ParseMap(string rawInput)
    {
        var split = rawInput.Split(Environment.NewLine);

        var stuff = new char[split[0].Length, split.Length];

        for (var y = 0; y < split.Length; y++)
        {
            for (var x = 0; x < split[0].Length; x++)
            {
                stuff[x, y] = split[y][x];
            }
        }

        return stuff;
    }

    public string Part1(char[,] input)
    {
        for (var y = 0; y < input.GetLength(1); y++)
        {
            for (var x = 0; x < input.GetLength(0); x++)
            {
                Move(input, x, y, 0, -1);
            }
        }

        return Summarize(input).ToString();
    }
    
    public string Part2(char[,] input)
    {
        var states = new List<string>();

        for (var i = 0; i < 10_000; i++)
        {
            var printed = Print(input);
            var parsed = ParseMap(printed);
            var printedParsed = Print(parsed);
            
            Debug.Assert(printed == printedParsed);

            var existingIndex = states.IndexOf(printed);
            
            Console.WriteLine($"i: {i} | Summarize: {Summarize(input)}");
            // Console.WriteLine(copy);

            if (existingIndex is not -1)
            {
                var target = 1_000_000_000;
                var started = existingIndex;
                var now = i;

                var remaining = target - now;
                var cycleLength = now - started;
                var howManyRemainingFitInThat = remaining % cycleLength;

                var correctIndex = started + howManyRemainingFitInThat;

                var correctOne = states[correctIndex];

                return Summarize(ParseMap(correctOne)).ToString();
            }

            states.Add(printed);

            for (var y = 0; y < input.GetLength(1); y++)
            {
                for (var x = 0; x < input.GetLength(0); x++)
                {
                    Move(input, x, y, 0, -1);
                }
            }
            for (var y = 0; y < input.GetLength(1); y++)
            {
                for (var x = 0; x < input.GetLength(0); x++)
                {
                    Move(input, x, y, -1, 0);
                }
            }
            for (var y = input.GetLength(1) - 1; y >= 0; y--)
            {
                for (var x = 0; x < input.GetLength(0); x++)
                {
                    Move(input, x, y, 0, 1);
                }
            }
            for (var y = 0; y < input.GetLength(1); y++)
            {
                for (var x = input.GetLength(0) - 1; x >= 0; x--)
                {
                    Move(input, x, y, 1, 0);
                }
            }
        }

        throw new UnreachableException();
    }


    // public string Part2(char[,] input)
    // {
    //     var states = new List<char[]>();
    //
    //     for (var i = 0; i < 10_000; i++)
    //     {
    //         var copy = ((char[,]) input.Clone()).Cast<char>().ToArray();
    //
    //         var existingIndex = -1;
    //         for (var ind = 0; ind < states.Count; ind++)
    //         {
    //             if (copy.SequenceEqual(states[ind]))
    //             {
    //                 existingIndex = ind;
    //                 break;
    //             }
    //         }
    //         
    //         Console.WriteLine(Summarize(input).ToString());
    //
    //         if (existingIndex is not -1)
    //         {
    //             var target = 1_000_000_000;
    //             var started = existingIndex;
    //             var now = i;
    //
    //             var remaining = target - now;
    //             var cycleLength = now - started;
    //             var howManyRemainingFitInThat = remaining % cycleLength;
    //
    //             var correctIndex = started + howManyRemainingFitInThat;
    //
    //             var correctOne = states[correctIndex];
    //
    //             var xLength = input.GetLength(0);
    //             var yLength = input.GetLength(1);
    //             var temp = new char[xLength, yLength];
    //             var it = 0;
    //             for (var curY = 0; curY < yLength; curY++)
    //             {
    //                 for (var xLe = 0; xLe < xLength; xLe++)
    //                 {
    //                     temp[curY, xLe] = correctOne[it];
    //                     it++;
    //                 }
    //             }
    //
    //             // sanity check: 
    //             Debug.Assert(temp.Cast<char>().SequenceEqual(correctOne));
    //
    //             return Summarize(temp).ToString();
    //         }
    //
    //         states.Add(copy);
    //
    //         for (var y = 0; y < input.GetLength(1); y++)
    //         {
    //             for (var x = 0; x < input.GetLength(0); x++)
    //             {
    //                 Move(input, x, y, 0, -1);
    //             }
    //         }
    //         for (var y = 0; y < input.GetLength(1); y++)
    //         {
    //             for (var x = 0; x < input.GetLength(0); x++)
    //             {
    //                 Move(input, x, y, -1, 0);
    //             }
    //         }
    //         for (var y = input.GetLength(1) - 1; y <= 0; y--)
    //         {
    //             for (var x = 0; x < input.GetLength(0); x++)
    //             {
    //                 Move(input, x, y, 0, 1);
    //             }
    //         }
    //         for (var y = 0; y < input.GetLength(1); y++)
    //         {
    //             for (var x = input.GetLength(0) - 1; x <= 0; x--)
    //             {
    //                 Move(input, x, y, 1, 0);
    //             }
    //         }
    //     }
    //
    //     throw new UnreachableException();
    // }

    private static int Summarize(char[,] input)
    {
        var total = 0;
        var maxY = input.GetLength(1);
        for (var y = 0; y < maxY; y++)
        {
            for (var x = 0; x < input.GetLength(0); x++)
            {
                if (input[x, y] is 'O')
                {
                    total += maxY - y;
                }
            }
        }
        return total;
    }

    private static void Move(char[,] map, int x, int y, int dirX, int dirY)
    {
        var c = map[x, y];

        if (c != 'O') return;

        var targetX = x;
        var targetY = y;
        while (true)
        {
            targetX += dirX;
            targetY += dirY;
            char target;
            try
            {
                target = map[targetX, targetY];
            }
            catch
            {
                break;
            }

            if (target is '.')
            {
                continue;
            }

            if (target is '#' or 'O')
            {
                break;
            }
        }

        map[x, y] = '.';
        map[targetX - dirX, targetY - dirY] = 'O';
    }

    private static string Print(char[,] input)
    {
        var sb = new StringBuilder();

        for (var y = 0; y < input.GetLength(1); y++)
        {
            for (var x = 0; x < input.GetLength(0); x++)
            {
                sb.Append(input[x, y]);
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    // private static int CalculateHash(char[,] input)
    // {
    //     for (var y = 0; y < input.GetLength(1); y++)
    //     {
    //         for (var x = 0; x < input.GetLength(0); x++)
    //         {
    //             Move(input, x, y, 0, -1);
    //         }
    //     }   
    // }
}