using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 11, "Monkey in the Middle")]
public class Day11 : IPuzzle<Monkey[]>
{
    public Monkey[] Parse(string inputText)
    {
        inputText = Example.Text;
        return inputText.Split(Environment.NewLine + Environment.NewLine)
            .Select(Monkey.Parse).ToArray();
    }

    public string Part1(Monkey[] input)
    {
        void ItemHandler(ThrowOperation item) => input[item.ReceiverMonkey].ReceiveItem(item);

        foreach (var m in input)
        {
            m.ThrownItems.Subscribe(ItemHandler);
        }

        for (var roundNumber = 0; roundNumber < 20; roundNumber++)
        {
            foreach (var monkey in input)
            {
                while (monkey.ThrowNext())
                {
                }
            }
        }

        return input.Select(x => x.InspectCount).OrderDescending().Take(2).Aggregate(1UL, (i, i1) => i * i1).ToString();
    }

    public string Part2(Monkey[] input)
    {
        void ItemHandler(ThrowOperation item) => input[item.ReceiverMonkey].ReceiveItem(item);

        foreach (var m in input)
        {
            m.ThrownItems.Subscribe(ItemHandler);
            m.DoWorry = true;
        }

        for (var roundNumber = 0; roundNumber < 10_000; roundNumber++)
        {
            foreach (var monkey in input)
            {
                while (monkey.ThrowNext())
                {
                }
            }
        }

        return input.Select(x => x.InspectCount).OrderDescending().Take(2).Aggregate(1UL, (i, i1) => i * i1).ToString();
    }
}

public partial record Monkey
{
    private static readonly Regex MonkeyParseRegex = MonkeyParseGeneratedRegex();

    private readonly Subject<ThrowOperation> _throwSubject = new();
    private readonly Queue<BackpackItem> _items = new();
    private readonly Func<ulong, ulong> _riskLevelModifier;
    private readonly Func<ulong, int> _riskLevelToReceiverMonkey;

    public int Number { get; }
    public IObservable<ThrowOperation> ThrownItems => _throwSubject.AsObservable();
    public ulong InspectCount { get; private set; }
    public bool DoWorry { get; set; }

    private Monkey(int number,
        IEnumerable<BackpackItem> startingItems,
        Func<ulong, ulong> riskLevelModifier,
        Func<ulong, int> riskLevelToReceiverMonkey)
    {
        Number = number;
        foreach (var startingItem in startingItems)
        {
            _items.Enqueue(startingItem);
        }

        _riskLevelModifier = riskLevelModifier;
        _riskLevelToReceiverMonkey = riskLevelToReceiverMonkey;
    }

    public bool ThrowNext()
    {
        if (!_items.TryDequeue(out var currentItem))
        {
            return false;
        }

        currentItem.ApplyModifier(_riskLevelModifier);
        InspectCount++;

        if (!DoWorry)
        {
            currentItem.CalmDown();
        }

        var newRiskLevelToReceiverMonkey = currentItem.CalculateReceiverMonkey(_riskLevelToReceiverMonkey);
        _throwSubject.OnNext(new ThrowOperation(currentItem, newRiskLevelToReceiverMonkey));
        return true;
    }

    public void ReceiveItem(ThrowOperation @throw)
    {
        Debug.Assert(@throw.ReceiverMonkey == Number);
        _items.Enqueue(@throw.Item);
    }

    public static Monkey Parse(string input)
    {
        var match = MonkeyParseRegex.Match(input);
        if (!match.Success)
        {
            throw new MonkeyException("Could not parse monkey");
        }

        var number = int.Parse(match.Groups["number"].Value);
        var items = match.Groups["items"].Value.Split(", ").Select(BackpackItem.Parse);
        var operation = match.Groups["operation"].Value;
        var operationNumber = match.Groups["operationNumber"].Value;
        var divisibleBy = uint.Parse(match.Groups["divisibleBy"].Value);
        var ifTrue = int.Parse(match.Groups["ifTrue"].Value);
        var ifFalse = int.Parse(match.Groups["ifFalse"].Value);

        ulong? possibleOperationNumber = ulong.TryParse(operationNumber, out var a) ? a : null;
        Func<ulong, ulong> riskLevelModifier = (operation, possibleOperationNumber) switch
        {
            ("+", null) => x => x + x,
            ("*", null) => x => x * x,
            ("+", { } op) => x => x + op,
            ("*", { } op) => x => x * op,
            _ => throw new MonkeyException("Invalid risk level operation"),
        };

        Func<ulong, int> riskLevelToReceiverMonkey =
            x => x % divisibleBy == 0 ? ifTrue : ifFalse;

        return new Monkey(number, items, riskLevelModifier, riskLevelToReceiverMonkey);
    }

    [GeneratedRegex("""
        ^Monkey (?<number>\d+):(?:\r\n|\r|\n).+items: (?<items>.+)(?:\r\n|\r|\n).+= old (?<operation>\+|\*) (?<operationNumber>\-?\d+|old)(?:\r\n|\r|\n).+by (?<divisibleBy>\d+)(?:\r\n|\r|\n).+monkey (?<ifTrue>\d+)(?:\r\n|\r|\n).+monkey (?<ifFalse>\d+)$
        """,
        RegexOptions.Multiline | RegexOptions.Compiled)]
    private static partial Regex MonkeyParseGeneratedRegex();
}

public record BackpackItem
{
    private ulong RiskLevel { get; set; }

    private BackpackItem(ulong riskLevel)
    {
        RiskLevel = riskLevel;
    }

    public void ApplyModifier(Func<ulong, ulong> modifier) => RiskLevel = modifier(RiskLevel);

    public void CalmDown() => RiskLevel /= 3;

    public static BackpackItem Parse(string @in) => new(ulong.Parse(@in));

    public int CalculateReceiverMonkey(Func<ulong, int> riskLevelToReceiverMonkey) =>
        riskLevelToReceiverMonkey(RiskLevel);
}

public record ThrowOperation(BackpackItem Item, int ReceiverMonkey);

public class MonkeyException : Exception
{
    public MonkeyException(string message) : base(message)
    {
    }
}

file class Example
{
    public const string Text = """
Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1
""";
}