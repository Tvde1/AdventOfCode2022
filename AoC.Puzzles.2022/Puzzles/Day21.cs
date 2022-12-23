using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AoC.Common.Algorithms;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 21, "Monkey Math")]
public class Day21 : IPuzzle<Dictionary<string, MathMonkey>>
{
    public Dictionary<string, MathMonkey> Parse(string inputText)
    {
        // inputText = """
        //     root: pppw + sjmn
        //     dbpl: 5
        //     cczh: sllz + lgvd
        //     zczc: 2
        //     ptdq: humn - dvpt
        //     dvpt: 3
        //     lfqf: 4
        //     humn: 5
        //     ljgn: 2
        //     sjmn: drzm * dbpl
        //     sllz: 4
        //     pppw: cczh / lfqf
        //     lgvd: ljgn * ptdq
        //     drzm: hmdt - zczc
        //     hmdt: 32
        //     """;
        return inputText.Split(Environment.NewLine).Select(MathMonkey.Parse).ToDictionary(x => x.Name);
    }

    public string Part1(Dictionary<string, MathMonkey> input)
    {
        const string target = "root";

        return GetMonkeyNumber(target).ToString();

        long GetMonkeyNumber(string monkeyName)
        {
            var monkey = input[monkeyName];
            var result = monkey.Expression switch
            {
                MonkeyConstExpression monkeyConstExpression => monkeyConstExpression.Value,
                MonkeyBinaryExpression monkeyBinaryExpression => monkeyBinaryExpression.Operator switch
                {
                    '+' => GetMonkeyNumber(monkeyBinaryExpression.Left) + GetMonkeyNumber(monkeyBinaryExpression.Right),
                    '-' => GetMonkeyNumber(monkeyBinaryExpression.Left) - GetMonkeyNumber(monkeyBinaryExpression.Right),
                    '*' => GetMonkeyNumber(monkeyBinaryExpression.Left) * GetMonkeyNumber(monkeyBinaryExpression.Right),
                    '/' => GetMonkeyNumber(monkeyBinaryExpression.Left) / GetMonkeyNumber(monkeyBinaryExpression.Right),
                    _ => throw new ArgumentOutOfRangeException(),
                },
                _ => throw new ArgumentOutOfRangeException(),
            };

            return result;
        }
    }

    public string Part2(Dictionary<string, MathMonkey> input)
    {
        var root = input["root"];
        var rootExpression = (root.Expression as MonkeyBinaryExpression)!;
        root.Expression = new MonkeyBinaryExpression('=', rootExpression.Left, rootExpression.Right);

        var me = input["humn"];
        me.Expression = new HumanInputExpression();

        var totalExpr = BuildExpression("root");
        
        Debugger.Break();

        string BuildExpression(string monkeyName)
        {
            if (monkeyName == "humn")
            {
                return "x";
            }
            
            var monkey = input[monkeyName];
            var expr = monkey.Expression switch
            {
                MonkeyConstExpression monkeyConstExpression => monkeyConstExpression.Value + "L",
                MonkeyBinaryExpression monkeyBinaryExpression => $"({BuildExpression(monkeyBinaryExpression.Left)} {monkeyBinaryExpression.Operator} {BuildExpression(monkeyBinaryExpression.Right)})",
                _ => throw new ArgumentOutOfRangeException(),
            };

            if (!expr.Contains('x') && !expr.Contains('='))
            {
                expr = CSharpScript.EvaluateAsync<double>(expr).Result + "L";
            }

            return expr;
        }


        return "Not implemented";
    }
}

public partial record struct MathMonkey(string Name, MonkeyExpression Expression)
{
    private static readonly Regex Regex = MathMonkeyRegex();

    public static MathMonkey Parse(string input)
    {
        var match = Regex.Match(input);
        var name = match.Groups["name"].Value;
        var isConstMonkey = match.Groups["constExpr"].Success;
        if (isConstMonkey)
        {
            return new MathMonkey(name, new MonkeyConstExpression(int.Parse(match.Groups["constExpr"].Value)));
        }

        var left = match.Groups["left"].Value;
        var right = match.Groups["right"].Value;
        var op = match.Groups["op"].Value;

        return new MathMonkey(name, new MonkeyBinaryExpression(op[0], left, right));
    }

    [GeneratedRegex(
        """
        ^(?<name>\w+): (?:(?<constExpr>\d+)|(?<opExpr>(?<left>[a-z]{4}) (?<op>[\+\*\-\/]) (?<right>[a-z]{4})))$
        """, RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex MathMonkeyRegex();
}

public abstract record MonkeyExpression;

public record MonkeyConstExpression(long Value) : MonkeyExpression;
public record MonkeyBinaryExpression(char Operator, string Left, string Right) : MonkeyExpression;
public record HumanInputExpression() : MonkeyExpression;