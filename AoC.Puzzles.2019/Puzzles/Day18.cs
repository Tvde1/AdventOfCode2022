using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 18, "Maze")]
public class Day18 : IPuzzle<(Maze, MazeState)>
{
    public (Maze, MazeState) Parse(string inputText)
    {
        return Maze.Parse(inputText);
    }

    public string Part1((Maze, MazeState) input)
    {
        var maze = input.Item1;
        var queue = new PriorityQueue<(MazeState State, int Steps), (int Cost, int Heuristic)>(new CustomHeuristicComparer());

        queue.Enqueue((input.Item2, 0), (0, 0));

        var visited = new HashSet<int>();

        while (queue.TryDequeue(out var state, out _))
        {
            foreach (var permutation in maze.GetMoves(state.State.Cursor.X, state.State.Cursor.Y))
            {
                var newState = state.State.StepTo(permutation);

                if (newState == null)
                {
                    continue;
                }

                if (!visited.Add(newState.GetHash()))
                {
                    continue;
                }

                var newSteps = state.Steps + 1;

                int specialSpotCount = newState.SpecialSpots.Count;
                if (specialSpotCount == 0)
                {
                    return newSteps.ToString();
                }

                queue.Enqueue((newState, newSteps), CreatePriority(state, specialSpotCount));
            }
        }

        return "No valid path";
    }

    private static (int Cost, int Heuristic) CreatePriority((MazeState State, int Steps) state, int specialSpotCount)
    {
        return (state.Steps, specialSpotCount);
    }

    public string Part2((Maze, MazeState) input)
    {
        return "Not implemented";
    }
}

internal sealed class CustomHeuristicComparer : IComparer<(int Cost, int Heuristic)>
{
    public int Compare((int Cost, int Heuristic) x, (int Cost, int Heuristic) y)
    {
        if (x.Cost + x.Heuristic > y.Cost + y.Heuristic)
        {
            return 1;
        }

        if (x.Cost + x.Heuristic < y.Cost + y.Heuristic)
        {
            return -1;
        }

        return x.Cost.CompareTo(y.Cost);
    }
}

public class Maze
{
    public bool[,] Board { get; init; } = null!;

    public IEnumerable<(int X, int Y)> GetMoves(int x, int y)
    {
        if (x + 1 < Board.GetLength(0) && !Board[x + 1, y])
            yield return (x + 1, y);

        if (x > 0 && !Board[x - 1, y])
            yield return (x - 1, y);

        if (y + 1 < Board.GetLength(1) && !Board[x, y + 1])
            yield return (x, y + 1);

        if (y > 0 && !Board[x, y - 1])
            yield return (x, y - 1);
    }

    public static (Maze, MazeState) Parse(string input)
    {
        var split = input.Split(Environment.NewLine);

        var maze = new bool[split[0].Length, split.Length];
        var specialSpots = new Dictionary<(int X, int Y), char>();

        (int X, int Y) cursor = default;

        for (var y = 0; y < split.Length; y++)
        {
            var line = split[y];
            for (var x = 0; x < line.Length; x++)
            {
                var c = line[x];

                if (c == '#')
                {
                    maze[x, y] = true;
                }
                else if (c == '@')
                {
                    cursor = (x, y);
                }
                else if (c != '.')
                {
                    specialSpots.Add((x, y), c);
                }
            }

        }

        return
            (new Maze
            {
                Board = maze,
            },
            new MazeState
            {
                Cursor = cursor,
                SpecialSpots = specialSpots,
            });
    }
}

public record MazeState
{
    public (int X, int Y) Cursor { get; init; }

    public Dictionary<(int X, int Y), char> SpecialSpots { get; init; } = null!;

    public int GetHash()
    {
        var hashCode = 0;

        foreach (var item in SpecialSpots.Values)
        {
            if (char.IsUpper(item))
            {
                hashCode |= 1 << (item - 'A');
            }
        }

        hashCode *= 100_00;
        hashCode += (Cursor.X * 100) + Cursor.Y;

        return hashCode;
    }

    public MazeState? StepTo((int X, int Y) cursor)
    {
        var spotsCopy = new Dictionary<(int X, int Y), char>(SpecialSpots);

        if (spotsCopy.TryGetValue(cursor, out var c))
        {
            if (c >= 'A' && c <= 'Z')
            {
                return null;
            }

            spotsCopy.Remove(cursor);
            var upper = char.ToUpper(c);

            var door = spotsCopy.FirstOrDefault(x => x.Value == upper);
            if (door.Value is not default(char))
            {
                spotsCopy.Remove(door.Key);
            }
        }

        return new MazeState
        {
            Cursor = cursor,
            SpecialSpots = spotsCopy,
        };
    }
}