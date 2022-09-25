﻿using AoC.Common;
using AoC.Common.Models;
using System.Runtime.CompilerServices;

namespace AoC.Puzzles._2019;

[Puzzle(2019, 1, "Calculate fuel")]
public class Day01 : IPuzzle<int[], Day01Input>
{
    public static int[] Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine).Select(int.Parse).ToArray();
    }

    public static string Part1(int[] input)
    {
        var mass = 0;
        foreach (var item in input)
        {
            mass += CalculateFuel(item);
        }

        return mass.ToString();
    }

    public static string Part2(int[] input)
    {
        var mass = 0;
        foreach(var item in input)
        {
            int fuelMass = CalculateFuel(item);

            while (fuelMass > 0)
            {
                mass += fuelMass;
                fuelMass = CalculateFuel(fuelMass);
            }
        }

        return mass.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateFuel(int mass) => mass / 3 - 2;
}

public class Day01Input : IPuzzleInput
{
    public static string Input => @"81157
80969
113477
81295
70537
90130
123804
94276
139327
123719
107814
122142
61204
135309
62810
85750
132568
76450
122948
124649
102644
80055
60517
125884
125708
99051
137158
100450
55239
66758
123848
88711
113047
125528
59285
103978
93047
98038
143019
92031
54353
115597
105629
80411
134966
135473
77357
65776
71096
66926
97853
80349
141914
127221
102492
143587
111493
84711
59826
135652
103334
138211
65088
82244
95011
78760
56691
62070
146134
81650
76904
98838
89629
59950
50390
78616
99731
53831
81273
103980
58485
137684
142457
111050
141916
55567
141945
100794
136425
77911
137114
77450
132048
143066
136805
114135
61565
67286
85512
137493";

}