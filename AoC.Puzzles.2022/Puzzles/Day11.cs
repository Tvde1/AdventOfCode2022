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

        return input.Select(x => x.InspectCount).OrderDescending().Take(2).Aggregate(1L, (i, i1) => i * i1).ToString();
    }

    public string Part2(Monkey[] input)
    {
        void ItemHandler(ThrowOperation item) => input[item.ReceiverMonkey].ReceiveItem(item);

        var totalDivThings = input.Select(x => x.DivisibleBy).Aggregate(1, (a, b) => a * b);
        
        foreach (var m in input)
        {
            m.ThrownItems.Subscribe(ItemHandler);
            m.DoWorry = true;
            m.NormalizeWith = totalDivThings;
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

        return input.Select(x => x.InspectCount).OrderDescending().Take(2).Aggregate(1L, (i, i1) => i * i1).ToString();
    }
}

public partial record Monkey
{
    private static readonly Regex MonkeyParseRegex = MonkeyParseGeneratedRegex();

    private readonly Subject<ThrowOperation> _throwSubject = new();
    private readonly Queue<BackpackItem> _items = new();
    private readonly Func<long, long> _riskLevelModifier;
    private readonly Func<long, int> _riskLevelToReceiverMonkey;

    public int Number { get; }
    public int DivisibleBy { get; }

    public IObservable<ThrowOperation> ThrownItems => _throwSubject.AsObservable();
    public long InspectCount { get; private set; }

    public bool DoWorry { get; set; }
    public int? NormalizeWith { get; set; }

    private Monkey(int number,
        IEnumerable<BackpackItem> startingItems,
        Func<long, long> riskLevelModifier,
        Func<long, int> riskLevelToReceiverMonkey,
        int divisibleBy)
    {
        Number = number;
        foreach (var startingItem in startingItems)
        {
            _items.Enqueue(startingItem);
        }

        _riskLevelModifier = riskLevelModifier;
        _riskLevelToReceiverMonkey = riskLevelToReceiverMonkey;
        DivisibleBy = divisibleBy;
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

        if (NormalizeWith is not null)
        {
            currentItem.Normalize(NormalizeWith.Value);
        }

        var receiverMonkey = currentItem.CalculateReceiverMonkey(_riskLevelToReceiverMonkey);
        _throwSubject.OnNext(new ThrowOperation(currentItem, receiverMonkey));
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
        var divisibleBy = int.Parse(match.Groups["divisibleBy"].Value);
        var ifTrue = int.Parse(match.Groups["ifTrue"].Value);
        var ifFalse = int.Parse(match.Groups["ifFalse"].Value);

        int? possibleOperationNumber = int.TryParse(operationNumber, out var a) ? a : null;
        Func<long, long> riskLevelModifier = (operation, possibleOperationNumber) switch
        {
            ("+", null) => x => x + x,
            ("*", null) => x => x * x,
            ("+", { } op) => x => x + op,
            ("*", { } op) => x => x * op,
            _ => throw new MonkeyException("Invalid risk level operation"),
        };

        Func<long, int> riskLevelToReceiverMonkey =
            x => x % divisibleBy == 0 ? ifTrue : ifFalse;

        return new Monkey(number, items, riskLevelModifier, riskLevelToReceiverMonkey, divisibleBy);
    }

    [GeneratedRegex("""
        ^Monkey (?<number>\d+):(?:\r\n|\r|\n).+items: (?<items>.+)(?:\r\n|\r|\n).+= old (?<operation>\+|\*) (?<operationNumber>\-?\d+|old)(?:\r\n|\r|\n).+by (?<divisibleBy>\d+)(?:\r\n|\r|\n).+monkey (?<ifTrue>\d+)(?:\r\n|\r|\n).+monkey (?<ifFalse>\d+)$
        """,
        RegexOptions.Multiline | RegexOptions.Compiled)]
    private static partial Regex MonkeyParseGeneratedRegex();
}

public record BackpackItem
{
    private long RiskLevel { get; set; }

    private BackpackItem(int riskLevel)
    {
        RiskLevel = riskLevel;
    }

    public void ApplyModifier(Func<long, long> modifier) => RiskLevel = modifier(RiskLevel);

    public void CalmDown() => RiskLevel /= 3;

    public void Normalize(int normalizeWith) => RiskLevel %= normalizeWith;
    
    public static BackpackItem Parse(string @in) => new(int.Parse(@in));

    public int CalculateReceiverMonkey(Func<long, int> riskLevelToReceiverMonkey) =>
        riskLevelToReceiverMonkey(RiskLevel);
}

public record ThrowOperation(BackpackItem Item, int ReceiverMonkey);

public class MonkeyException : Exception
{
    public MonkeyException(string message) : base(message)
    {
    }
}