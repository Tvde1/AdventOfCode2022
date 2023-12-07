using System.Diagnostics;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 7, "Camel Cards")]
public partial class Day07 : IPuzzle<List<Day07.Hand>>
{
    public partial record Hand(CardValue[] Values, int Bid)
    {
        public static Hand Parse(string input)
        {
            var split = input.Split(' ');
            return new Hand(
                [ParseValue(split[0][0]), ParseValue(split[0][1]), ParseValue(split[0][2]), ParseValue(split[0][3]), ParseValue(split[0][4]),],
                int.Parse(split[1])
            );
        }

        private static CardValue ParseValue(char v)
        {
            return v switch
            {
                'A' => CardValue.A,
                'K' => CardValue.K,
                'Q' => CardValue.Q,
                'J' => CardValue.J,
                'T' => CardValue.Ten,
                >= '2' and <= '9' => (CardValue)(v - '0'),
                _ => throw up,
            };
        }
    }

    public enum CardValue : byte
    {
        A = 14,
        K = 13,
        Q = 12,
        J = 11,
        Ten = 10,
        Nine = 9,
        Eight = 8,
        Seven = 7,
        Six = 6,
        Five = 5,
        Four = 4,
        Three = 3,
        Two = 2,
    }

    enum HandType
    {
        FiveOfAKind = 1,
        FourOfAKind = 2,
        FullHouse = 3,
        ThreeOfAKind = 4,
        TwoPair = 5,
        OnePair = 6,
        HighCard = 7,
    }

    public List<Hand> Parse(string rawInput)
    {
        //rawInput = """
        //           32T3K 765
        //           T55J5 684
        //           KK677 28
        //           KTJJT 220
        //           QQQJA 483
        //           """;
        return rawInput.Split(Environment.NewLine).Select(Hand.Parse).ToList();
    }

    public string Part1(List<Hand> input)
    {
        var buckets = Enum.GetValues<HandType>()
            .ToDictionary(x => x, x => new SortedSet<Hand>(new LeftToRightComparer()));

        foreach(var hand in input)
        {
            var handType = GetHandType(hand);
            buckets[handType].Add(hand);
        }

        return buckets.OrderBy(x => x.Key)
            .SelectMany(x => x.Value)
            .Aggregate(
                (Index: input.Count, Score: 0L),
                (acc, hand) => (acc.Index - 1, acc.Score + (acc.Index * hand.Bid))
            )
            .Score
            .ToString();
    }

    public string Part2(List<Hand> input)
    {
        var buckets = Enum.GetValues<HandType>()
            .ToDictionary(x => x, x => new SortedSet<Hand>(new LeftToRightJokerComparer()));

        foreach (var hand in input)
        {
            if (hand.Values is [CardValue.A, CardValue.A, CardValue.Nine, CardValue.Nine, CardValue.Nine])
            {
                Debugger.Break();
            }
            var handType = GetHandTypeWithJoker(hand);
            buckets[handType].Add(hand);
        }

        return buckets.OrderBy(x => x.Key)
            .SelectMany(x => x.Value)
            .Aggregate(
                (Index: input.Count, Score: 0L),
                (acc, hand) => (acc.Index - 1, acc.Score + (acc.Index * hand.Bid))
            )
            .Score
            .ToString();
    }

    private static HandType GetHandType(Hand hand)
    {
        var counts = hand.Values.GroupBy(x => x)
            .Select(x => x.Count())
            .OrderDescending()
            .ToList();

        return counts switch
        {
            [5] => HandType.FiveOfAKind,
            [4, 1] => HandType.FourOfAKind,
            [3, 2] => HandType.FullHouse,
            [3, 1, 1] => HandType.ThreeOfAKind,
            [2, 2, 1] => HandType.TwoPair,
            [2, 1, 1, 1] => HandType.OnePair,
            [1, 1, 1, 1, 1] => HandType.HighCard,
            _ => throw new UnreachableException(),
        };
    }

    private static HandType GetHandTypeWithJoker(Hand hand)
    {
        var counts = hand.Values
            .Where(x => x != CardValue.J)
            .GroupBy(x => x)
            .Select(x => x.Count())
            .OrderDescending()
            .ToList();

        return counts switch
        {
            // 5 Cards
            [5] => HandType.FiveOfAKind,
            [4, 1] => HandType.FourOfAKind,
            [3, 2] => HandType.FullHouse,
            [3, 1, 1] => HandType.ThreeOfAKind,
            [2, 2, 1] => HandType.TwoPair,
            [2, 1, 1, 1] => HandType.OnePair,
            [1, 1, 1, 1, 1] => HandType.HighCard,

            // 4 Cards (1 joker)
            [4] => HandType.FiveOfAKind,
            [3, 1] => HandType.FourOfAKind,
            [2, 2] => HandType.FullHouse,
            [2, 1, 1] => HandType.ThreeOfAKind,
            [1, 1, 1, 1] => HandType.OnePair,

            // 3 Cards (2 jokers)
            [3] => HandType.FiveOfAKind,
            [2, 1] => HandType.FourOfAKind,
            [1, 1, 1] => HandType.ThreeOfAKind,

            // 2 Cards (3 jokers)
            [2] => HandType.FiveOfAKind,
            [1, 1] => HandType.FourOfAKind,

            // 1 Card (4 jokers)
            [1] => HandType.FiveOfAKind,

            // All jokers muhahaha
            [] => HandType.FiveOfAKind,

            _ => throw new UnreachableException(),
        };
    }

    internal class LeftToRightComparer : IComparer<Hand>
    {
        public int Compare(Hand? x, Hand? y)
        {
            if (x == null || y == null) throw up;

            for (var i = 0; i < 5; i++)
            {
                var compare = Comparer<CardValue>.Default.Compare(y.Values[i], x.Values[i]);
                if (compare != 0)
                {
                    return compare;
                }
            }

            return 0;
        }
    }

    internal class LeftToRightJokerComparer : IComparer<Hand>
    {
        public int Compare(Hand? x, Hand? y)
        {
            if (x == null || y == null) throw up;

            for (var i = 0; i < 5; i++)
            {
                var compare = Comparer<CardValue>.Default.Compare(y.Values[i], x.Values[i]);

                if (compare == 0)
                {
                    continue;
                }

                if (y.Values[i] == CardValue.J)
                {
                    return -1;
                }

                if (x.Values[i] == CardValue.J)
                {
                    return +1;
                }

                return compare;
            }

            return 0;
        }
    }
}