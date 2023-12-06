using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

// [Puzzle(2023, 25, "asdf")]
public partial class Day25 : IPuzzle<List<Day25.Almanac>>
{
    public partial record Almanac
    {
        public static Almanac Parse(string input)
        {
            return new Almanac();
        }
    }

    public List<Almanac> Parse(string rawInput)
    {
        rawInput = """
                   
                   """;
        return rawInput.Split(Environment.NewLine).Select(Almanac.Parse).ToList();
    }

    public string Part1(List<Almanac> input)
    {
        return "ok";
    }

    public string Part2(List<Almanac> input)
    {
        return "ok";
    }
}