using System.Text;
using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 15, "Beacon Exclusion Zone")]
public class Day15 : IPuzzle<SensorData[]>
{
    private const int Part1YLevel = 10;
    
    public SensorData[] Parse(string inputText)
    {
        inputText = """
            Sensor at x=2, y=18: closest beacon is at x=-2, y=15
            Sensor at x=9, y=16: closest beacon is at x=10, y=16
            Sensor at x=13, y=2: closest beacon is at x=15, y=3
            Sensor at x=12, y=14: closest beacon is at x=10, y=16
            Sensor at x=10, y=20: closest beacon is at x=10, y=16
            Sensor at x=14, y=17: closest beacon is at x=10, y=16
            Sensor at x=8, y=7: closest beacon is at x=2, y=10
            Sensor at x=2, y=0: closest beacon is at x=2, y=10
            Sensor at x=0, y=11: closest beacon is at x=2, y=10
            Sensor at x=20, y=14: closest beacon is at x=25, y=17
            Sensor at x=17, y=20: closest beacon is at x=21, y=22
            Sensor at x=16, y=7: closest beacon is at x=15, y=3
            Sensor at x=14, y=3: closest beacon is at x=15, y=3
            Sensor at x=20, y=1: closest beacon is at x=15, y=3
            """;
        return inputText.Split(Environment.NewLine).Select(SensorData.Parse).ToArray();
    }

    public string Part1(SensorData[] input)
    {
        
        
        var sb = Print(input);

        return sb;
    }

    public string Part2(SensorData[] input)
    {
        return "Not implemented";
    }
    

    private static string Print(SensorData[] input)
    {
        var allPoints = input.SelectMany(x => new[]
        {
            new
            {
                Point = x.SensorPoint,
                C = 'S',
            },
            new
            {
                Point = x.ClosestBeaconPoint,
                C = 'B',
            }
        }).ToArray();

        var minX = allPoints.Min(x => x.Point.X);
        var maxX = allPoints.Max(x => x.Point.X);
        var minY = allPoints.Min(x => x.Point.Y);
        var maxY = allPoints.Max(x => x.Point.Y);

        var sb = new StringBuilder();

        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                var point = (x, y);
                var pointData = allPoints.FirstOrDefault(x => x.Point == point);
                sb.Append(pointData?.C ?? '.');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public partial record SensorData((int X, int Y) SensorPoint, (int X, int Y) ClosestBeaconPoint)
{
    private static readonly Regex _sensorRegex = SensorRegex();

    public static SensorData Parse(string input)
    {
        var match = _sensorRegex.Match(input);
        var sensorPoint = (int.Parse(match.Groups["sensorX"].Value), int.Parse(match.Groups["sensorY"].Value));
        var closestBeaconPoint = (int.Parse(match.Groups["beaconX"].Value), int.Parse(match.Groups["beaconY"].Value));
        return new SensorData(sensorPoint, closestBeaconPoint);
    }

    [GeneratedRegex(@"Sensor at x=(?<sensorX>\-?\d+), y=(?<sensorY>\-?\d+): closest beacon is at x=(?<beaconX>\-?\d+), y=(?<beaconY>\-?\d+)", RegexOptions.Compiled)]
    private static partial Regex SensorRegex();
}