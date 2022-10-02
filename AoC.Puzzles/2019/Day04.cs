using AoC.Common;
using AoC.Common.Models;
using System.ComponentModel;
using System.Diagnostics;

namespace AoC.Puzzles._2019;

[Puzzle(2019, 4, "Secure container number generator")]
public class Day04 : IPuzzle<(int From, int To), Day04Input>
{
    public static (int From, int To) Parse(string inputText)
    {
        var s = inputText.Split('-');
        return (int.Parse(s[0]), int.Parse(s[1]));
    }

    public static string Part1((int From, int To) input)
    {
        static bool TestNumber(int number)
        {
            var strNum = number.ToString();
            var hasDoubleOccurred = false;
            var prevDigit = -1;

            foreach (var c in strNum)
            {
                if (c < prevDigit)
                {
                    return false;
                }

                if (c == prevDigit)
                {
                    hasDoubleOccurred = true;
                }

                prevDigit = c;
            }

            return hasDoubleOccurred;
        }

        var count = 0;
        var l = Parallel.For(input.From, input.To, (n) =>
        {
            if (TestNumber(n))
            {
                Interlocked.Increment(ref count);
            }
        });

        return count.ToString();
    }

    public static string Part2((int From, int To) input)
    {
        static bool TestNumber(int number)
        {
            var strNum = number.ToString();
            var hasDoubleOccurred = false;
            var hasDoubleOccurredFor = new int[10];
            var prevDigit = -1;

            foreach (var c in strNum)
            {
                var numChar = c - '0';
                if (numChar < prevDigit)
                {
                    return false;
                }

                if (numChar == prevDigit)
                {
                    hasDoubleOccurredFor[numChar]++;
                    hasDoubleOccurred = true;
                }

                prevDigit = numChar;
            }

            return hasDoubleOccurred && hasDoubleOccurredFor.Any(x => x == 1);
        }

        var count = 0;
        var l = Parallel.For(input.From, input.To, (n) =>
        {
            if (TestNumber(n))
            {
                Interlocked.Increment(ref count);
            }
        });

        return count.ToString();
    }
}

public class Day04Input : IPuzzleInput
{
    public static string Input => @"165432-707912";
}