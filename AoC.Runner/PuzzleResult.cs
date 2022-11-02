using AoC.Common.Models;

namespace AoC.Runner;

public record PuzzleResult(PuzzleModel Puzzle, string Part1, string Part2, long ElapsedMsPart1, long ElapsedMsPart2);
