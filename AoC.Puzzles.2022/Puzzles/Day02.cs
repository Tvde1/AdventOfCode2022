﻿using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 2, "Rock Paper Scissors")]
public class Day02 : IPuzzle<(char TheirMove, char MyMove)[]>
{
    public (char TheirMove, char MyMove)[] Parse(string inputText)
    {
        // inputText = """
        //             A Y
        //             B X
        //             C Z
        //             """;
        return inputText.Split(Environment.NewLine).Select(x => (x[0], x[2])).ToArray();
    }

    public string Part1((char TheirMove, char MyMove)[] input)
    {
        return input.Aggregate<(char, char), int>(0, (total, current) => 
            total + current switch
            {
                ('A', 'X') => 1 + 3,
                ('A', 'Y') => 2 + 6,
                ('A', 'Z') => 3 + 0,
                
                ('B', 'X') => 1 + 0,
                ('B', 'Y') => 2 + 3,
                ('B', 'Z') => 3 + 6,
                
                ('C', 'X') => 1 + 6,
                ('C', 'Y') => 2 + 0,
                ('C', 'Z') => 3 + 3,
                _ => throw new ArgumentOutOfRangeException(nameof(current), current, null),
            }).ToString();
    }

    public string Part2((char TheirMove, char MyMove)[] input)
    {
        return input.Aggregate<(char, char), int>(0, (total, current) => 
            total + current switch
            {
                ('A', 'X') => 3 + 0,
                ('A', 'Y') => 1 + 3,
                ('A', 'Z') => 2 + 6,
                
                ('B', 'X') => 1 + 0,
                ('B', 'Y') => 2 + 3,
                ('B', 'Z') => 3 + 6,
                
                ('C', 'X') => 2 + 0,
                ('C', 'Y') => 3 + 3,
                ('C', 'Z') => 1 + 6,
                _ => throw new ArgumentOutOfRangeException(nameof(current), current, null),
            }).ToString();
    }
}