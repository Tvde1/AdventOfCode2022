using System.Diagnostics;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 2, "Cube Conundrum")]
public class Day02 : IPuzzle<List<Day02.Game>>
{
    public List<Day02.Game> Parse(string inputText)
    {
        // inputText = """
        //             Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
        //             Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
        //             Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
        //             Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
        //             Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
        //             """;
        return inputText.Split(Environment.NewLine).Select(x => Game.Parse(x)).ToList();
    }

    public string Part1(List<Game> input)
    {
        return input
            .Where(MatchesThreshold)
            .Select(x => x.Id)
            .Sum()
            .ToString();

        bool MatchesThreshold(Game game) => game.Sets.All(set => set is { Red: <= 12, Green: <= 13, Blue: <= 14, });
    }

public string Part2(List<Game> input)
{
    return input
        .Select(AddUpSets)
        .Select(MultiplyNumbers)
        .Sum()
        .ToString();

    (int Red, int Blue, int Green) AddUpSets(Game game)
    {
        var red = game.Sets.Max(x => x.Red);
        var blue = game.Sets.Max(x => x.Blue);
        var green = game.Sets.Max(x => x.Green);
        return (red, blue, green);
    }

    int MultiplyNumbers((int Red, int Blue, int Green) numbers)
    {
        return numbers.Red * numbers.Blue * numbers.Green;
    }
}

    public record Set(int Red, int Green, int Blue)
    {
        public static Set Parse(ReadOnlySpan<char> input)
        {
            var red = 0;
            var blue = 0;
            var green = 0;

            while (true)
            {
                var space = input.IndexOf(' ');
                var num = int.Parse(input[..space]);

                var comma = input.IndexOf(',');
                var commaIndex = comma == -1 ? Index.End : comma;

                var color = input[(space + 1)..commaIndex];
                
                switch (color)
                {
                    case "green":
                        green += num;
                        break;
                    case "red":
                        red += num;
                        break;
                    case "blue":
                        blue += num;
                        break;
                }

                if (commaIndex.IsFromEnd)
                {
                    return new Set(red, green, blue);
                }

                input = input[(comma + 2)..]; // space
            }
        }
    }

    public record Game(int Id, List<Set> Sets)
    {
        public static Game Parse(ReadOnlySpan<char> input)
        {
            var indexOfColon = input.IndexOf(':');
            var gameLength = "Game ".Length;

            var gameNum = int.Parse(input[gameLength..indexOfColon]);

            var sets = new List<Set>();

            while (true)
            {
                input = input[(indexOfColon + 2)..];

                indexOfColon = input.IndexOf(';');
                var range = indexOfColon == -1
                    ? input
                    : input[..indexOfColon];

                sets.Add(Set.Parse(range));

                if (indexOfColon == -1)
                {
                    return new Game(gameNum, sets);
                }
            }
        }
    }
} ;