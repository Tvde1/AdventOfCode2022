using AoC.Common.Attributes;
using AoC.Common.Models;
using BenchmarkDotNet.Running;
using System.Reflection;
using AoC.Common.Interfaces;
using System.Diagnostics;

namespace AoC.Runner;

public class PuzzleRunner : IPuzzleRunner
{
    private readonly IReadOnlyList<PuzzleModel> _puzzles;
    private readonly MethodInfo _runMethod;
    private readonly MethodInfo _benchmarkMethod;

    private static readonly Assembly[] Assemblies =
    {
        Assembly.GetAssembly(typeof(Puzzles._2019.Puzzles.Day01))!,
        Assembly.GetAssembly(typeof(Puzzles._2022.Puzzles.Day01))!,
        Assembly.GetAssembly(typeof(Puzzles._2023.Puzzles.Day01))!,
    };

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

        return (PuzzleResult)method.Invoke(null, new object[] { puzzle, })!;
    }

    public void BenchmarkPuzzle(PuzzleModel puzzle)
    {
        var method = _benchmarkMethod.MakeGenericMethod(puzzle.PuzzleType, puzzle.ParsedType, puzzle.InputType);

        method.Invoke(null, null);
    }

    private static void RunBenchmark<TPuzzle, TParsed, TPuzzleInputProvider>()
        where TPuzzle : IPuzzle<TParsed>, new()
        where TPuzzleInputProvider : IPuzzleInputProvider
    {
        var summary = BenchmarkRunner.Run<PuzzleBenchmarkRunner<TPuzzle, TParsed, TPuzzleInputProvider>>();
        Console.WriteLine(summary.ToString());
    }

    private static PuzzleResult RunPuzzle<TPuzzle, TParsed, TPuzzleInputProvider>(PuzzleModel puzzleInfo)
        where TPuzzle : IPuzzle<TParsed>, new()
        where TPuzzleInputProvider : IPuzzleInputProvider
    {
        var puzzle = new TPuzzle();

        var rawInput = TPuzzleInputProvider.GetRawInput();

        var parsed = puzzle.Parse(rawInput);
        var parsed2 = puzzle.Parse(rawInput);

        var sw = Stopwatch.StartNew();
        string part1 = "Errored";
        Exception? part1Exception = null;
        try
        {
            part1 = puzzle.Part1(parsed);
        }
        catch (Exception e)
        {
            part1Exception = e;
        }
        finally
        {
            sw.Stop();
        }

        var elapsedPart1 = sw.ElapsedMilliseconds;

        sw.Restart();
        string part2 = "Errored";
        Exception? part2Exception = null;
        try
        {
            part2 = puzzle.Part2(parsed2);
        }
        catch (Exception e)
        {
            part2Exception = e;
        }
        finally
        {
            sw.Stop();
        }
        var elapsedPart2 = sw.ElapsedMilliseconds;

        return new PuzzleResult(puzzleInfo, part1, part2, elapsedPart1, elapsedPart2, part1Exception, part2Exception);
    }

    private static Dictionary<(int Year, int Day), Type> GetPuzzleInputProviders()
    {
        var c = Assemblies
            .SelectMany(
                assembly => assembly.GetTypes(),
                (_, type) => new
                {
                    Type = type,
                    PuzzleAttribute = type.GetCustomAttribute<PuzzleInputAttribute>(),
                })
            .Where(x => x.PuzzleAttribute != null);

        return c.ToDictionary(
            x => (x.PuzzleAttribute!.Year, x.PuzzleAttribute.Day),
            x => x.Type);
    }

    private static IReadOnlyList<PuzzleModel> GetAllPuzzles()
    {
        var puzzleInputProviders = GetPuzzleInputProviders();

        var c = Assemblies
            .SelectMany(
                assembly => assembly.GetTypes(),
                (_, type) => new
                {
                    Type = type,
                    PuzzleAttribute = type.GetCustomAttribute<PuzzleAttribute>(),
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