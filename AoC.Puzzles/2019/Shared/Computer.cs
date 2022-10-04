using System.Diagnostics;
using System.Runtime.CompilerServices;

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
    End = 99
}

public enum ParameterMode : byte
{
    Position = 0,
    Immediate = 1,
}

public readonly ref struct Computer
{
    private readonly Span<int> _memory;

    public Computer(int[] memory)
    {
        _memory = memory;
    }

    public int FirstInteger => _memory[0];

    public List<int> Execute(ReadOnlySpan<int> inputs = new())
    {
        var outputs = new List<int>();
        var cursor = 0;
        var inputCursor = 0;

        while (true)
        {
            var operation = Operation.Parse(_memory[cursor]);
            cursor++;

            var output = RunCycle(_memory, inputs, operation, ref cursor, ref inputCursor);
            switch (output)
            {
                case { IsExit: true }:
                    return outputs;
                case { } o:
                    outputs.Add(o.Value);
                    break;
                default:
                    continue;
            }
        }
    }

    private static Output? RunCycle(Span<int> memory, ReadOnlySpan<int> inputs, Operation operation, ref int cursor,
        ref int inputCursor)
    {
        switch (operation.Opcode)
        {
            case Opcodes.Addition:
                Operations.Add(memory, operation, ref cursor);
                return null;
            case Opcodes.Multiplication:
                Operations.Multiply(memory, operation, ref cursor);
                return null;
            case Opcodes.Input:
                Debug.Assert(operation.ParameterModes.ToArray().All(x => x == ParameterMode.Position));
                Operations.Input(memory, ref cursor, inputs[inputCursor++]);
                return null;
            case Opcodes.Output:
                var output = Operations.Output(memory, operation, ref cursor);
                return Output.FromValue(output);
            case Opcodes.JumpIfTrue:
                Operations.JumpIfTrue(memory, operation, ref cursor);
                return null;
            case Opcodes.JumpIfFalse:
                Operations.JumpIfFalse(memory, operation, ref cursor);
                return null;
            case Opcodes.LessThan:
                Operations.LessThan(memory, operation, ref cursor);
                return null;
            case Opcodes.Equals:
                Operations.Equals(memory, operation, ref cursor);
                return null;
            case Opcodes.End:
                return Output.Exit;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation.Opcode), $"Invalid opcode: {operation.Opcode}.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetParameter(Span<int> memory, ParameterMode parameterMode, ref int cursor)
    {
        var param = parameterMode switch
        {
            ParameterMode.Position => memory[memory[cursor]],
            ParameterMode.Immediate => memory[cursor],
            _ => throw new ArgumentOutOfRangeException()
        };
        cursor++;
        return param;
    }

    private static class Operations
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(Span<int> memory, Operation operation, ref int cursor)
        {
            var a = GetParameter(memory, operation.ParameterModes[0], ref cursor);
            var b = GetParameter(memory, operation.ParameterModes[1], ref cursor);
            var c = memory[cursor++];

            memory[c] = a + b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(Span<int> memory, Operation operation, ref int cursor)
        {
            var a = GetParameter(memory, operation.ParameterModes[0], ref cursor);
            var b = GetParameter(memory, operation.ParameterModes[1], ref cursor);
            var c = memory[cursor++];

            memory[c] = a * b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Input(Span<int> memory, ref int cursor, int input)
        {
            var a = memory[cursor++];
            memory[a] = input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Output(Span<int> memory, Operation operation, ref int cursor)
        {
            var a = GetParameter(memory, operation.ParameterModes[0], ref cursor);
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void JumpIfTrue(Span<int> memory, Operation operation, ref int cursor)
        {
            var a = GetParameter(memory, operation.ParameterModes[0], ref cursor);
            var b = GetParameter(memory, operation.ParameterModes[1], ref cursor);

            if (a != 0)
            {
                cursor = b;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void JumpIfFalse(Span<int> memory, Operation operation, ref int cursor)
        {
            var a = GetParameter(memory, operation.ParameterModes[0], ref cursor);
            var b = GetParameter(memory, operation.ParameterModes[1], ref cursor);

            if (a == 0)
            {
                cursor = b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LessThan(Span<int> memory, Operation operation, ref int cursor)
        {
            var a = GetParameter(memory, operation.ParameterModes[0], ref cursor);
            var b = GetParameter(memory, operation.ParameterModes[1], ref cursor);
            var c = memory[cursor++];

            memory[c] = a < b ? 1 : 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Equals(Span<int> memory, Operation operation, ref int cursor)
        {
            var a = GetParameter(memory, operation.ParameterModes[0], ref cursor);
            var b = GetParameter(memory, operation.ParameterModes[1], ref cursor);
            var c = memory[cursor++];

            memory[c] = a == b ? 1 : 0;
        }
    }
}

public ref struct Operation
{
    public Opcodes Opcode { get; }
    public Span<ParameterMode> ParameterModes { get; }

    private Operation(Opcodes opcode, Span<ParameterMode> parameterModes)
    {
        Opcode = opcode;
        ParameterModes = parameterModes;
    }

    public static Operation Parse(int input)
    {
        var opcode = (Opcodes)(input % 100);

        const int parameterModeCount = 4;
        Span<ParameterMode> parameterModes = new ParameterMode[parameterModeCount];

        input /= 10;
        for (var i = 0; i < parameterModeCount; i++)
        {
            input /= 10;
            parameterModes[i] = (ParameterMode)(input & 1);
        }

        return new Operation(opcode, parameterModes);
    }
}

public readonly record struct Output(bool IsExit, int Value)
{
    public static Output Exit => new(true, default);

    public static Output FromValue(int value) => new(false, value);
}