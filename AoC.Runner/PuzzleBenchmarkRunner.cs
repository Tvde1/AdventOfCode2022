using AoC.Common.Interfaces;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace AoC.Runner;

[SimpleJob(RuntimeMoniker.Net70)]
[HtmlExporter, MarkdownExporter]
[MemoryDiagnoser(false)]
public class PuzzleBenchmarkRunner<TPuzzle, TParsed, TPuzzleInputProvider>
    where TPuzzle : IPuzzle<TParsed>, new()
    where TPuzzleInputProvider : IPuzzleInputProvider
{
    private readonly TPuzzle _puzzle = new TPuzzle();
    private readonly string _rawInput = TPuzzleInputProvider.GetRawInput();
    private TParsed? _parsed;

    [GlobalSetup]
    public void Setup()
    {
        _parsed = _puzzle.Parse(_rawInput);
    }

    [Benchmark, BenchmarkCategory("Parse")]
    public TParsed Parse()
    {
        return _puzzle!.Parse(_rawInput!);
    }

    [Benchmark, BenchmarkCategory("Part1")]
    public string Part1()
    {
        return _puzzle!.Part1(_parsed!);
    }

    [Benchmark, BenchmarkCategory("Part2")]
    public string Part2()
    {
        return _puzzle!.Part2(_parsed!);
    }
}
