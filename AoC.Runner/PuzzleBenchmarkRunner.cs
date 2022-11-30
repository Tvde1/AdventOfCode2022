using AoC.Common.Interfaces;
using BenchmarkDotNet.Attributes;

namespace AoC.Runner;

[HtmlExporter, MarkdownExporter]
[MemoryDiagnoser(false)]
public class PuzzleBenchmarkRunner<TPuzzle, TParsed, TPuzzleInputProvider>
    where TPuzzle : IPuzzle<TParsed>, new()
    where TPuzzleInputProvider : IPuzzleInputProvider
{
    private TPuzzle? _puzzle;
    private string? _rawInput;
    private TParsed? _parsed;

    [GlobalSetup]
    public void GetInput()
    {
        _rawInput = TPuzzleInputProvider.GetRawInput();

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
