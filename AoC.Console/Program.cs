using AoC.Common.Models;
using AoC.Runner;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

var services = new ServiceCollection();

services
    .AddSingleton(AnsiConsole.Create(new AnsiConsoleSettings()))
    .AddSingleton<IPuzzleRunner, PuzzleRunner>()
    .AddSingleton<AoCConsole>();

var serviceProvider = services.BuildServiceProvider();

var console = serviceProvider.GetRequiredService<AoCConsole>();

console.Run();

class AoCConsole
{
    private readonly IAnsiConsole _console;
    private readonly IPuzzleRunner _puzzleRunner;

    public AoCConsole(
        IAnsiConsole console,
        IPuzzleRunner puzzleRunner)
    {
        _puzzleRunner = puzzleRunner;
        _console = console;
    }

    public void Run()
    {
        var font = FigletFont.Default;
        var f = new FigletText(font, "Advent of Code")
        {
            Color = ConsoleColor.Green
        };

        _console.Write(f);

        var puzzles = _puzzleRunner.GetPuzzles();

        var years = puzzles.Select(x => x.Year).Distinct().OrderBy(x => x).ToArray();

        if (years.Length == 0)
        {
            _console.Markup("Could not find any puzzles. Exiting.");
            return;
        }

        (var year, puzzles) = PickYear(puzzles, years);

        _console.MarkupLineInterpolated($"Running year [red]{year}[/].");

        var puzzlesToRun = PickPuzzles(puzzles);

        _console.MarkupLineInterpolated($"Running puzzle [red]{string.Join(", ", puzzlesToRun.Select(x => x.Name))}[/].");

        var doBenchmark = _console.Confirm("Do you want to benchmark?", false);

        if (doBenchmark)
        {
            PerformBenchmark(puzzlesToRun.First());
        }
        else
        {
            RunPuzzle(puzzlesToRun);
        }
    }

    private void PerformBenchmark(PuzzleModel puzzleModel)
    {
        _puzzleRunner.BenchmarkPuzzle(puzzleModel);
    }

    private void RunPuzzle(IReadOnlyCollection<PuzzleModel> puzzlesToRun)
    {
        var results = new List<PuzzleResult>();

        _console.Progress()
            .AutoRefresh(false)
            .AutoClear(false)
            .HideCompleted(false)
            .Start(ctx =>
            {
                var at = ctx.AddTask("Run all", true, puzzlesToRun.Count);

                foreach (var puzzle in puzzlesToRun)
                {
                    var t = ctx.AddTask(puzzle.Name, true, 1);

                    var result = _puzzleRunner.RunPuzzle(puzzle);

                    t.Increment(1);
                    t.StopTask();
                    at.Increment(1);
                    ctx.Refresh();

                    results.Add(result);
                }

                at.StopTask();
                ctx.Refresh();
            });

        foreach (var result in results)
        {
            var grid = new Grid();

            grid.AddColumns(2);

            var res1Panel = new Panel(result.Part1).Header("Part 1");
            var res2Panel = new Panel(result.Part2).Header("Part 2");

            grid.AddRow(res1Panel, res2Panel);
            grid.AddRow(new Markup($"Took {result.ElapsedMsPart1}ms.").Alignment(Justify.Center), new Markup($"Took {result.ElapsedMsPart2}ms.").Alignment(Justify.Center));

            var statsPanel = new Panel(grid).Header($"{result.Puzzle.Year} Day {result.Puzzle.Day:D2}");

            _console.Write(statsPanel);
        }
    }

    private static IReadOnlyCollection<PuzzleModel> PickPuzzles(IReadOnlyCollection<PuzzleModel> puzzles)
    {
        var days = puzzles.OrderBy(x => x.Day).ToArray();

        var day = days[^1].Day;

        if (days.Length > 1)
        {
            var chosenDay = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What [green]day[/] do you want to execute?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more years)[/]")
                    .AddChoices(days.Select(x => $"{x.Day} {x.Name}").Prepend("Latest")));

            if (int.TryParse(chosenDay.Split(' ')[0], out var chosenDayInt))
            {
                day = chosenDayInt;
            }
        }

        return puzzles.Where(x => x.Day == day).ToList();
    }

    private static (int, IReadOnlyCollection<PuzzleModel>) PickYear(IReadOnlyCollection<PuzzleModel> puzzles, int[] years)
    {
        var year = years[^1];

        if (years.Length > 1)
        {
            var chosenYear = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What [green]year[/] do you want to execute?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more years)[/]")
                    .AddChoices(years.Select(x => x.ToString())
                        .Prepend("Latest")));

            if (int.TryParse(chosenYear, out var chosenYearInt))
            {
                year = chosenYearInt;
            }
        }

        return (year, puzzles.Where(x => x.Year == year).ToArray());
    }
}