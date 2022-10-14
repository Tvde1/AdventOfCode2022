using System.Diagnostics;
using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 10, "Asteroid radar")]
public class Day10 : IPuzzle<bool[,]>
{
    public bool[,] Parse(string inputText)
    {
        var split = inputText.Split(Environment.NewLine);

        var b = new bool[split[0].Length, split.Length];

        for (var currY = 0; currY < split.Length; currY++)
        {
            var currX = 0;
            foreach (var cha in split[currY].ToCharArray())
            {
                b[currX, currY] = cha == '#';
                currX++;
            }
        }

        var parsedToString = ToString(b);
        Debug.Assert(inputText == parsedToString);

        return b;
    }

    public string Part1(bool[,] input)
    {
        var (mostVisibleCount, mostVisibleLocation) = GetMostVisible(input);

        return $"{mostVisibleCount} ({mostVisibleLocation.X}, {mostVisibleLocation.Y})";
    }

    public string Part2(bool[,] input)
    {
        var (_, mostVisibleLocation) = GetMostVisible(input);

        var deletedCount = 0;
        var alternateUniverse = (bool[,])input.Clone();

        while (deletedCount < 200)
        {
            var batch = GetPolarCoordinates(alternateUniverse, mostVisibleLocation)
                .ToLookup(x => x.θ)
                .Select(x => x.MinBy(a => a.R))
                .OrderByDescending(x =>
                {
                    if (x.θ < Math.PI / 4)
                    {
                        return Math.PI + x.θ;
                    }
                    
                    return x.θ;
                });

            foreach (var asteroid in batch)
            {
                // boom
                alternateUniverse[asteroid.Location.X, asteroid.Location.Y] = false;
                deletedCount++;

                if (deletedCount == 200)
                {
                    return $"{asteroid.Location.X}{asteroid.Location.Y:D2}";
                }
            }
        }

        return "Universe is wiped";
    }

    private static (int Count, (int X, int Y) Location) GetMostVisible(bool[,] input)
    {
        var mostVisibleCount = 0;
        var mostVisibleLocation = (X: 0, Y: 0);

        for (var x = 0; x < input.GetLength(1); x++)
        {
            for (var y = 0; y < input.GetLength(0); y++)
            {
                if (!input[x, y])
                {
                    continue;
                }

                var location = (x, y);
                var amountVisible = GetPolarCoordinates(input, location)
                    .DistinctBy(a => a.θ)
                    .Count();
                if (amountVisible > mostVisibleCount)
                {
                    mostVisibleLocation = location;
                    mostVisibleCount = amountVisible;
                }
            }
        }

        return (mostVisibleCount, mostVisibleLocation);
    }

    private static IList<((int X, int Y) Location, double R, double θ)> GetPolarCoordinates(bool[,] input, (int x, int y) location)
    {
        var on = new List<((int X, int Y) Location, double R, double θ)>();

        for (var cursorX = 0; cursorX < input.GetLength(1); cursorX++)
        {
            for (var cursorY = 0; cursorY < input.GetLength(0); cursorY++)
            {
                if (input[cursorX, cursorY])
                {
                    var relativeX = cursorX - location.x;
                    var relativeY = cursorY - location.y;

                    if (relativeX == 0 && relativeY == 0)
                    {
                        continue;
                    }

                    var θ = Math.Atan2(relativeX, relativeY);
                    var r = Math.Sqrt(Math.Pow(relativeX, 2) + Math.Pow(relativeY, 2));

                    on.Add(((cursorX, cursorY), r, θ));
                }
            }
        }

        return on;
    }

    private static string ToString(bool[,] input)
    {
        var sb = new StringBuilder();
        for (var y = 0; y < input.GetLength(0); y++)
        {
            for (var x = 0; x < input.GetLength(1); x++)
            {
                sb.Append(input[x, y] ? '#' : '.');
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }
}