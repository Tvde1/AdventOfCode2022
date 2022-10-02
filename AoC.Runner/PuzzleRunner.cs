using AoC.Common;
using AoC.Common.Models;
using AoC.Puzzles._2019;
using BenchmarkDotNet.Running;
using System.Reflection;

namespace AoC.Runner;

public class PuzzleRunner : IPuzzleRunner
{
    private readonly IReadOnlyList<PuzzleModel> _puzzles;
    private readonly MethodInfo _runMethod;
    private readonly MethodInfo _benchmarkMethod;

    public PuzzleRunner()
    {
        _puzzles = GetAllPuzzles();
        _runMethod = GetType().GetMethod(nameof(RunPuzzle), BindingFlags.Static | BindingFlags.NonPublic)!;
        _benchmarkMethod = GetType().GetMethod(nameof(RunBenchmark), BindingFlags.Static | BindingFlags.NonPublic)!;
    }

    public IReadOnlyCollection<PuzzleModel> GetPuzzles()
    {
        return _puzzles;
    }

    public PuzzleResult RunPuzzle(PuzzleModel puzzle)
    {
        var method = _runMethod.MakeGenericMethod(puzzle.PuzzleType, puzzle.ParsedType, puzzle.InputType);

        return (PuzzleResult) method.Invoke(null, new object[] { puzzle })!;
    }

    public void BenchmarkPuzzle(PuzzleModel puzzle)
    {
        var method = _benchmarkMethod.MakeGenericMethod(puzzle.PuzzleType, puzzle.ParsedType, puzzle.InputType);

        method.Invoke(null, null);
    }

    private static void RunBenchmark<TPuzzle, TParsed, TInput>()
        where TPuzzle : IPuzzle<TParsed, TInput>
        where TInput : IPuzzleInput
    {
        var summary = BenchmarkRunner.Run<PuzzleBenchmarkRunner<TPuzzle, TParsed, TInput>>();
        Console.WriteLine(summary.ToString());
    }

    private static PuzzleResult RunPuzzle<TPuzzle, TParsed, TInput>(PuzzleModel puzzle)
        where TPuzzle : IPuzzle<TParsed, TInput>
        where TInput : IPuzzleInput
    {
        var rawInput = TInput.Input;
        var parsed = TPuzzle.Parse(rawInput);
        var parsed2 = TPuzzle.Parse(rawInput);

        var part1 = TPuzzle.Part1(parsed);
        var part2 = TPuzzle.Part2(parsed2);

        return new PuzzleResult(puzzle, part1, part2);
    }

    private IReadOnlyList<PuzzleModel> GetAllPuzzles()
    {
        var assembly = Assembly.GetAssembly(typeof(Day01));

        var c = assembly.GetTypes()
            .Select(x => new
            {
                Type = x,
                PuzzleAttribute = x.GetCustomAttribute<PuzzleAttribute>(),
            })
            .Where(x => x.PuzzleAttribute != null);

        return c.Select(x => new PuzzleModel(
                x.PuzzleAttribute.Name,
                x.PuzzleAttribute.Year,
                x.PuzzleAttribute.Day,
                x.Type,
                x.Type.GetInterfaces()[0].GenericTypeArguments[0],
                x.Type.GetInterfaces()[0].GenericTypeArguments[1]))
            .ToList();
    }
}