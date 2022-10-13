using System.Numerics;

namespace AoC.Puzzles._2019.Shared;

public class Memory
{
    private readonly long _arraySize;
    private readonly long[] _internalMemory;
    private readonly Dictionary<long, long> _additionalMemory;
    
    public Memory(long[] memory)
    {
        _arraySize = memory.Length;
        _internalMemory = memory;
        _additionalMemory = new();
    }

    public long this[long i]
    {
        get
        {
            if (i < _arraySize)
                return _internalMemory[i];
            
            return _additionalMemory.TryGetValue(i, out var val) ? val : 0;
        }
        set
        {
            if (i < _arraySize)
                _internalMemory[i] = value;
            else
                _additionalMemory[i] = value;
        }
    }
}