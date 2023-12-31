using System.Text.RegularExpressions;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 12, "Moon gravity")]
public class Day12 : IPuzzle<Moon[]>
{
    public Moon[] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine).Select(Moon.Parse).ToArray();
    }

    public string Part1(Moon[] input)
    {
        for (var i = 0; i < 1000; i++)
        {
            for (var moonIndex = 0; moonIndex < input.Length; moonIndex++)
            {
                for (var otherMoonIndex = 0; otherMoonIndex < input.Length; otherMoonIndex++)
                {
                    if (moonIndex == otherMoonIndex)
                        continue;

                    input[moonIndex].ApplyGravity(input[otherMoonIndex]);
                }
            }

            foreach (var moon in input)
            {
                moon.ApplyVelocity();
            }
        }

        var totalEnergy = input.Sum(moon => moon.CalculateEnergy());

        return totalEnergy.ToString();
    }

    public string Part2(Moon[] input)
    {
        var xRythm = GetRepetitionTime(input[0].Position.X, input[1].Position.X, input[2].Position.X,
            input[3].Position.X);
        var yRythm = GetRepetitionTime(input[0].Position.Y, input[1].Position.Y, input[2].Position.Y,
            input[3].Position.Y);
        var zRythm = GetRepetitionTime(input[0].Position.Z, input[1].Position.Z, input[2].Position.Z,
            input[3].Position.Z);

        var lcm = MyLeastCommonMultipleLol(MyLeastCommonMultipleLol(xRythm, yRythm), zRythm);
        
        return lcm.ToString();
    }

    private static long GetRepetitionTime(int a, int b, int c, int d)
    {
        var originalA = a;
        var originalB = b;
        var originalC = c;
        var originalD = d;

        var velosA = 0;
        var velosB = 0;
        var velosC = 0;
        var velosD = 0;

        var i = 0;
        while (true)
        {
            velosA += -a.CompareTo(b) + -a.CompareTo(c) + -a.CompareTo(d);
            velosB += -b.CompareTo(a) + -b.CompareTo(c) + -b.CompareTo(d);
            velosC += -c.CompareTo(a) + -c.CompareTo(b) + -c.CompareTo(d);
            velosD += -d.CompareTo(a) + -d.CompareTo(b) + -d.CompareTo(c);

            a += velosA;
            b += velosB;
            c += velosC;
            d += velosD;

            i++;

            if (velosA == 0 && velosB == 0 && velosC == 0 && velosD == 0
                && a == originalA && b == originalB && c == originalC && d == originalD)
            {
                return i;
            }
        }
    }

    private static long MyLeastCommonMultipleLol(long a, long b)
    {
        var workA = a;
        var workB = b;

        while (true)
        {
            if (workA < workB)
            {
                workA += a;
            }
            else if (workB < workA)
            {
                workB += b;
            }
            else
            {
                return workA;
            }
        }
    }
}

public class Moon
{
    private static readonly Regex ParseRegex = new(@"^<x=(?<x>-?\d+), y=(?<y>-?\d+), z=(?<z>-?\d+)>$",
        RegexOptions.Compiled);

    public Moon(Vec3D position, Vec3D velocity)
    {
        Position = position;
        Velocity = velocity;
    }

    public Vec3D Position { get; private set; }

    public Vec3D Velocity { get; private set; }

    public int CalculateEnergy()
    {
        return (Math.Abs(Position.X) +
                Math.Abs(Position.Y) +
                Math.Abs(Position.Z)) *
               (Math.Abs(Velocity.X) +
                Math.Abs(Velocity.Y) +
                Math.Abs(Velocity.Z));
    }

    public void ApplyVelocity()
    {
        Position += Velocity;
    }

    public void ApplyGravity(Moon other)
    {
        var x = Position.X.CompareTo(other.Position.X);
        var y = Position.Y.CompareTo(other.Position.Y);
        var z = Position.Z.CompareTo(other.Position.Z);
        Velocity += new Vec3D(-x, -y, -z);
    }

    public static Moon Parse(string input)
    {
        var match = ParseRegex.Match(input);
        return new Moon(
            new Vec3D(
                int.Parse(match.Groups["x"].Value),
                int.Parse(match.Groups["y"].Value),
                int.Parse(match.Groups["z"].Value)),
            Vec3D.Empty);
    }
}

public readonly record struct Vec3D(int X, int Y, int Z)
{
    public static readonly Vec3D Empty = new(0, 0, 0);

    public static Vec3D operator +(Vec3D @this, Vec3D other) =>
        new(@this.X + other.X, @this.Y + other.Y, @this.Z + other.Z);
}