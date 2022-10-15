using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2019.Puzzles;

[Puzzle(2019, 14, "Ores")]
public class Day14 : IPuzzle<List<Reaction>>
{
    public List<Reaction> Parse(string inputText)
    {
        return inputText.Split(Environment.NewLine).Select(Reaction.Parse).ToList();
    }

    public string Part1(List<Reaction> input)
    {
        var recepies = input.ToDictionary(x => x.Output.Name);

        var oreRequirements = MakeFuel(recepies);
        return oreRequirements.ToString();
    }

    public string Part2(List<Reaction> input)
    {
        var recepies = input.ToDictionary(x => x.Output.Name);

        var ores = 1_000_000_000_000;

        var low = ores / MakeFuel(recepies);
        var high = 10_000_000L;

        while (low < high - 1)
        {
            var mid = (low + high) / 2;

            Console.WriteLine($"Trying {mid}");

            var oreRequirements = MakeFuel(recepies, mid);

            if (oreRequirements > ores)
            {
                high = mid;
            }
            else
            {
                low = mid;
            }
        }

        return low.ToString();
    }


    private static long MakeFuel(IReadOnlyDictionary<string, Reaction> recepies, long requiredFuel = 1)
    {
        var requirements = recepies.Select(x => x.Value.Output.Name).ToDictionary(x => x, _ => 0L);
        var bag = recepies.Select(x => x.Value.Output.Name).ToDictionary(x => x, _ => 0L);

        requirements["FUEL"] = requiredFuel;

        var oreRequirements = 0L;

        while (true)
        {
            var f = false;
            foreach (var required in requirements)
            {
                if (required.Value == 0)
                {
                    continue;
                }

                f = true;
                if (bag[required.Key] >= required.Value)
                {
                    bag[required.Key] -= required.Value;
                    requirements[required.Key] -= required.Value;
                    continue;
                }

                var reaction = recepies[required.Key];

                var requiredNewChemicals = required.Value - bag[required.Key];

                var requiredReactions = requiredNewChemicals / reaction.Output.Quantity;
                if (requiredNewChemicals % reaction.Output.Quantity != 0) { requiredReactions++; }

                foreach (var inputs in reaction.Inputs)
                {
                    if (inputs.Name == "ORE")
                    {
                        oreRequirements += inputs.Quantity * requiredReactions;
                    }
                    else
                    {
                        requirements[inputs.Name] += inputs.Quantity * requiredReactions;
                    }
                }

                bag[required.Key] += reaction.Output.Quantity * requiredReactions;
            }

            if (!f)
                return oreRequirements;
        }
    }
}

public record Ingredient(long Quantity, string Name)
{
    public long Quantity { get; set; } = Quantity;

    public static Ingredient Parse(string input)
    {
        var split = input.Split(' ');

        return new Ingredient(long.Parse(split[0]), split[1]);
    }
}

public record Reaction(Ingredient[] Inputs, Ingredient Output)
{
    public static Reaction Parse(string input)
    {
        var reactionParts = input.Split("=>", StringSplitOptions.TrimEntries);

        var output = Ingredient.Parse(reactionParts[1]);

        var splitInputs = reactionParts[0].Split(',', StringSplitOptions.TrimEntries);
        var inputs = new Ingredient[splitInputs.Length];
        for (var i = 0; i < splitInputs.Length; i++)
        {
            inputs[i] = Ingredient.Parse(splitInputs[i]);
        }

        return new Reaction(inputs, output);
    }
}