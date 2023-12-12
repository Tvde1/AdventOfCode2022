using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 12, "Hot Springs")]
public partial class Day12 : IPuzzle<List<Day12.Spring>>
{
    public partial record Spring(string Pattern, int[] GroupSizes)
    {
        public static Spring Parse(string input)
        {
            var s = input.Split(' ');
            return new Spring(s[0], s[1].Split(',').Select(int.Parse).ToArray());
        }
    }

    public List<Spring> Parse(string rawInput)
    {
        //rawInput = """
        //           ???.### 1,1,3
        //           .??..??...?##. 1,1,3
        //           ?#?#?#?#?#?#?#? 1,3,1,6
        //           ????.#...#... 4,1,1
        //           ????.######..#####. 1,6,5
        //           ?###???????? 3,2,1
        //           """;
        return rawInput.Split(Environment.NewLine).Select(Spring.Parse).ToList();
    }

    public string Part1(List<Spring> input)
    {
        return input
            .Select(x => AmountOfPossiblePermutations(x.Pattern, x.GroupSizes))
            .Sum()
            .ToString();
    }

    public string Part2(List<Spring> input)
    {
        return input
            .Select(FuckMeUp)
            .Select(x => AmountOfPossiblePermutations(x.Pattern, x.GroupSizes))
            .Sum()
            .ToString();
    }

    private Spring FuckMeUp(Spring x)
    {
        var newText = string.Join('?', Enumerable.Repeat(x.Pattern, 5));
        var newRange = Enumerable.Repeat(x.GroupSizes, 5).SelectMany(x => x).ToArray();
        return new Spring(newText, newRange);
    }

    public static int AmountOfPossiblePermutations(string input, int[] groups)
    {
        var indexOfQuestion = input.IndexOf('?');

        if (indexOfQuestion < 0)
        {
            return DoesInputMatch(input, groups) ? 1 : 0;
        }

        var withHash = input[..indexOfQuestion] + '#' + input[(indexOfQuestion + 1)..];
        var withDot = input[..indexOfQuestion] + '.' + input[(indexOfQuestion + 1)..];

        var total = 0;

        if (DoesInputMatch(withHash, groups))
        {
            total += AmountOfPossiblePermutations(withHash, groups);
        }
        if (DoesInputMatch(withDot, groups))
        {
            total += AmountOfPossiblePermutations(withDot, groups);
        }

        return total;
    }

    public static bool DoesInputMatch(ReadOnlySpan<char> input, int[] groups)
    {
        bool isInGroup = false;
        var groupIndex = 0;
        var amountInGroup = 0;
        for(var i = 0; i < input.Length; i++)
        {
            if (input[i] is '?') return true;

            if (input[i] is '.')
            {
                if (isInGroup)
                {
                    if (groups[groupIndex] != amountInGroup) // current group is not big enough
                    {
                        return false;
                    }

                    isInGroup = false;
                    amountInGroup = 0;
                    groupIndex++;
                }
            }

            if (input[i] is '#')
            {
                if (groupIndex == groups.Length) // we closed the last group and found a '#'
                {
                    return false;
                }

                isInGroup = true;
                amountInGroup++;
                if (groups[groupIndex] < amountInGroup) // we've exeeded the current group
                {
                    return false;
                }
            }
        }

        if (groupIndex == groups.Length)
        {
            return true;
        }

        if (groupIndex == groups.Length - 1 && groups[groupIndex] == amountInGroup)
        {
            return true;
        }

        return false;
    }
}