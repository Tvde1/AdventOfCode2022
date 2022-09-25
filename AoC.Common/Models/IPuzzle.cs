namespace AoC.Common.Models;

public interface IPuzzle<TParsed, TInput>
    where TInput : IPuzzleInput
{
    static string Input => TInput.Input;

    static abstract TParsed Parse(string rawInput);

    public static abstract string Part1(TParsed input);
    public static abstract string Part2(TParsed input);
}

public interface IPuzzleInput
{
    static abstract string Input { get; }
}