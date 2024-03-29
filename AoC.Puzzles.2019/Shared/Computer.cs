﻿namespace AoC.Puzzles._2019.Shared;

public class Computer
{
    private readonly Memory _memory;
    private long _cursor;
    private long _relativeOffset;

    public Computer(long[] memory)
    {
        _memory = new Memory(memory);
    }

    public Computer(int[] memory)
    {
        var m = new long[memory.Length];
        memory.CopyTo(m, 0);

        _memory = new Memory(m);
    }

    public int FirstInteger => (int)_memory[0];

    public bool ContinueWithInput(long? input, out List<long> outputs)
    {
        outputs = new List<long>();
        while (true)
        {
            var operation = Operation.Parse(_memory[_cursor]);

            if (operation.Opcode == Opcodes.Input && input == null)
            {
                return false;
            }

            _cursor++;

            var output = RunCycleInternal(operation, ref input);

            switch (output)
            {
                case { IsExit: true, }:
                    return true;
                case { Fault: { } f, }:
                    throw new InvalidOperationException(f);
                case { Value: { } v, }:
                    outputs.Add(v);
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
            var hasFinished = ContinueWithInput(inputs[inputCursor], out var currOutputs);
            outputs.AddRange(currOutputs);

            if (hasFinished)
            {
                break;
            }

            inputCursor++;
        }

        return outputs;
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

    private long GetParameterCursor(ParameterMode parameterMode)
    {
        var param = parameterMode switch
        {
            ParameterMode.Position => _memory[_cursor],
            ParameterMode.Immediate => _cursor,
            ParameterMode.Relative => _memory[_cursor] + _relativeOffset,
            _ => throw new ArgumentOutOfRangeException(nameof(parameterMode), parameterMode,
                "Not a valid ParameterMode")
        };
        _cursor++;
        return param;
    }

    private void Add(Operation operation)
    {
        var a = GetParameterCursor(operation.ParameterModes[0]);
        var b = GetParameterCursor(operation.ParameterModes[1]);
        var c = GetParameterCursor(operation.ParameterModes[2]);

        _memory[c] = _memory[a] + _memory[b];
    }

    private void Multiply(Operation operation)
    {
        var a = GetParameterCursor(operation.ParameterModes[0]);
        var b = GetParameterCursor(operation.ParameterModes[1]);
        var c = GetParameterCursor(operation.ParameterModes[2]);

        _memory[c] = _memory[a] * _memory[b];
    }

    private void Input(Operation operation, long input)
    {
        var a = GetParameterCursor(operation.ParameterModes[0]);
        _memory[a] = input;
    }

    private long Output(Operation operation)
    {
        var a = GetParameterCursor(operation.ParameterModes[0]);
        return _memory[a];
    }

    private void JumpIfTrue(Operation operation)
    {
        var a = GetParameterCursor(operation.ParameterModes[0]);
        var b = GetParameterCursor(operation.ParameterModes[1]);

        if (_memory[a] != 0)
        {
            _cursor = _memory[b];
        }
    }

    private void JumpIfFalse(Operation operation)
    {
        var a = GetParameterCursor(operation.ParameterModes[0]);
        var b = GetParameterCursor(operation.ParameterModes[1]);

        if (_memory[a] == 0)
        {
            _cursor = _memory[b];
        }
    }

    private void LessThan(Operation operation)
    {
        var a = GetParameterCursor(operation.ParameterModes[0]);
        var b = GetParameterCursor(operation.ParameterModes[1]);
        var c = GetParameterCursor(operation.ParameterModes[2]);

        _memory[c] = _memory[a] < _memory[b] ? 1 : 0;
    }

    private void Equals(Operation operation)
    {
        var a = GetParameterCursor(operation.ParameterModes[0]);
        var b = GetParameterCursor(operation.ParameterModes[1]);
        var c = GetParameterCursor(operation.ParameterModes[2]);

        _memory[c] = _memory[a] == _memory[b] ? 1 : 0;
    }

    private void AdjustRelativeOffset(Operation operation)
    {
        var a = GetParameterCursor(operation.ParameterModes[0]);

        _relativeOffset += _memory[a];
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