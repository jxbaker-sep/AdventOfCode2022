using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day12;

[UsedImplicitly]
public class Day12 : AdventOfCode<long, Day12Input>
{
    public override Day12Input Parse(string input)
    {
        var g = input.Lines().Select(c => c.Select(it => it switch 
        {
            'S' => 0,
            'E' => 'z' - 'a',
            >= 'a' and <= 'z' => it - 'a',
            _ => throw new ApplicationException()
        }).ToList()).ToList();
        var start = input.Lines().SelectMany((line, row) => line.Select((c, col) => (row, col, c))).First(it => it.c == 'S');
        var end = input.Lines().SelectMany((line, row) => line.Select((c, col) => (row, col, c))).First(it => it.c == 'E');
        return new(g, new(start.row, start.col), new(end.row, end.col));
    }

    [TestCase(Input.Example, 31)]
    [TestCase(Input.File, 481)]
    public override long Part1(Day12Input input)
    {
        return FindRoute(input.Grid, input.Start, input.End) ?? throw new ApplicationException();
    }

    [TestCase(Input.Example, 29)]
    [TestCase(Input.File, 480)]
    public override long Part2(Day12Input input)
    {
        var startPositions = input.Grid.SelectMany((line, row) => line.Select((c, col) => (row, col, c))).Where(it => it.c == 0).ToList();
        return startPositions.Select(position => FindRoute(input.Grid, new(position.row, position.col), input.End)).OfType<long>().Min();
    }

    private long? FindRoute(List<List<int>> grid, Position start, Position end)
    {
        var closed = new Dictionary<Position, long>
        {
            { start, 0 }
        };
        var open = new Queue<Position>();
        open.Enqueue(start);
        while (open.TryDequeue(out var current))
        {
            foreach(var next in current.Orthogonals()
                .Where(next => !closed.ContainsKey(next))
                .Where(next => next.TryLookup(grid, out var nextElevation) && nextElevation <= current.Lookup(grid) + 1))
            {
                open.Enqueue(next);
                closed[next] = closed[current] + 1;
                if (next == end) return closed[next];
            }
        }
        return null;
    }
}

public record Day12Input(
    List<List<int>> Grid,
    Position Start,
    Position End
);