using AoC.Common.Interfaces;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace AoC.Runner;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[MemoryDiagnoser]
public class PuzzleBenchmarkRunner<TPuzzle, TParsed, TPuzzleInputProvider>
        where TPuzzle : IPuzzle<TParsed>, new()
        where TPuzzleInputProvider : IPuzzleInputProvider, new()
{
    private TPuzzle _puzzle = default!;
    private string _rawInput = null!;
    private TParsed _parsed = default!;
    private TParsed _parsed2 = default!;

    [GlobalSetup]
    public void GetInput()
    {
        var puzzleProvider = new TPuzzleInputProvider();
        _rawInput = puzzleProvider.GetRawInput();

        _puzzle = new TPuzzle();

        _parsed = _puzzle.Parse(_rawInput);
        _parsed2 = _puzzle.Parse(_rawInput);
    }

    [Benchmark]
    public TParsed Parse()
    {
        return _puzzle.Parse(_rawInput);
    }

    [Benchmark]
    public string Part1()
    {
        return _puzzle.Part1(_parsed);
    }

    [Benchmark]
    public string Part2()
    {
        return _puzzle.Part2(_parsed2);
    }
}
