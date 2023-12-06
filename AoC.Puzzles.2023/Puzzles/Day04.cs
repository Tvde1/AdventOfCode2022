using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 4, "Scratchcards")]
public partial class Day04 : IPuzzle<List<Day04.ScratchCard>>
{
    public partial record ScratchCard(int CardNumber, List<int> WinningNumbers, List<int> HaveNumbers)
    {
        public static ScratchCard Parse(string input)
        {
            var matches = Reg().Match(input);
            if (!matches.Success) throw up;

            var gameNum = int.Parse(matches.Groups["cardNo"].Value);
            var winningNumbers = matches.Groups["winning"].Value
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
            var haveNumbers = matches.Groups["have"].Value
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            return new ScratchCard(gameNum, winningNumbers, haveNumbers);
        }

        [GeneratedRegex(@"^Card\s+(?<cardNo>\d+): (?<winning>.+) \| (?<have>.+)$")]
        private static partial Regex Reg();
    }

    public List<ScratchCard> Parse(string rawInput)
    {
        // rawInput = """
        //            Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
        //            Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
        //            Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
        //            Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
        //            Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
        //            Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
        //            """;
        return rawInput.Split(Environment.NewLine).Select(ScratchCard.Parse).ToList();
    }

    public string Part1(List<ScratchCard> input)
    {
        return input.Select(card => 
                card.HaveNumbers.Intersect(card.WinningNumbers)
                    .Aggregate(0L, (scoreForCard, _) => scoreForCard is 0 ? 1 : scoreForCard * 2))
            .Sum()
            .ToString();
    }

    public string Part2(List<ScratchCard> input)
    {
        var pointsByCard = input.ToDictionary(card => card.CardNumber, card => card.HaveNumbers.Intersect(card.WinningNumbers).Count());
        
        var cardAppearing = pointsByCard.ToDictionary(x => x.Key, x => Math.Min(x.Value, 1));
        var cardsWithZeroButStillAppear = cardAppearing.Count(x => x.Value == 0);
        
        foreach(var cardNumber in input.Select(x => x.CardNumber))
        {
            var appearsHowManyTimes = cardAppearing[cardNumber];
            if (appearsHowManyTimes is 0) continue;

            var points = pointsByCard[cardNumber];

            for (var i = 1; i <= points; i++)
            {
                cardAppearing[cardNumber + i] += appearsHowManyTimes;
            }
        }

        return (cardAppearing.Select(x => x.Value).Sum() + cardsWithZeroButStillAppear).ToString();
    }
}