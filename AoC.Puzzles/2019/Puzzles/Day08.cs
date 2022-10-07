using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 8, "Space image format")]
public class Day08 : IPuzzle<int[]>
{
    public int[] Parse(string inputText)
    {
        var arr = new int[inputText.Length];
        for (var i = 0; i < inputText.Length; i++)
        {
            arr[i] = inputText[i] - '0';
        }
        return arr;
    }

    public string Part1(int[] input)
    {
        const int width = 25;
        const int height = 6;
        const int layerArea = width * height;

        var depth = input.Length / layerArea;

        var layers = SetupCreateLayers(input, depth, width, height);

        int layerWithFewestZeroes = GetLayerWithFewestZeroes(layers, depth, width, height);
        var (num1Digits, num2Digits) = CountDigits(layers, layerWithFewestZeroes, width, height);

        return (num1Digits * num2Digits).ToString();
    }

    public string Part2(int[] input)
    {
        return "Not implemented";
    }

    private static (int Num1Digits, int Num2Digits) CountDigits(int[,,] layers, int layerWithFewestZeroes, int width, int height)
    {
        var num1Digits = 0;
        var num2Digits = 0;
        for (var currX = 0; currX < width; currX++)
        {
            for (var currY = 0; currY < height; currY++)
            {
                var item = layers[layerWithFewestZeroes, currX, currY];
                if (item == 1)
                {
                    num1Digits++;
                }
                else if (item == 2)
                {
                    num2Digits++;
                }
            }
        }

        return (num1Digits, num2Digits);
    }

    private static int GetLayerWithFewestZeroes(int[,,] layers, int depth, int width, int height)
    {
        int layerWithFewestZeroes = -1;
        int lowestZeroCount = int.MaxValue;
        for (var currLayer = 0; currLayer < depth; currLayer++)
        {
            int localZeroCount = 0;
            for (var currX = 0; currX < width; currX++)
            {
                for (var currY = 0; currY < height; currY++)
                {
                    if (layers[currLayer, currX, currY] == 0)
                    {
                        localZeroCount++;
                    }
                }
            }

            if (localZeroCount < lowestZeroCount)
            {
                lowestZeroCount = localZeroCount;
                layerWithFewestZeroes = currLayer;
            }
        }

        return layerWithFewestZeroes;
    }

    private static int[,,] SetupCreateLayers(int[] input, int depth, int width, int height)
    {
        var layers = new int[depth, width, height];

        var i = 0;
        for (var currLayer = 0; currLayer < depth; currLayer++)
        {
            for (var currX = 0; currX < width; currX++)
            {
                for (var currY = 0; currY < height; currY++)
                {
                    layers[currLayer, currX, currY] = input[i];
                    i++;
                }
            }
        }

        return layers;
    }
}