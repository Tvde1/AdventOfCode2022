using System.Diagnostics;
using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 5, "If You Give A Seed A Fertilizer")]
public class Day05 : IPuzzle<Day05.Almanac>
{
    public record Almanac(long[] Seeds, Dictionary<string, Map> MapsBySource)
    {
        public static Almanac Parse(string input)
        {
            var chunks = input.Split(Environment.NewLine + Environment.NewLine);
            
            var seeds = chunks[0]["seeds: ".Length..].Split(' ').Select(long.Parse).ToArray();

            Dictionary<string, Map> maps = chunks.Skip(1)
                .Select(Map.Parse)
                .ToDictionary(p => p.From);

            return new Almanac(seeds, maps);
        }

        public void Print(StringBuilder builder)
        {
            builder.AppendLine($"Seeds: {string.Join(" ", Seeds)}");

            foreach (var range in MapsBySource.Values)
            {
                range.Print(builder);
            }
            builder.AppendLine();
        }
    }

    public record Map(string From, string To, List<MapRange> Ranges)
    {
        public static Map Parse(string input)
        {
            var lines = input.Split(Environment.NewLine);

            var dashIndex = lines[0].IndexOf('-');
            var mapIndex = lines[0].IndexOf(' ');
            var from = lines[0][..dashIndex];
            var to = lines[0][(dashIndex + "-to-".Length)..mapIndex];

            var mapRanges = lines.Skip(1)
                .Select(MapRange.Parse)
                .OrderBy(x => x.SourceRangeStart)
                .ToList();
            
            return new(from, to, mapRanges);
        }

        public void Print(StringBuilder builder)
        {
            builder.AppendLine($"{From}-to-{To} map:");
            builder.AppendLine();

            foreach(var range in Ranges)
            {
                range.Print(builder);
            }
            builder.AppendLine();
        }
    }

    public record MapRange(long DestinationRangeStart, long SourceRangeStart, long RangeLength)
    {
        public static MapRange Parse(string input)
        {
            var source = input.AsSpan();
            var space = source.IndexOf(' ');
            var first = long.Parse(source[..space]);

            source = source[(space + 1)..];
            
            space = source.IndexOf(' ');
            var second = long.Parse(source[..space]);

            source = source[(space + 1)..];
            var third = long.Parse(source[..]);

            return new MapRange(first, second, third);
        }
        
        public void Print(StringBuilder builder)
        {
            builder.AppendLine($"{DestinationRangeStart} {SourceRangeStart} {RangeLength}");
        }
    }

    public Almanac Parse(string rawInput)
    {
        var temp = """
                   seeds: 79 14 55 13
                   
                   seed-to-soil map:
                   50 98 2
                   52 50 48
                   
                   soil-to-fertilizer map:
                   0 15 37
                   37 52 2
                   39 0 15
                   
                   fertilizer-to-water map:
                   49 53 8
                   0 11 42
                   42 0 7
                   57 7 4
                   
                   water-to-light map:
                   88 18 7
                   18 25 70
                   
                   light-to-temperature map:
                   45 77 23
                   81 45 19
                   68 64 13
                   
                   temperature-to-humidity map:
                   0 69 1
                   1 0 69
                   
                   humidity-to-location map:
                   60 56 37
                   56 93 4
                   """;
        //rawInput = temp;
        return Almanac.Parse(rawInput);
    }

    public string Part1(Almanac input)
    {
        var a = input.Seeds
            .Select(x => ConvertSingle(x, "seed", input))
            .Select(x => ConvertSingle(x, "soil", input))
            .Select(x => ConvertSingle(x, "fertilizer", input))
            .Select(x => ConvertSingle(x, "water", input))
            .Select(x => ConvertSingle(x, "light", input))
            .Select(x => ConvertSingle(x, "temperature", input))
            .Select(x => ConvertSingle(x, "humidity", input))
            .ToList();

        return a.Min().ToString();
    }

    public string Part2(Almanac input)
    {
        var c = input.Seeds.Chunk(2)
            .Select(x => new SeedRange(x[0], x[1]))
            //.WriteLine("start")
            .SelectMany(x => ConvertRange(x, "seed", input))
            //.WriteLine("seed-to-soil")
            .SelectMany(x => ConvertRange(x, "soil", input))
            //.WriteLine("soil-to-fertilizer")
            .SelectMany(x => ConvertRange(x, "fertilizer", input))
            //.WriteLine("fertilizer-to-water")
            .SelectMany(x => ConvertRange(x, "water", input))
            //.WriteLine("water-to-light")
            .SelectMany(x => ConvertRange(x, "light", input))
            //.WriteLine("light-to-temperature")
            .SelectMany(x => ConvertRange(x, "temperature", input))
            //.WriteLine("temperature-to-humidity")
            .SelectMany(x => ConvertRange(x, "humidity", input))
            //.WriteLine("humidity-to-location")
            .ToList();
        return c.Select(x => x.From).Min().ToString();
    }

    private void Compare(List<long> longs, List<SeedRange> seedRanges)
    {
        var stackTrace = new StackTrace(true);
        var aaa = seedRanges.SelectMany(x => Enumerable.Range((int)x.From, (int)x.Length)).ToList();

        if (!aaa.SequenceEqual(longs.Select(x => (int)x)))
        {
            Console.WriteLine($"Uh uh. {stackTrace.GetFrame(1).GetFileLineNumber()} Expected:");
            Console.WriteLine("  " + string.Join(" ", longs));
            Console.WriteLine("Received:");
            Console.WriteLine("  " + string.Join(" ", aaa));
        }
    }

    private static long ConvertSingle(long item, string from, Almanac almanac)
    {
        var map = almanac.MapsBySource[from];

        foreach (var range in map.Ranges)
        {
            if (range.SourceRangeStart <= item)
            {
                var diff = item - range.SourceRangeStart;
                if (diff < range.RangeLength)
                {
                    return range.DestinationRangeStart + diff;
                }
            }
        }

        return item;
    }

    public record SeedRange(long From, long Length);

    private static IEnumerable<SeedRange> ConvertAndCompareMethodsRange(SeedRange range, string from, Almanac almanac)
    {
        var internalResult = ConvertRange(range, from, almanac).ToList();
        var naiveResult = Enumerable.Range((int) range.From, (int) range.Length)
            .Select(x => ConvertSingle(x, from, almanac)).ToList();

        var internalResultSeperated = internalResult.SelectMany(x => Enumerable.Range((int)x.From, (int)x.Length))
            .Select(x => (long)x);

        if (!naiveResult.SequenceEqual(internalResultSeperated))
        {
            Console.WriteLine($"Uh uh. {from}-to-");
            Console.WriteLine($" Expected:");
            Console.WriteLine("  " + string.Join(" ", naiveResult));
            Console.WriteLine("Received:");
            Console.WriteLine("  " + string.Join(" ", internalResultSeperated));
        }

        return internalResult;
    }


    private static IEnumerable<SeedRange> ConvertRange(SeedRange range, string from, Almanac almanac)
    {
        var map = almanac.MapsBySource[from];
        
        var currentMapRange = GetRangeWhichCurrentIndexStartsIn(range, map);
        while (true)
        {
            if (currentMapRange is null)
            {
                var nextMapRange = GetNextRange(range, map);

                if (nextMapRange is null)
                {
                    yield return range;
                    yield break;
                }

                var itemsUntilNextMapRange = nextMapRange.SourceRangeStart - range.From;

                if (range.Length <= itemsUntilNextMapRange)
                {
                    yield return range;
                    yield break;
                }

                (var rangeUntil, range) = ChopRange(range, itemsUntilNextMapRange);
                yield return rangeUntil;
                currentMapRange = nextMapRange;
                continue;
            }
            else
            {
                var amountCanStillFit = (currentMapRange.SourceRangeStart + currentMapRange.RangeLength) - range.From;

                if (amountCanStillFit >= range.Length)
                {
                    yield return range with
                    {
                        From = range.From +
                            (currentMapRange.DestinationRangeStart - currentMapRange.SourceRangeStart),
                    };
                    yield break;
                }

                (var rangeUntil, range) = ChopRange(range, amountCanStillFit);
                yield return rangeUntil with
                {
                    From = rangeUntil.From +
                        (currentMapRange.DestinationRangeStart - currentMapRange.SourceRangeStart),
                };
                currentMapRange = GetRangeWhichCurrentIndexStartsIn(range, map);

                continue;
            }
        }
    }

    // Input: [5, len: 20] 10
    // Output: (5: len 10, 15: len 10)
    private static (SeedRange Left, SeedRange Right) ChopRange(SeedRange range, long leftLength)
    {
        var result = (Left: range, Right: range);
        result.Left = range with
        {
            Length = leftLength,
        };
        result.Right = new SeedRange(range.From + leftLength,
            range.Length - leftLength);
        
        return result;
    }

    private static MapRange? GetRangeWhichCurrentIndexStartsIn(SeedRange range, Map map)
    {
        return map.Ranges
            .FirstOrDefault(x => 
                x.SourceRangeStart <= range.From &&
                x.SourceRangeStart + x.RangeLength > range.From);
    }
    
    private static MapRange? GetNextRange(SeedRange range, Map map)
    {
        return map.Ranges.FirstOrDefault(x => x.SourceRangeStart > range.From);
    }
}

public static class e
{
    public static IEnumerable<Day05.SeedRange> WriteLine(
        this IEnumerable<Day05.SeedRange> source,
        string from)
    {
        var l = source.ToList();
        Console.WriteLine($"Printing seed range: {from}");
        foreach (var range in l)
        {
            Console.WriteLine($"  {range.From} ({range.Length})");
        }

        return l;
    }
}