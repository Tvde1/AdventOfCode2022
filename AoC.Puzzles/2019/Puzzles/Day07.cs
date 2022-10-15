using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using AoC.Puzzles._2019.Shared;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 7, "Amplification")]
public class Day07 : IPuzzle<int[]>
{
    public int[] Parse(string inputText)
    {
        return inputText.Split(',').Select(int.Parse).ToArray();
    }

    public string Part1(int[] input)
    {
        var range = Enumerable.Range(0, 5).ToArray();
        const int count = 4 + 3 + 2 + 1 + 0;

        var highest = long.MinValue;
        var signal = string.Empty;

        foreach (var a in range)
        {
            foreach (var b in range)
            {
                if (b == a) continue;
                foreach (var c in range)
                {
                    if (c == a || c == b) continue;
                    foreach (var d in range)
                    {
                        if (d == a || d == b || d == c) continue;
                        var e = count - a - b - c - d;

                        var memA = (int[])input.Clone();
                        var memB = (int[])input.Clone();
                        var memC = (int[])input.Clone();
                        var memD = (int[])input.Clone();
                        var memE = (int[])input.Clone();

                        var ampA = new Computer(memA);
                        var ampB = new Computer(memB);
                        var ampC = new Computer(memC);
                        var ampD = new Computer(memD);
                        var ampE = new Computer(memE);

                        var outA = ampA.Execute(new long[] { a, 0 });
                        var outB = ampB.Execute(new [] { b, outA[0] });
                        var outC = ampC.Execute(new [] { c, outB[0] });
                        var outD = ampD.Execute(new [] { d, outC[0] });
                        var outE = ampE.Execute(new [] { e, outD[0] });

                        var num = outE[0];

                        if (num > highest)
                        {
                            highest = num;
                            signal = $"{a}{b}{c}{d}{e}";
                        }
                    }
                }
            }
        }

        return $"{highest} ({signal})";
    }

    public string Part2(int[] input)
    {
        var range = Enumerable.Range(5, 5).ToArray();
        const int count = 5 + 6 + 7 + 8 + 9;

        var highest = long.MinValue;
        var signal = string.Empty;

        foreach (var a in range)
        {
            foreach (var b in range)
            {
                if (b == a) continue;
                foreach (var c in range)
                {
                    if (c == a || c == b) continue;
                    foreach (var d in range)
                    {
                        if (d == a || d == b || d == c) continue;
                        var e = count - a - b - c - d;

                        var memA = (int[])input.Clone();
                        var memB = (int[])input.Clone();
                        var memC = (int[])input.Clone();
                        var memD = (int[])input.Clone();
                        var memE = (int[])input.Clone();

                        var ampA = new Computer(memA);
                        var ampB = new Computer(memB);
                        var ampC = new Computer(memC);
                        var ampD = new Computer(memD);
                        var ampE = new Computer(memE);

                        ampA.ContinueWithInput(a, out _);
                        ampB.ContinueWithInput(b, out _);
                        ampC.ContinueWithInput(c, out _);
                        ampD.ContinueWithInput(d, out _);
                        ampE.ContinueWithInput(e, out _);

                        long inA = 0;
                        while (true)
                        {
                            ampA.ContinueWithInput(inA, out var outA);
                            ampB.ContinueWithInput(outA[0], out var outB);
                            ampC.ContinueWithInput(outB[0], out var outC);
                            ampD.ContinueWithInput(outC[0], out var outD);
                            var isExited = ampE.ContinueWithInput(outD[0], out var outE);
                            inA = outE[0];
                            if (isExited) break;
                        }

                        if (inA > highest)
                        {
                            highest = inA;
                            signal = $"{a}{b}{c}{d}{e}";
                        }
                    }
                }
            }
        }

        return $"{highest} ({signal})";
    }
}