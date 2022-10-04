namespace AoC.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PuzzleInputAttribute : Attribute
{
    public int Year { get; }

    public int Day { get; }

    public PuzzleInputAttribute(int year, int day)
    {
        Year = year;
        Day = day;
    }
}