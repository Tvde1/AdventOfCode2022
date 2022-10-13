using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AoC.Puzzles._2019.Shared;

public enum ParameterMode : byte
{
    Position = 0,
    Immediate = 1,
    Relative = 2,
}

public struct Computer
{
    private readonly Memory _memory;
    private long _cursor = 0;
    private long _relativeOffset = 0;

    public Computer(long[] memory)
    {
        _memory = new(memory);
    }
    
    public Computer(int[] memory)
    {
        var m = new long[memory.Length];
        memory.CopyTo(m, 0);

        _memory = new(m);
    }

    public int FirstInteger => (int) _memory[0];

    public List<Out> ContinueWithInput(long? input)
    {
        var outputs = new List<Out>();
        while (true)
        {
            var operation = Operation.Parse(_memory[_cursor]);

            if (operation.Opcode == Opcodes.Input && input == null)
            {
                return outputs;
            }

            _cursor++;

            var output = RunCycleInternal(operation, ref input);

            switch (output)
            {
                case { IsExit: true }:
                    outputs.Add(output.Value);
                    return outputs;
                case { Fault: { } f }:
                    throw new InvalidOperationException(f);
                case { Value: { } v }:
                    outputs.Add(output.Value);
                    break;
                default:
                    continue;
            }
        }
    }

    public List<long> Execute(ReadOnlySpan<long> inputs = new())
    {
        var outputs = new List<long>();
        var inputCursor = 0;

        while (true)
        {
            var res = ContinueWithInput(inputs[inputCursor]);
            foreach (var r in CollectionsMarshal.AsSpan(res))
            {
                if (r.IsExit)
                {
                    return outputs;
                }

                outputs.Add(r.Value!.Value);
            }

            inputCursor++;
        }
    }

    private Out? RunCycleInternal(Operation operation, ref long? input)
    {
        switch (operation.Opcode)
        {
            case Opcodes.Addition:
                Add(operation);
                return null;
            case Opcodes.Multiplication:
                Multiply(operation);
                return null;
            case Opcodes.Input:
                if (input == null)
                    return Out.Faulted("No input provided for input opcode");
                Input(operation, input.Value);
                input = null;
                return null;
            case Opcodes.Output:
                var output = Output(operation);
                return Out.Result(output);
            case Opcodes.JumpIfTrue:
                JumpIfTrue(operation);
                return null;
            case Opcodes.JumpIfFalse:
                JumpIfFalse(operation);
                return null;
            case Opcodes.LessThan:
                LessThan(operation);
                return null;
            case Opcodes.Equals:
                Equals(operation);
                return null;
            case Opcodes.AdjustRelativeOffset:
                AdjustRelativeOffset(operation);
                return null;
            case Opcodes.End:
                return Out.Exit;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), $"Invalid opcode: {operation.Opcode}.");
        }
    }

    private long GetParameter(ParameterMode parameterMode)
    {
        var param = parameterMode switch
        {
            ParameterMode.Position => _memory[_memory[_cursor]],
            ParameterMode.Immediate => _memory[_cursor],
            ParameterMode.Relative => _memory[_memory[_cursor] + _relativeOffset],
            _ => throw new ArgumentOutOfRangeException(nameof(parameterMode), parameterMode, "Not a valid ParameterMode")
        };
        _cursor++;
        return param;
    }

    private void Add(Operation operation)
    {
        var a = GetParameter(operation.ParameterModes[0]);
        var b = GetParameter(operation.ParameterModes[1]);
        var c = GetParameter(operation.ParameterModes[2]);

        _memory[c] = a + b;
    }

    private void Multiply(Operation operation)
    {
        var a = GetParameter(operation.ParameterModes[0]);
        var b = GetParameter(operation.ParameterModes[1]);
        var c = GetParameter(operation.ParameterModes[2]);

        _memory[c] = a * b;
    }

    private void Input(Operation operation, long input)
    {
        var a = GetParameter(operation.ParameterModes[0]);
        _memory[a] = input;
    }

    private long Output(Operation operation)
    {
        var a = GetParameter(operation.ParameterModes[0]);
        return a;
    }

    private void JumpIfTrue(Operation operation)
    {
        var a = GetParameter(operation.ParameterModes[0]);
        var b = GetParameter(operation.ParameterModes[1]);

        if (a != 0)
        {
            _cursor = b;
        }
    }

    private void JumpIfFalse(Operation operation)
    {
        var a = GetParameter(operation.ParameterModes[0]);
        var b = GetParameter(operation.ParameterModes[1]);

        if (a == 0)
        {
            _cursor = b;
        }
    }

    private void LessThan(Operation operation)
    {
        var a = GetParameter(operation.ParameterModes[0]);
        var b = GetParameter(operation.ParameterModes[1]);
        var c = GetParameter(operation.ParameterModes[2]);

        _memory[c] = a < b ? 1 : 0;
    }

    private void Equals(Operation operation)
    {
        var a = GetParameter(operation.ParameterModes[0]);
        var b = GetParameter(operation.ParameterModes[1]);
        var c = GetParameter(operation.ParameterModes[2]);

        _memory[c] = a == b ? 1 : 0;
    }
    
    private void AdjustRelativeOffset(Operation operation)
    {
        var a = GetParameter(operation.ParameterModes[0]);

        _relativeOffset += a;
    }
}

public readonly record struct Out(bool IsExit, long? Value, string? Fault)
{
    public static Out Exit => new(true, default, null);

    public static Out Result(long value) => new(false, value, null);

    public static Out Faulted(string fault) => new(false, null, fault);

    public override string ToString()
    {
        return IsExit ? "EXIT"
            : Value != null ? Value.Value.ToString()
            : Fault!;
    }
}