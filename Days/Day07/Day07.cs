using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day07;

[UsedImplicitly]
public class Day07 : AdventOfCode<long, List<Day05Line>>
{
    public override List<Day05Line> Parse(string input) => input.Lines().Parse<Day05Line>();


    [TestCase(Input.Example, 95437)]
    [TestCase(Input.File, 1642503)]
    public override long Part1(List<Day05Line> input)
    {
        var root = CreateDirTree(input);
        var sentinel = 100_000L;

        return Walk(root).Select(d => TotalSize(d)).Where(ts => ts <= sentinel).Sum();
    }

    [TestCase(Input.Example, 24933642)]
    [TestCase(Input.File, 6999588)]
    public override long Part2(List<Day05Line> input)
    {
        var root = CreateDirTree(input);
        var freespace = 70000000 - TotalSize(root);
        var required = 30000000 - freespace;
        return Walk(root).Select(d => TotalSize(d)).Where(ts => ts >= required).Min();
    }

    private Directory CreateDirTree(List<Day05Line> lines)
    {
        var root = new Directory("/", new(), new(), null);
        var cwd = root;
        foreach(var line in lines)
        {
            if (line.Cd == "/") 
            {
                cwd = root;
            }
            else if (line.Cd == "..")
            {
                cwd = cwd.Parent ?? throw new ApplicationException();
            }
            else if (!string.IsNullOrWhiteSpace(line.Cd))
            {
                cwd = cwd.Directories.First(d => d.Name == line.Cd);
            }
            else if (!string.IsNullOrWhiteSpace(line.Dir))
            {
                cwd.Directories.Add(new Directory(line.Dir, new(), new(), cwd));
            }
            else if (line.File != null)
            {
                cwd.Files.Add(line.File);
            }
        }

        return root;
    }

    private long TotalSize(Directory d)
    {
        return Walk(d).SelectMany(d => d.Files).Sum(f => f.Size);
    }

    private IEnumerable<Directory> Walk(Directory d)
    {
        yield return d;
        foreach(var d2 in d.Directories)
        {
            foreach(var d3 in Walk(d2)) yield return d3;
        }
    }
}

public record Directory(string Name, List<Directory> Directories, List<File> Files, Directory? Parent);

public record File(long Size, [Format(Regex = ".*")]string Name);

public record Day05Line(
    [Alternate, Format(Regex = ".*", Before = "$ cd")] string Cd,
    [Alternate, Format(Regex = "", Before = "$ ls")] string Ls,
    [Alternate, Before("dir")] string Dir,
    [Alternate] File? File
);