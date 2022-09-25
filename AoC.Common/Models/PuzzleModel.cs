namespace AoC.Common.Models;

public readonly record struct PuzzleModel(string Name, int Year, int Day, Type PuzzleType, Type ParsedType, Type InputType);