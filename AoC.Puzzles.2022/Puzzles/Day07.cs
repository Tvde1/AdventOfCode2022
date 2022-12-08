using System.Diagnostics;
using System.Text;
using AoC.Common.Attributes;
using AoC.Common.Interfaces;

namespace AoC.Puzzles._2022.Puzzles;

[Puzzle(2022, 7, "No Space Left On Device")]
public class Day07 : IPuzzle<TerminalOutput>
{
    public TerminalOutput Parse(string inputText)
    {
        // inputText = """
        // $ cd /
        // $ ls
        // dir a
        // 14848514 b.txt
        // 8504156 c.dat
        // dir d
        // $ cd a
        // $ ls
        // dir e
        // 29116 f
        // 2557 g
        // 62596 h.lst
        // $ cd e
        // $ ls
        // 584 i
        // $ cd ..
        // $ cd ..
        // $ cd d
        // $ ls
        // 4060174 j
        // 8033020 d.log
        // 5626152 d.ext
        // 7214296 k
        // """;
        //
        return TerminalOutput.Parse(inputText);
    }

    public string Part1(TerminalOutput input)
    {
        var graph = BuildGraph(input);

        // Console.WriteLine(graph.ToString());

        var directoriesWithSizeAtMostOneHundredThousand = new List<(string Name, long Size)>();

        var sum = 0L;
        
        graph.Traverse(dir =>
        {
            var totalDirSize = dir.TotalSize;
            if (totalDirSize <= 100_000)
            {
                sum += totalDirSize;
            }
        });
        
        return sum.ToString();
    }

    public string Part2(TerminalOutput input)
    {
        const long totalSpace = 70_000_000;
        const long requiredFreeSpace = 30_000_000;
        
        var graph = BuildGraph(input);

        var currentFreeSpace = totalSpace - graph.TotalSize;
        var spaceToFree = requiredFreeSpace - currentFreeSpace;

        var potentialDirSizeToDelete = long.MaxValue;
        
        graph.Traverse(dir =>
        {
            var totalDirSize = dir.TotalSize;
            if (totalDirSize > spaceToFree && totalDirSize < potentialDirSizeToDelete)
            {
                potentialDirSizeToDelete = totalDirSize;
            }
        });

        return potentialDirSizeToDelete.ToString();
    }

    private static TerminalDir BuildGraph(TerminalOutput terminalOutput)
    {
        Debug.Assert(terminalOutput.Commands.First() is ChangeDirectoryTerminalCommand { RelativePath: "/", });

        var root = new TerminalDir("/", null, new(), new());
        var current = root;

        foreach (var command in terminalOutput.Commands.Skip(1))
        {
            switch (command)
            {
                case ChangeDirectoryTerminalCommand cdCommand:
                {
                    HandleCdCommand(ref current, cdCommand);
                    break;
                }
                case ListDirectoryTerminalCommand lsCommand:
                {
                    HandleLsCommand(current, lsCommand);
                    break;
                }
            }
        }

        return root;
    }

    private static void HandleCdCommand(ref TerminalDir current, ChangeDirectoryTerminalCommand command)
    {
        if (string.Equals(command.RelativePath, "..", StringComparison.CurrentCultureIgnoreCase))
        {
            Debug.Assert(current.Parent is not null);
            current = current.Parent;
            return;
        }

        current = current.Directories.First(x => x.Name == command.RelativePath);
    }

    private static void HandleLsCommand(TerminalDir current, ListDirectoryTerminalCommand lsCommand)
    {
        foreach (var listDirectoryTerminalCommandOutput in lsCommand.Outputs)
        {
            switch (listDirectoryTerminalCommandOutput)
            {
                case ListDirectoryTerminalCommandDirectoryOutput listDirectoryTerminalCommandDirectoryOutput:
                    current.Directories.Add(new TerminalDir(listDirectoryTerminalCommandOutput.Name, current, 
                        new(), new()));
                    break;
                case ListDirectoryTerminalCommandFileOutput listDirectoryTerminalCommandFileOutput:
                    current.Files.Add(new TerminalFile(listDirectoryTerminalCommandOutput.Name,
                        listDirectoryTerminalCommandFileOutput.Size));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(listDirectoryTerminalCommandOutput));
            }
        }
    }
}

public record TerminalDir(string Name, TerminalDir? Parent, List<TerminalDir> Directories, List<TerminalFile> Files)
{
    public long TotalSize => Files.Sum(x => x.Size) + Directories.Sum(x => x.TotalSize);

    public void Traverse(Action<TerminalDir> dirActions, Action<TerminalFile>? fileActions = null)
    {
        if (fileActions is not null)
        {
            foreach (var file in Files)
            {
                fileActions(file);
            }
        }

        foreach (var dir in Directories)
        {
            dirActions(dir);
            dir.Traverse(dirActions, fileActions);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        ToString(sb, 0);
        return sb.ToString();
    }

    private void ToString(StringBuilder stringBuilder, int indentations)
    {
        stringBuilder.Append(' ', indentations * 2);
        stringBuilder.Append("- ");
        stringBuilder.Append(Name);
        stringBuilder.Append(" (dir, TotalSize=");
        stringBuilder.Append(TotalSize);
        stringBuilder.AppendLine(")");

        foreach (var terminalDir in Directories)
        {
            terminalDir.ToString(stringBuilder, indentations + 1);
        }
        
        foreach (var terminalFile in Files)
        {
            stringBuilder.Append(' ', (indentations + 1) * 2);
            stringBuilder.Append("- ");
            stringBuilder.Append(terminalFile.Name);
            stringBuilder.Append(" (file, Size=");
            stringBuilder.Append(terminalFile.Size);
            stringBuilder.AppendLine(")");
        }
    }
}

public record TerminalFile(string Name, long Size);

#region Terminal Output

public record TerminalOutput
{
    public List<TerminalCommand> Commands { get; }

    private TerminalOutput(List<TerminalCommand> commands)
    {
        Commands = commands;
    }

    public static TerminalOutput Parse(string input)
    {
        var commands = new List<TerminalCommand>();

        TerminalCommand? currentCommand = null;
        var sp = input.AsSpan();

        foreach (var line in sp.EnumerateLines())
        {
            if (line[0] == '$')
            {
                if (currentCommand is not null)
                {
                    commands.Add(currentCommand);
                }

                currentCommand = line[2..4] switch
                {
                    ['c', 'd'] => new ChangeDirectoryTerminalCommand(line[5..].ToString()),
                    ['l', 's'] => new ListDirectoryTerminalCommand(),
                    _ => throw new InvalidOperationException($"Unknown command: {line[2..4]}"),
                };
                continue;
            }

            Debug.Assert(currentCommand is not null);
            currentCommand.ContinueParse(line);
        }
        
        if (currentCommand is not null)
        {
            commands.Add(currentCommand);
        }

        return new TerminalOutput(commands);
    }
}

public abstract record TerminalCommand()
{
    public abstract void ContinueParse(ReadOnlySpan<char> line);
}

public record ChangeDirectoryTerminalCommand(string RelativePath) : TerminalCommand
{
    public override void ContinueParse(ReadOnlySpan<char> line) =>
        throw new InvalidOperationException("A cd command should not have any more lines");
}

public record ListDirectoryTerminalCommand : TerminalCommand
{
    public List<ListDirectoryTerminalCommandOutput> Outputs { get; } = new();

    public override void ContinueParse(ReadOnlySpan<char> line)
    {
        Outputs.Add(ListDirectoryTerminalCommandOutput.Parse(line));
    }
}

public abstract record ListDirectoryTerminalCommandOutput(string Name)
{
    public static ListDirectoryTerminalCommandOutput Parse(ReadOnlySpan<char> line)
    {
        if (line.StartsWith("dir"))
        {
            return new ListDirectoryTerminalCommandDirectoryOutput(line[4..].ToString());
        }

        var spaceIndex = line.IndexOf(' ');

        var size = long.Parse(line[..spaceIndex]);
        var name = line[(spaceIndex + 1)..].ToString();

        return new ListDirectoryTerminalCommandFileOutput(name, size);
    }
}

public record ListDirectoryTerminalCommandFileOutput(string Name, long Size)
    : ListDirectoryTerminalCommandOutput(Name);

public record ListDirectoryTerminalCommandDirectoryOutput(string Name)
    : ListDirectoryTerminalCommandOutput(Name);

#endregion