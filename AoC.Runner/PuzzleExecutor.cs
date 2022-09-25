using AoC.Common.Models;
using BenchmarkDotNet.Attributes;

namespace AoC.Runner;

[MemoryDiagnoser]
public class PuzzleBenchmarkRunner<TPuzzle, TParsed, TInput>
        where TPuzzle : IPuzzle<TParsed, TInput>
        where TInput : IPuzzleInput
{
    private string _rawInput;
    private TParsed _parsed = default!;

    [GlobalSetup]
    public void GetInput()
    {
        _rawInput = TInput.Input;
        _parsed = TPuzzle.Parse(_rawInput);
    }

    [Benchmark]
    public TParsed Parse()
    {
        return TPuzzle.Parse(_rawInput);
    }

    [Benchmark]
    public string Part1()
    {
        return TPuzzle.Part1(_parsed);
    }

    [Benchmark]
    public string Part2()
    {
        return TPuzzle.Part2(_parsed);
    }
}
