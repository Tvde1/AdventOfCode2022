namespace AoC.Common.Interfaces;

public interface IPuzzle<TParsed>
{
    TParsed Parse(string rawInput);

    string Part1(TParsed input);// => "Not implemented";
    string Part2(TParsed input);// => "Not implemented";
}