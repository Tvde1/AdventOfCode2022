using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2023.Puzzles;

[Puzzle(2023, 8, "Haunted Wasteland")]
public partial class Day08 : IPuzzle<Day08.Maze>
{
    public partial record Maze(string Path, Dictionary<string, (string Left, string Right)> Network)
    {
        public static Maze Parse(string input)
        {
            var lines = input.Split(Environment.NewLine);

            var network = lines.Skip(2)
                .Select(x => (from: x[0..3], left: x[7..10], right: x[12..15]))
                .ToDictionary(x => x.from, x => (x.left, x.right));

            return new Maze(lines[0], network);
        }
    }

    public Maze Parse(string rawInput)
    {
         rawInput = """
                    LR

                    11A = (11B, XXX)
                    11B = (XXX, 11Z)
                    11Z = (11B, XXX)
                    22A = (22B, XXX)
                    22B = (22C, 22C)
                    22C = (22Z, 22Z)
                    22Z = (22B, 22B)
                    XXX = (XXX, XXX)
                    """;
        return Maze.Parse(rawInput);
    }

    public string Part1(Maze input)
    {
        return "bla";
        string currentLocation = "AAA";
        const string targetLocation = "ZZZ";

        int currentSteps = 0;

        int spotInPattern = 0;

        while (true)
        {
            var currentChoice = input.Network[currentLocation];
            var command = input.Path[spotInPattern];

            currentLocation = command switch
            {
                'L' => currentChoice.Left,
                'R' => currentChoice.Right,
                _ => throw up,
            };
            currentSteps++;

            if (currentLocation == targetLocation)
            {
                break;
            }

            spotInPattern++;
            if (spotInPattern == input.Path.Length)
            {
                spotInPattern = 0;
            }
        }


        return currentSteps.ToString();
    }

    public string Part2(Maze input)
    {
        var positions = input.Network.Keys.Where(x => x.EndsWith('A'))
            .Select((x, i) => (GhostIndex: i, Current: x))
            .ToList();

        var startPositions = positions.ToHashSet();

        var reachZPoints = input.Network.Keys.Where(x => x.EndsWith('Z'))
            .ToDictionary(x => x, x => new List<(int GhostIndex, int Count)>());

        var totalCycleCountPerGhostUntilRepeat = new Dictionary<int, int>();
        
        int currentSteps = 0;
        int spotInPattern = 0;

        var hasMoved = false;

        while (true)
        {
            var command = input.Path[spotInPattern];
            
            var currentLocationsCopy = new List<(int GhostIndex, string Current)>();
            foreach (var ghost in positions)
            {
                var currentChoice = input.Network[ghost.Current];

                var newLocation = command switch
                {
                    'L' => currentChoice.Left,
                    'R' => currentChoice.Right,
                    _ => throw up,
                };

                if (newLocation.EndsWith('Z'))
                {
                    reachZPoints[newLocation].Add((ghost.GhostIndex, currentSteps + 1));
                }
                
                currentLocationsCopy.Add((ghost.GhostIndex, newLocation));
            }

            currentSteps++;

            if(currentLocationsCopy.Count == 0)
            {
                break;
            }

            positions = currentLocationsCopy;
            spotInPattern++;
            if (spotInPattern == input.Path.Length)
            {
                spotInPattern = 0;
            }

            hasMoved = true;
        }

        return currentSteps.ToString();
    }
}