namespace AoC.Common;

public class BarfException() : Exception("Bleeech")
{
    // ReSharper disable once InconsistentNaming
    public static BarfException up => new BarfException();
}