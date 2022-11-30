using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using AoC.Puzzles._2019.Shared;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 13, "Gaming")]
public class Day13 : IPuzzle<long[]>
{
    private const int ScreenWidth = 48;
    private const int ScreenHeight = 24;

    public long[] Parse(string inputText)
    {
        return inputText.Split(',').Select(long.Parse).ToArray();
    }

    public string Part1(long[] input)
    {
        var arcade = new Arcade(input, ScreenWidth, ScreenHeight);

        var output = arcade.PlayStep(null);

        var blockCount = output.Screen.Cast<Tile>().Count(tile => tile == Tile.Block);

        return blockCount.ToString();
    }

    public string Part2(long[] gameCode)
    {
        var wantToPlay = false;
        Console.Write("Do you want to play? (y/n)");
        if (Console.ReadKey().Key == ConsoleKey.Y)
        {
            wantToPlay = true;
        }

        gameCode[0] = 2; // coin hack

        var arcade = new Arcade(gameCode, ScreenWidth, ScreenHeight);

        var input = 0L;

        var score = 0L;
        while (true)
        {
            var output = arcade.PlayStep(input);

            score = output.Score ?? score;

            if (output.HasExited)
            {
                return score.ToString();
            }

            var screen = DrawScreen(output.Screen, score);

            Console.WriteLine(screen);

            if (wantToPlay)
            {
                var key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        input = -1;
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        input = 1;
                        break;
                    default:
                        input = 0;
                        break;
                }
            }
            else
            {
                var ballX = -1;
                var paddleX = -1;

                for (var y = ScreenHeight - 1; y > 0; y--)
                {
                    for (var x = 0; x < ScreenWidth; x++)
                    {
                        if (output.Screen[x, y] == Tile.Ball)
                        {
                            ballX = x;
                            break;
                        }

                        if (output.Screen[x, y] == Tile.HorizontalPaddle)
                        {
                            paddleX = x;
                            break;
                        }
                    }

                    if (ballX != -1 && paddleX != -1)
                    {
                        break;
                    }
                }

                if (ballX < paddleX)
                {
                    input = -1;
                }
                else if (ballX > paddleX)
                {
                    input = 1;
                }
                else
                {
                    input = 0;
                }
                
            }
        }
    }

    private static string DrawScreen(Tile[,] screen, long score)
    {
        var sb = new StringBuilder();

        sb.Append("Score: ");
        sb.AppendLine(score.ToString());

        for (var y = 0; y < screen.GetLength(1); y++)
        {
            for (var x = 0; x < screen.GetLength(0); x++)
            {
                var c = screen[x, y] switch
                {
                    Tile.Empty => ' ',
                    Tile.Wall => '#',
                    Tile.Block => 'X',
                    Tile.HorizontalPaddle => '-',
                    Tile.Ball => 'o',
                    _ => throw new ArgumentOutOfRangeException()
                };
                sb.Append(c);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public enum Tile
{
    Empty = 0,
    Wall = 1,
    Block = 2,
    HorizontalPaddle = 3,
    Ball = 4,
}

public class Arcade
{
    private readonly Computer _computer;

    private readonly Tile[,] canvas;


    public Arcade(long[] memory, int screenWidth, int screenHeight)
    {
        _computer = new Computer(memory);
        canvas = new Tile[screenWidth, screenHeight];
    }

    public (Tile[,] Screen, long? Score, bool HasExited) PlayStep(long? input)
    {
        var hasExited = _computer.ContinueWithInput(input, out var result);

        var drawInstructions = result.Chunk(3)
            .Select(DrawInstruction.Parse);

        var (screen, score) = Draw(drawInstructions);

        return (screen, score, hasExited);
    }

    private (Tile[,], long? Score) Draw(IEnumerable<DrawInstruction> instructions)
    {
        long? score = null;
        foreach (var instruction in instructions)
        {
            if (instruction.X == -1 && instruction.Y == 0)
            {
                score = instruction.Instruction;
            }
            else
            {
                canvas[instruction.X, instruction.Y] = (Tile)instruction.Instruction;
            }
        }

        return (canvas, score);
    }
}

public readonly record struct DrawInstruction(long X, long Y, long Instruction)
{
    public static DrawInstruction Parse(long[] longs)
    {
        return new DrawInstruction(longs[0], longs[1], longs[2]);
    }
}