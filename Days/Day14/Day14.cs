using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day14;

[UsedImplicitly]
public class Day14 : AdventOfCode<long, HashSet<Position>>
{
    public override HashSet<Position> Parse(string input)
    {
        var result = new HashSet<Position>();
        input.Lines()
            .ToList()
            .ForEach(line => {
                var points = line.Parse<Day14Line>();
                foreach(var pair in points.Points.Windows(2))
                {
                    DrawLine(result, pair[0], pair[1]);
                }
            });
        return result;
    }


    [TestCase(Input.Example, 24)]
    [TestCase(Input.File, 1330)]
    public override long Part1(HashSet<Position> input)
    {
        input = input.ToHashSet();
        var killplane = input.Select(p => p.Y).Max() + 1;
        foreach(var count in Enumerable.Range(0, int.MaxValue))
        {
            if (!Sandfall(input, new Position(0, 500), killplane))
            {
                return count;
            }
        }
        throw new ApplicationException();
    }


    [TestCase(Input.Example, 93)]
    [TestCase(Input.File, 26139)]
    public override long Part2(HashSet<Position> input)
    {
        input = input.ToHashSet();
        var floor = input.Select(p => p.Y).Max() + 2;
        DrawLine(input, new(500 + floor * 2, floor), new(500 - floor * 2, floor));
        foreach(var count in Enumerable.Range(0, int.MaxValue))
        {
            if (input.Contains(new(0, 500))) return count;
            if (!Sandfall(input, new Position(0, 500), floor + 1))
            {
                throw new ApplicationException();
            }
        }
        throw new ApplicationException();
    }

    private void DrawLine(HashSet<Position> result, Day14Point p1, Day14Point p2)
    {
        var rp1 = new Position(p1.y, p1.x);
        var rp2 = new Position(p2.y, p2.x);
        var v = (rp2 - rp1).Unit;
        while (rp1 != rp2)
        {
            result.Add(rp1);
            rp1 += v;
        }
        result.Add(rp2);
    }

    private bool Sandfall(HashSet<Position> input, Position position, long killplane)
    {
        var s = Vector.South;
        var sw = Vector.South + Vector.West;
        var se = Vector.South + Vector.East;
        if (input.Contains(position)) 
        {
            throw new ApplicationException("Sand hole is blocked.");
        }
        while (position.Y < killplane)
        {
            if (!input.Contains(position + s))
            {
                position += s;
            }
            else if (!input.Contains(position + sw))
            {
                position += sw;
            }
            else if (!input.Contains(position + se))
            {
                position += se;
            }
            else
            {
                input.Add(position);
                return true;
            }
        }
        return false;
    }
}

public record Day14Line(
    [Format(Separator = "->")]List<Day14Point> Points
);

public record Day14Point(
    long x,
    [Before(",")]long y
);