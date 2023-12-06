using System.Net.WebSockets;
using System.Numerics;
using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 6, "asdf")]
public partial class Day06 : IPuzzle<Day06.RaceSheet>
{
    public partial record RaceSheet(int[] Times, int[] Distances)
    {
        public static RaceSheet Parse(string input)
        {
            var lines = input.Split('\n');
            var times = lines[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Skip(1).Select(int.Parse).ToArray();
            var distances = lines[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Skip(1).Select(int.Parse).ToArray();

            return new RaceSheet(times, distances);
        }
    }

    public RaceSheet Parse(string rawInput)
    {
        //rawInput = """
        //           Time:      7  15   30
        //           Distance:  9  40  200
        //           """;
        return RaceSheet.Parse(rawInput);
    }

    public string Part1(RaceSheet input)
    {
        var totals = 1;
        for (int i = 0; i < input.Times.Length; i++)
        {
            var time = input.Times[i];
            var distance = input.Distances[i];

            var count = 0;
            foreach (var winning in NumberOfWaysToWin(time, distance))
            {
                count++;
            }
            totals *= count;
        }

        return totals.ToString();
    }

    public string Part2(RaceSheet input) => Part2_Fast(input);

    public string Part2_Naive(RaceSheet input)
    {
        var time = long.Parse(string.Join(string.Empty, input.Times));
        var distance = long.Parse(string.Join(string.Empty, input.Distances));

        long wins = 0;
        for (long i = 0; i <= time; i++)
        {
            if (CalculateDistance(time, i) > distance)
            {
                wins++;
            }
        }

        return (wins + 1).ToString();
    }

    public string Part2_Fast(RaceSheet input)
    {
        var time = long.Parse(string.Join(string.Empty, input.Times));
        var distance = long.Parse(string.Join(string.Empty, input.Distances));

        long start = 0;
        for (long i = 0; i < time; i++)
        {
            if (CalculateDistance(time, i) > distance)
            {
                start = i;
                break;
            }
        }

        long end = 0;
        for (long i = time; i > 0; i--)
        {
            if (CalculateDistance(time, i) > distance)
            {
                end = i;
                break;
            }
        }

        return (end - start + 1).ToString();
    }

    public string Part2_Bad(RaceSheet input)
    {
        var time = long.Parse(string.Join(string.Empty, input.Times));
        var distance = long.Parse(string.Join(string.Empty, input.Distances));

        var a = new BigInteger(-1);
        var b = new BigInteger(time);
        var c = new BigInteger(distance);

        //var ans1 = (-b + Math.Sqrt(b * b - (4 * a * c))) / 2 * a;
        //var ans2 = (-b - Math.Sqrt(b * b - (4 * a * c))) / 2 * a;

        //var one = Math.Ceiling(ans1);
        //var two = Math.Floor(ans2);

        //return (two - one).ToString();

        var url = $"https://www.wolframalpha.com/input?i=%28%28{time}+-+x%29+*+x%29+-+{distance}+%3D+0";

        return $"""
            This is too difficult. Just use: Wolfram alpha:
            {url}
            Round the first answer up, and the second one down.
            """;
    }

    private IEnumerable<int> NumberOfWaysToWin(int time, int distance)
    {
        foreach(var holdDuration in Enumerable.Range(1, distance - 1))
        {
            var endReached = CalculateDistance(time, holdDuration);
            if (endReached > distance)
            {
                yield return holdDuration;
            }
        }
    }

    private long CalculateDistance(long time, long holdDuration)
    {
        return (time - holdDuration) * holdDuration;
    }
}