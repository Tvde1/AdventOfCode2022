using System.Text;
using System.Text.RegularExpressions;
using AoC.Common.Algorithms;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 16, "Proboscidea Volcanium")]
public class Day16 : IPuzzle<Valve>
{
    public Valve Parse(string inputText)
    {
        inputText = """
            Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
            Valve BB has flow rate=13; tunnels lead to valves CC, AA
            Valve CC has flow rate=2; tunnels lead to valves DD, BB
            Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
            Valve EE has flow rate=3; tunnels lead to valves FF, DD
            Valve FF has flow rate=0; tunnels lead to valves EE, GG
            Valve GG has flow rate=0; tunnels lead to valves FF, HH
            Valve HH has flow rate=22; tunnel leads to valve GG
            Valve II has flow rate=0; tunnels lead to valves AA, JJ
            Valve JJ has flow rate=21; tunnel leads to valve II
            """;
        return Valve.ParseGraph(inputText);
    }

    public string Part1(Valve input)
    {
        var totalTime = 30;

        var beginState = new State
        {
            CurrentValve = input,
            OnValves = new(),
            TimeLeft = totalTime,
            CurrentFlowRate = 0,
            TotalFlow = 0,
        };

        var (state, cost) = AStar.GetShortestPath(beginState, 
            isGoal: s => s.TimeLeft == 0,
            getNeighbors: s => s.VisitNeighbors().Concat(s.OpenValve()),
            getCost: s => -s.TotalFlow,
            getHeuristic: s => (s.TimeLeft * s.CurrentFlowRate));
        
        return state.TotalFlow.ToString();

        return "Not implemented";
    }

    public string Part2(Valve input)
    {
        return "Not implemented";
    }
}

public class HigherCostIsBetterComparer : IComparer<int>
{
    public int Compare(int x, int y)
    {
        return x.CompareTo(y);
    }
}

public partial record Valve(string Name, int FlowRate, List<Valve> Connections)
{
    private static readonly Regex _valveRegex = ValveRegex();

    public static Valve ParseGraph(string input)
    {
        var parsed = input.Split(Environment.NewLine).Select(x =>
        {
            var match = _valveRegex.Match(x);
            var name = match.Groups["name"].Value;
            var flowRate = int.Parse(match.Groups["flowRate"].Value);
            var connections = match.Groups["connections"].Value.Split(", ").ToList();
            return new
            {
                Name = name,
                FlowRate = flowRate,
                Connections = connections,
            };
        }).ToList();

        var valves = parsed.ToDictionary(p => p.Name, p => new Valve(p.Name, p.FlowRate, new List<Valve>()));

        foreach (var p in parsed)
        {
            var valve = valves[p.Name];
            foreach (var connection in p.Connections)
            {
                valve.Connections.Add(valves[connection]);
            }
        }

        return valves["AA"];
    }

    [GeneratedRegex(
        @"Valve (?<name>\w+) has flow rate=(?<flowRate>\d+); tunnels? leads? to valves? (?<connections>(?:\w+(?:, )?)+)")]
    private static partial Regex ValveRegex();
}

public record State
{
    public HashSet<string> OnValves { get; init; }

    public Valve CurrentValve { get; init; }
    public bool IsCurrentValveOpen => OnValves.Contains(CurrentValve.Name);
    
    public int CurrentFlowRate { get; init; }
    public int TotalFlow { get; init; }
    
    public int TimeLeft { get; init; }

    public IEnumerable<State> OpenValve()
    {
        if (TimeLeft == 0)
        {
            throw new InvalidOperationException("time's up bro");
        }

        if (IsCurrentValveOpen)
        {
            yield break;
        }

        yield return this with
        {
            OnValves = new HashSet<string>(OnValves)
            {
                CurrentValve.Name,
            },
            TimeLeft = TimeLeft - 1,
            TotalFlow = CurrentFlowRate + TotalFlow,
            CurrentFlowRate = CurrentFlowRate + CurrentValve.FlowRate,
        };
    }

    public IEnumerable<State> VisitNeighbors() =>
        TimeLeft == 0
            ? throw new InvalidOperationException("time's up bro")
            : CurrentValve.Connections.Select(n => this with
            {
                CurrentValve = n,
                TimeLeft = TimeLeft - 1,
                TotalFlow = CurrentFlowRate + TotalFlow,
            });
}