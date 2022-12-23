using System.Numerics;

namespace AoC.Common.Algorithms;

public static class Dijstra
{
    public static (TState State, TCost Cost) GetShortestPath<TState, TCost>(
        TState start,
        Func<TState, bool> isGoal,
        Func<TState, IEnumerable<TState>> getNeighbors,
        Func<TState, TCost> getCost)
        where TCost : INumber<TCost>
    {
        return AStar.GetShortestPath<TState, TCost>(start, isGoal, getNeighbors, getCost, _ => TCost.Zero);
    }
}

public static class AStar
{
    public static (TState State, TCost Cost) GetShortestPath<TState, TCost>(
        TState start,
        Func<TState, bool> isGoal,
        Func<TState, IEnumerable<TState>> getNeighbors,
        Func<TState, TCost> getCost,
        Func<TState, TCost> getHeuristic,
        IComparer<TCost>? comparer = null)
        where TCost : INumber<TCost>
    {
        var queue = new PriorityQueue<TState, TCost>(comparer ?? Comparer<TCost>.Default);

        TCost GetHeuristicCost(TState state) => getCost(state) + getHeuristic(state);

        queue.Enqueue(start, GetHeuristicCost(start));

        var visited = new HashSet<TState>();

        while (queue.Count > 0)
        {
            var state = queue.Dequeue();
            if (visited.Contains(state))
                continue;

            if (isGoal(state))
            {
                return (state, getCost(state));
            }

            visited.Add(state);

            foreach (var neighbor in getNeighbors(state))
            {
                queue.Enqueue(neighbor, GetHeuristicCost(neighbor));
            }
        }

        throw new InvalidOperationException("No path found.");
    }
}