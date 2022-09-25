namespace AoC.Puzzles._2019;

public enum Opcodes
{
    Addition = 1,
    Multiplicaton = 2,
    End = 99
}

public class Computer
{
    private readonly int[] _memory;

    public Computer(int[] memory, int? verb, int? noun)
    {
        _memory = memory;

        if (verb.HasValue) _memory[1] = verb.Value;
        if (noun.HasValue) _memory[2] = noun.Value;
    }

    public int FirstInteger => _memory[0];

    public void Execute()
    {
        var cursor = 0;

        while (true)
        {
            var opcode = (Opcodes) _memory[cursor];
            var a = _memory[cursor + 1];
            var b = _memory[cursor + 2];
            var c = _memory[cursor + 3];

            switch (opcode)
            {
                case Opcodes.Addition:
                    {
                        _memory[c] = _memory[a] + _memory[b];
                        cursor += 4;
                        break;
                    }
                case Opcodes.Multiplicaton:
                    {
                        _memory[c] = _memory[a] * _memory[b];
                        cursor += 4;
                        break;
                    }
                case Opcodes.End:
                    return;
                    cursor ++;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode), $"Invalid opcode: {opcode}.");
            }
        }
    }
}
