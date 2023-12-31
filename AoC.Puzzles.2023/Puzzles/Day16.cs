using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 16, "The Floor Will Be Lava")]
public class Day16 : IPuzzle<char[,]>
{
    public char[,] Parse(string rawInput)
    {
        var testRawInput = """
                           .|...\....
                           |.-.\.....
                           .....|-...
                           ........|.
                           ..........
                           .........\
                           ..../.\\..
                           .-.-/..|..
                           .|....-|.\
                           ..//.|....
                           """;

        // rawInput = testRawInput;

        return ParseMap(rawInput);
    }

    private static char[,] ParseMap(string rawInput)
    {
        var split = rawInput.Split(Environment.NewLine);

        var stuff = new char[split[0].Length, split.Length];

        for (var y = 0; y < split.Length; y++)
        {
            for (var x = 0; x < split[0].Length; x++)
            {
                stuff[x, y] = split[y][x];
            }
        }

        return stuff;
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    public string Part1(char[,] input)
    {
        return GetEnergizedCount(input, 0, 0, Direction.Right).ToString();
    }
    
    public string Part2(char[,] input)
    {
        var highestCount = Enumerable.Empty<int>();

        var maxY = input.GetLength(1);
        for (var y = 0; y < maxY; y++)
        {
            highestCount = highestCount.Append(GetEnergizedCount(input, 0, y, Direction.Right));
        }

        var maxX = input.GetLength(0);
        for (var x = 0; x < maxX; x++)
        {
            highestCount = highestCount.Append(GetEnergizedCount(input, x, 0, Direction.Down));
        }
        
        for (var y = 0; y < input.GetLength(1); y++)
        {
            highestCount = highestCount.Append(GetEnergizedCount(input, maxX - 1, y, Direction.Left));
        }
        
        for (var x = 0; x < input.GetLength(0); x++)
        {
            highestCount = highestCount.Append(GetEnergizedCount(input, x, maxY - 1, Direction.Up));
        }

        return highestCount.Max().ToString();
    }

    private int GetEnergizedCount(char[,] input, int startX, int startY, Direction startDirection)
    {
        var cursors = new Queue<(int X, int Y, Direction Direction)>();
        var visited = new HashSet<(int X, int Y, Direction Direction)>();
        var enlightened = new HashSet<(int X, int Y)>();

        cursors.Enqueue((startX, startY, startDirection));

        while (cursors.TryDequeue(out var cursor))
        {
            char current = default;
            try
            {
                current = input[cursor.X, cursor.Y];
            }
            catch
            {
                continue;
            }

            if (!visited.Add(cursor))
            {
                continue;
            }

            enlightened.Add((cursor.X, cursor.Y));


            switch (current)
            {
                case '.':
                {
                    cursors.Enqueue(GoFurther(cursor));
                    break;
                }
                case '/':
                {
                    var newDirection = cursor.Direction switch
                    {
                        Direction.Up => Direction.Right,
                        Direction.Down => Direction.Left,
                        Direction.Left => Direction.Down,
                        Direction.Right => Direction.Up,
                    };
                    cursors.Enqueue(GoFurther((cursor.X, cursor.Y, newDirection)));
                    break;
                }
                case '\\':
                {
                    var newDirection = cursor.Direction switch
                    {
                        Direction.Up => Direction.Left,
                        Direction.Down => Direction.Right,
                        Direction.Left => Direction.Up,
                        Direction.Right => Direction.Down,
                    };
                    cursors.Enqueue(GoFurther((cursor.X, cursor.Y, newDirection)));
                    break;
                }
                case '|':
                {
                    if (cursor.Direction is Direction.Up or Direction.Down)
                    {
                        goto case '.';
                    }

                    cursors.Enqueue(GoFurther((cursor.X, cursor.Y, Direction.Up)));
                    cursors.Enqueue(GoFurther((cursor.X, cursor.Y, Direction.Down)));
                    break;
                }
                case '-':
                {
                    if (cursor.Direction is Direction.Left or Direction.Right)
                    {
                        goto case '.';
                    }

                    cursors.Enqueue(GoFurther((cursor.X, cursor.Y, Direction.Left)));
                    cursors.Enqueue(GoFurther((cursor.X, cursor.Y, Direction.Right)));
                    break;
                }
            }
        }
        //
        // var sb = new StringBuilder();
        // for (var y = 0; y < input.GetLength(1); y++)
        // {
        //     for (var x = 0; x < input.GetLength(0); x++)
        //     {
        //         sb.Append(enlightened.Contains((x, y)) ? '#' : '.');
        //     }
        //
        //     sb.AppendLine();
        // }
        //
        // Console.Write(sb.ToString());

        return enlightened.Count;
    }

    private static (int X, int Y, Direction Direction) GoFurther((int X, int Y, Direction Direction) cursor)
    {
        var nextX = cursor.Direction switch
        {
            Direction.Left => cursor.X - 1,
            Direction.Right => cursor.X + 1,
            _ => cursor.X,
        };
        var nextY = cursor.Direction switch
        {
            Direction.Up => cursor.Y - 1,
            Direction.Down => cursor.Y + 1,
            _ => cursor.Y,
        };

        return (nextX, nextY, cursor.Direction);
    }
}