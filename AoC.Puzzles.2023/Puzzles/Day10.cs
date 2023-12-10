using System.Diagnostics;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using Pos = System.ValueTuple<char, System.ValueTuple<int, int>>;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 10, "Pipe Maze")]
public class Day10 : IPuzzle<Day10.Maze>
{
    public record Maze
    {
        const char StartValue = 'S';

        private readonly string _map;

        private Maze(string map)
        {
            _map = map;
            Width = map.AsSpan().IndexOf(Environment.NewLine) + Environment.NewLine.Length;
            Height = map.AsSpan().Count(Environment.NewLine);
        }

        public int Width { get; }
        public int Height { get; }

        public static Maze Parse(string input)
        {
            return new Maze(input);
        }

        public Pos Move(Pos current, Pos from)
        {
            var conn = GetConnectionsForKnownPipe(current);
            return conn.one.Item2 == from.Item2 ? conn.two : conn.one;
        }

        public (char, (int, int)) GetStartPosition()
        {
            var m = _map.Split(Environment.NewLine);
            for (var y = 0; y < m.Length; y++)
            {
                var idx = m[y].IndexOf(StartValue);
                if (idx != -1)
                {
                    return (StartValue, (idx, y));
                }
            }

            throw new UnreachableException();
        }
        
        public (Pos one, Pos two) GetConnectionsForKnownPipe((char p, (int x, int y)) pipe)
        {
            return pipe.p switch
            {
                '|' => (Get((pipe.Item2.x, pipe.Item2.y + 1)), Get((pipe.Item2.x, pipe.Item2.y - 1))),
                '-' => (Get((pipe.Item2.x + 1, pipe.Item2.y)), Get((pipe.Item2.x - 1, pipe.Item2.y))),
                'L' => (Get((pipe.Item2.x, pipe.Item2.y - 1)), Get((pipe.Item2.x + 1, pipe.Item2.y))),
                'J' => (Get((pipe.Item2.x, pipe.Item2.y - 1)), Get((pipe.Item2.x - 1, pipe.Item2.y))),
                '7' => (Get((pipe.Item2.x, pipe.Item2.y + 1)), Get((pipe.Item2.x - 1, pipe.Item2.y))),
                'F' => (Get((pipe.Item2.x, pipe.Item2.y + 1)), Get((pipe.Item2.x + 1, pipe.Item2.y))),
                _ => throw up,
            };
        }


        public (char, (int x, int y)) Get((int x, int y) location)
        {
            var index = location.y * Width + location.x;
            return (_map.AsSpan()[index], location);
        }
    }

    char startChar = '7';
    public Maze Parse(string rawInput)
    {
        // startChar = 'F';
        // rawInput = """
        //            ...........
        //            .S-------7.
        //            .|F-----7|.
        //            .||.....||.
        //            .||.....||.
        //            .|L-7.F-J|.
        //            .|..|.|..|.
        //            .L--J.L--J.
        //            ...........
        //            """;
        return Maze.Parse(rawInput);
    }

    public string Part1(Maze maze)
    {
        return ((GetLine(maze, startChar).Count + 1) / 2).ToString();
    }

    public string Part2(Maze maze)
    {
        var line = GetLine(maze, startChar);
        var count = 0;

        for (var y = 0; y < maze.Height; y++)
        {
            var hasTopPart = false;
            var hasBottomPart = false;
            
            for (var x = 0; x < maze.Width; x++)
            {
                if (!line.Contains((x, y)))
                {
                    if (hasTopPart && hasBottomPart)
                    {
                        count++;
                    }

                    continue;
                }

                var ch = maze.Get((x, y));
                
                switch (ch.Item1)
                {
                    case '|':
                        hasTopPart = !hasTopPart;
                        hasBottomPart = !hasBottomPart;
                        break;
                    case 'L':
                    case 'J':
                        hasTopPart = !hasTopPart;
                        break;
                    case 'F':
                    case '7':
                        hasBottomPart = !hasBottomPart;
                        break;
                }
            }
        }

        return count.ToString();
    }

    public HashSet<(int x, int y)> GetLine(Maze maze, char startChar)
    {
        var start = maze.GetStartPosition();

        var prev = (startChar, start.Item2); // sneaky
        var (current, other) = maze.GetConnectionsForKnownPipe(prev);

        var set = new HashSet<(int, int)>
        {
            prev.Item2,
            current.Item2,
            other.Item2,
        };
        
        while (true)
        {
            var next = maze.Move(current, prev);
            if (next.Item2 == start.Item2)
            {
                return set;
            }

            set.Add(next.Item2);
            prev = current;
            current = next;
        }
    }
}