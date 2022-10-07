using AoC.Common.Interfaces;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace AoC.Runner;

[SimpleJob(RuntimeMoniker.Net60, baseline: true)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.NativeAot70)]
[HtmlExporter, MarkdownExporter]
[MemoryDiagnoser(false)]
public class PuzzleBenchmarkRunner<TPuzzle, TParsed, TPuzzleInputProvider>
        where TPuzzle : IPuzzle<TParsed>, new()
        where TPuzzleInputProvider : IPuzzleInputProvider, new()
{
    private TPuzzle? _puzzle;
    private string? _rawInput;
    private TParsed? _parsed;

    [GlobalSetup]
    public void GetInput()
    {
        var puzzleProvider = new TPuzzleInputProvider();
        _rawInput = puzzleProvider.GetRawInput();

        _puzzle = new TPuzzle();

        _parsed = _puzzle.Parse(_rawInput);
    }

    [BenchmarkCategory("Parse"), Benchmark]
    public TParsed Parse()
    {
        return _puzzle!.Parse(_rawInput!);
    }

    [BenchmarkCategory("Part1"),Benchmark]
    public string Part1()
    {
        return _puzzle!.Part1(_parsed!);
    }

    [BenchmarkCategory("Part2"),Benchmark]
    public string Part2()
    {
        return _puzzle!.Part2(_parsed!);
    }
}
