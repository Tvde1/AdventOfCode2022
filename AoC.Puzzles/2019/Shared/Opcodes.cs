namespace AoC.Puzzles._2019.Shared;

public enum Opcodes : byte
{
    Addition = 1,
    Multiplication = 2,
    Input = 3,
    Output = 4,
    JumpIfTrue = 5,
    JumpIfFalse = 6,
    LessThan = 7,
    Equals = 8,
    AdjustRelativeOffset = 9,
    End = 99
}