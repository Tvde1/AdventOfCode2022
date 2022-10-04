using AoC.Common.Attributes;
using AoC.Common.Models;
using AoC.Puzzles._2019.Puzzles;
using BenchmarkDotNet.Running;
using System.Reflection;
using AoC.Common.Interfaces;

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

    private static void RunBenchmark<TPuzzle, TParsed, TPuzzleInputProvider>()
        where TPuzzle : IPuzzle<TParsed>, new()
        where TPuzzleInputProvider : IPuzzleInputProvider, new()
    {
        var summary = BenchmarkRunner.Run<PuzzleBenchmarkRunner<TPuzzle, TParsed, TPuzzleInputProvider>>();
        Console.WriteLine(summary.ToString());
    }

    private static PuzzleResult RunPuzzle<TPuzzle, TParsed, TPuzzleInputProvider>(PuzzleModel puzzleInfo)
        where TPuzzle : IPuzzle<TParsed>, new()
        where TPuzzleInputProvider : IPuzzleInputProvider, new()
    {
        var inputProvider = new TPuzzleInputProvider();
        var puzzle = new TPuzzle();

        var rawInput = inputProvider.GetRawInput();

        var parsed = puzzle.Parse(rawInput);
        var parsed2 = puzzle.Parse(rawInput);

        var part1 = puzzle.Part1(parsed);
        var part2 = puzzle.Part2(parsed2);

        return new PuzzleResult(puzzleInfo, part1, part2);
    }

    private static Dictionary<(int Year, int Day), Type> GetPuzzleInputProviders()
    {
        var assembly = Assembly.GetAssembly(typeof(Day01));

        var c = assembly!.GetTypes()
            .Select(x => new
            {
                Type = x,
                PuzzleAttribute = x.GetCustomAttribute<PuzzleInputAttribute>(),
            })
            .Where(x => x.PuzzleAttribute != null);

        return c.ToDictionary(
            x => (x.PuzzleAttribute!.Year, x.PuzzleAttribute.Day),
            x => x.Type);
    }

    private static IReadOnlyList<PuzzleModel> GetAllPuzzles()
    {
        var puzzleInputProviders = GetPuzzleInputProviders();
        var assembly = Assembly.GetAssembly(typeof(Day01));

        var c = assembly!.GetTypes()
            .Select(x => new
            {
                Type = x,
                PuzzleAttribute = x.GetCustomAttribute<PuzzleAttribute>(),
            })
            .Where(x => x.PuzzleAttribute != null);

        return c.Select(x => new PuzzleModel(
                x.PuzzleAttribute!.Name,
                x.PuzzleAttribute.Year,
                x.PuzzleAttribute.Day,
                x.Type,
                x.Type.GetInterfaces()[0].GenericTypeArguments[0],
                puzzleInputProviders[(x.PuzzleAttribute.Year, x.PuzzleAttribute.Day)]))
            .ToList();
    }
}