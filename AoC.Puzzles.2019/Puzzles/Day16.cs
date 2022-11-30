using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using AoC.Puzzles._2019.Shared;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 16, "Flawed frequency transmission")]
public class Day16 : IPuzzle<int[]>
{
    const int PhaseCount = 100;

    public int[] Parse(string inputText)
    {
        // inputText = "80871224585914546619083218645595";
        return inputText.Select(x => x - '0').ToArray();
    }

    public string Part1(int[] input)
    {
        var resultSignal = DecodeSignal(input, PhaseCount);

        return string.Join(string.Empty, resultSignal.Take(8));
    }

    public string Part2(int[] input)
    {
        var offset = int.Parse(string.Join(string.Empty, input.Take(7)));

        IEnumerable<int> yikes = input;
        
        for (var i = 0; i < 999; i++)
        {
            yikes = yikes.Concat(input);
        }

        var shit = yikes.ToArray();

        var resultSignal = DecodeSignal(shit, PhaseCount);

        return string.Join(string.Empty, resultSignal.Skip(offset).Take(8));
    }


    private static int[] DecodeSignal(int[] input, int phaseCount)
    {
        var patterns = GeneratePatterns(input.Length);

        var workingArr = (int[])input.Clone();

        for (var i = 0; i < phaseCount; i++)
        {
            var newArr = new int[input.Length];

            for (var charIndexY = 0; charIndexY < workingArr.Length; charIndexY++)
            {
                var tot = 0;
                for (var charIndexX = 0; charIndexX < workingArr.Length; charIndexX++)
                {
                    tot += workingArr[charIndexX] * patterns[charIndexY, charIndexX];
                }

                newArr[charIndexY] = Math.Abs(tot) % 10;
            }

            workingArr = newArr;
        }

        return workingArr;
    }

    private static int[,] GeneratePatterns(int inputLength)
    {
        var arr = new int[inputLength, inputLength];

        for (var i = 0; i < inputLength; i++)
        {
            using var iterator = GeneratePattern(i + 1).Skip(1).GetEnumerator();
            for (var j = 0; j < inputLength; j++)
            {
                iterator.MoveNext();
                arr[i, j] = iterator.Current;
            }
        }

        return arr;
    }

    private static IEnumerable<int> GeneratePattern(int repeatCount)
    {
        while (true)
        {
            for (var i = 0; i < repeatCount; i++)
            {
                yield return 0;
            }

            for (var i = 0; i < repeatCount; i++)
            {
                yield return 1;
            }

            for (var i = 0; i < repeatCount; i++)
            {
                yield return 0;
            }

            for (var i = 0; i < repeatCount; i++)
            {
                yield return -1;
            }
        }
    }
}