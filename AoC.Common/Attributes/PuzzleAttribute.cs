namespace AoC.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PuzzleAttribute : Attribute
{
    public string Name { get; }
    public int Year { get; }
    public int Day { get; }

    public PuzzleAttribute(int year, int day, string name)
    {
        Year = year;
        Day = day;
        Name = name;
    }
}
