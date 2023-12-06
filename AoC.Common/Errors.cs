namespace AoC.Common;

public class BarfException() : Exception("Bleeech")
{
    public static BarfException up => new BarfException();
}