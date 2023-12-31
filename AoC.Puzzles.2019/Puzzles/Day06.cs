using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 6, "Orbits")]
public class Day06 : IPuzzle<Orbit[]>
{
    public Orbit[] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine).Select(Orbit.Parse).ToArray();
    }

    public string Part1(Orbit[] input)
    {
        var lookup = input.ToLookup(x => x.Center, x => x.Orbiter)
            .ToDictionary(x => x.Key, x => x.ToList());

        (int Count, int AmountOfOrbiters) CountOrbiters(string source)
        {
            if (!lookup.TryGetValue(source, out var orbiters))
            {
                return (0, 0);
            }

            var count = 0;
            var amountOfOrbiters = 0;
            foreach (var orbiter in orbiters)
            {
                amountOfOrbiters++;
                
                var res = CountOrbiters(orbiter);
                amountOfOrbiters += res.AmountOfOrbiters;
                count += res.Count;
            }

            return (count + 1 * amountOfOrbiters, amountOfOrbiters);
        }
        
        return CountOrbiters("COM").Count.ToString();
    }

    public string Part2(Orbit[] input)
    {
        var lookup = input
            .Concat(input.Select(x => new Orbit(x.Orbiter, x.Center)))
            .ToLookup(x => x.Center, x => x.Orbiter)
            .ToDictionary(x => x.Key, x => x.ToList());

        var q = new Queue<(string Center, int StepsCount)>(lookup["YOU"].Select(x => (x, 0)));
        var visited = new HashSet<string>();

        while (q.TryDequeue(out var next))
        {
            visited.Add(next.Center);

            foreach (var nextNext in lookup[next.Center])
            {
                if (nextNext == "SAN")
                {
                    return next.StepsCount.ToString();
                }
                
                if (!visited.Contains(nextNext))
                {
                    q.Enqueue((nextNext, next.StepsCount + 1));
                }
            }
        }
        
        return "No valid path";
    }
}

public record Orbit(string Center, string Orbiter)
{
    public static Orbit Parse(string input)
    {
        var s = input.Split(')');
        return new Orbit(s[0], s[1]);
    }
}