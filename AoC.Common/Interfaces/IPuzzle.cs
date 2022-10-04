namespace AoC.Common.Interfaces;

public interface IPuzzle<TParsed>
{
    TParsed Parse(string rawInput);

    string Part1(TParsed input);
    string Part2(TParsed input);
}