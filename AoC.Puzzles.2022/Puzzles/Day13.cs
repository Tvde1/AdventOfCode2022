using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 13, "Distress Signal")]
public class Day13 : IPuzzle<(Packet Left, Packet Right)[]>
{
    public (Packet Left, Packet Right)[] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine + Environment.NewLine)
            .Select(x =>
            {
                var sp = x.AsSpan();
                var lineBreak = sp.IndexOf('\r');
                var left = Packet.Parse(sp[..lineBreak], out _);
                var right = Packet.Parse(sp[(lineBreak + 2)..], out _);

                return (left, right);
            }).ToArray();
    }

    public string Part1((Packet Left, Packet Right)[] input)
    {
        var correctSum = 0;
        for (var index = 0; index < input.Length; index++)
        {
            var p = input[index];
            Debug.WriteLine(p.Left.ToString());
            Debug.WriteLine(p.Right.ToString());
            var comparison = p.Left.CompareTo(p.Right);
            Debug.WriteLine($"Comparison: {comparison}");
            if (comparison <= 0) correctSum += index + 1;
        }

        return correctSum.ToString();
    }

    public string Part2((Packet Left, Packet Right)[] input)
    {
        var ordered = input.SelectMany(x => new[]
            {
                x.Left, x.Right,
            })
            .Append(Packet.Parse("[[2]]", out _))
            .Append(Packet.Parse("[[6]]", out _))
            .Order()
            .ToArray();

        int? firstDividerIndex = null;
        int? secondDividerIndex = null;

        var i = 1;
        foreach (var packet in ordered)
        {
            if (!firstDividerIndex.HasValue && packet.IsDividerOne)
            {
                firstDividerIndex = i;
            }
            else if (packet.IsDividerTwo)
            {
                secondDividerIndex = i;
                break;
            }

            i++;
        }

        Debug.Assert(firstDividerIndex.HasValue);
        Debug.Assert(secondDividerIndex.HasValue);

        return (firstDividerIndex.Value * secondDividerIndex.Value).ToString();
    }
}

public record Packet : IComparable<Packet>
{
    private readonly int? _intValue;
    private readonly Packet[]? _listValue;

    private Packet(int value)
    {
        _intValue = value;
    }

    private Packet(Packet[] value)
    {
        _listValue = value;
    }

    [MemberNotNullWhen(true, nameof(_intValue))]
    [MemberNotNullWhen(false, nameof(_listValue))]
    private bool IsSingleValue => _intValue.HasValue;

    public bool IsDividerOne => _listValue is [{ _listValue: [{ _intValue: 2, },], },];
    public bool IsDividerTwo => _listValue is [{ _listValue: [{ _intValue: 6, },], },];

    private Packet Wrap() => new(new[] { this, });

    public static Packet Parse(ReadOnlySpan<char> span, out int charactersRead)
    {
        charactersRead = 0;
        if (span[0] == '[')
        {
            charactersRead++;
            var l = new List<Packet>();

            while (true)
            {
                if (span[charactersRead] == ']')
                {
                    charactersRead++;
                    return new Packet(l.ToArray());
                }

                var p = Packet.Parse(span[charactersRead..], out var incrementRead);
                l.Add(p);
                charactersRead += incrementRead;

                if (span[charactersRead] == ',')
                {
                    charactersRead++;
                }
            }
        }

        charactersRead = span.IndexOfAny(',', ']', '[');
        return new Packet(int.Parse(span[..charactersRead]));
    }

    public override string ToString()
    {
        return IsSingleValue ? _intValue.Value.ToString() : $"[{string.Join(',', (IEnumerable<Packet>) _listValue)}]";
    }

    public int CompareTo(Packet other)
    {
        if (IsSingleValue && other.IsSingleValue)
        {
            return _intValue.Value.CompareTo(other._intValue.Value);
        }

        if (IsSingleValue && !other.IsSingleValue)
        {
            return Wrap().CompareTo(other);
        }

        if (!IsSingleValue && other.IsSingleValue)
        {
            return CompareTo(other.Wrap());
        }

        Debug.Assert(!IsSingleValue);
        Debug.Assert(!other.IsSingleValue);

        for (var index = 0; index < _listValue.Length; index++)
        {
            var item = _listValue[index];
            if (other._listValue.Length < (index + 1))
            {
                return 1;
            }

            var comparison = item.CompareTo(other._listValue[index]);
            if (comparison != 0)
            {
                return comparison;
            }
        }

        if (_listValue.Length == other._listValue.Length)
        {
            return 0;
        }

        return -1;
    }
}