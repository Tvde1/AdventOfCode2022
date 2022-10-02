using AoC.Common.Models;

namespace AoC.Runner;

public interface IPuzzleRunner
{
    public IReadOnlyCollection<PuzzleModel> GetPuzzles();

    public PuzzleResult RunPuzzle(PuzzleModel puzzle);
    public void BenchmarkPuzzle(PuzzleModel puzzle);
}
