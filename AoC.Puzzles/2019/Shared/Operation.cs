using System.Diagnostics;

namespace AoC.Puzzles._2019.Shared;

public ref struct Operation
{
    public Opcodes Opcode { get; }
    public ParameterMode[] ParameterModes { get; }

    private Operation(Opcodes opcode, ParameterMode[] parameterModes)
    {
        Opcode = opcode;
        ParameterModes = parameterModes;
    }

    public static Operation Parse(long input)
    {
        var opcode = (Opcodes)(input % 100);

        const int parameterModeCount = 4;
        var parameterModes = new ParameterMode[parameterModeCount];

        input /= 10;
        for (var i = 0; i < parameterModeCount; i++)
        {
            input /= 10;
            parameterModes[i] = (ParameterMode)(input % 10);
        }
        
        var parsed = new Operation(opcode, parameterModes);

        return parsed;
    }

    public override string ToString()
    {
        return $"{string.Join(string.Empty, ParameterModes.Reverse().Select(x => (int)x))}{((int)Opcode):D2}".TrimStart('0');
    }
}