using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 6, "Tuning Trouble")]
public class Day06 : IPuzzle<string>
{
    public string Parse(string inputText) => inputText;

    public string Part1(string input) => GetIndexOfSignal(input, 4).ToString();

    public string Part2(string input) => GetIndexOfSignal(input, 14).ToString();

    private static int GetIndexOfSignal(string input, int payloadSize)
    {
        var span = input.AsSpan();
        Span<sbyte> indexToChar = stackalloc sbyte[('z' - 'a') + 1];
        indexToChar.Fill(-1);
        var index = 0;

        retry:
        for (sbyte i = 0; i < payloadSize; i++)
        {
            var currentInspectingChar = span[index + i];
            var currentInspectingCharAlreadyOccursNextAt = indexToChar[currentInspectingChar - 'a'];

            if (currentInspectingCharAlreadyOccursNextAt == -1)
            {
                indexToChar[currentInspectingChar - 'a'] = i;
                continue;
            }

            index += currentInspectingCharAlreadyOccursNextAt + 1;

            indexToChar.Fill(-1);

            goto retry;
        }

        return index + payloadSize;
    }


    private static int GetIndexOfSignalNaive(string input, int payloadSize)
    {
        var span = input.AsSpan();
        var index = 0;
        while (true)
        {
            var a = span[index..(index + payloadSize)];
        
            if (!ContainsDuplicates(a))
            {
                return index + payloadSize;
            }
        
            index++;
        }
    }
    
    private static bool ContainsDuplicates(ReadOnlySpan<char> input)
    {
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = i + 1; j < input.Length; j++)
            {
                if (input[i] == input[j])
                {
                    return true;
                }
            }
        }

        return false;
    }
}