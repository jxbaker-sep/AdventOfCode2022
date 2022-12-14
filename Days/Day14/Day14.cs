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
            if (!Sandfall(input, new Position(0, 500), killplane, false))
            {
                return count;
            }
            // DrawIt(input);
        }
        throw new ApplicationException();
    }


    [TestCase(Input.Example, 93)]
    [TestCase(Input.File, 26139)]
    public override long Part2(HashSet<Position> input)
    {
        input = input.ToHashSet();
        var killplane = input.Select(p => p.Y).Max() + 2;
        foreach(var count in Enumerable.Range(0, int.MaxValue))
        {
            if (!Sandfall(input, new Position(0, 500), killplane, true))
            {
                return count;
            }
            // DrawIt(input);
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

    private bool Sandfall(HashSet<Position> input, Position position, long killplane, bool killplaneIsInfinite)
    {
        var s = Vector.South;
        var sw = Vector.South + Vector.West;
        var se = Vector.South + Vector.East;
        if (input.Contains(position)) 
        {
            if (killplaneIsInfinite) return false;
            throw new ApplicationException("Sand hole is blocked.");
        }
        while (position.Y < killplane)
        {
            if (killplaneIsInfinite && position.Y == killplane - 1)
            {
                input.Add(position);
                return true;
            }
            else if (!input.Contains(position + s))
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
        if (killplaneIsInfinite) throw new ApplicationException();
        return false;
    }

    private void DrawIt(HashSet<Position> input)
    {
        var miny = input.Select(p => p.Y).Min();
        var maxy = input.Select(p => p.Y).Max();
        var minx = input.Select(p => p.X).Min();
        var maxx = input.Select(p => p.X).Max();
        Console.WriteLine();
        for(var y = miny; y <= maxy; y++)
        {
            Console.WriteLine();
            for(var x = minx; x <= maxx; x++)
            {
                Console.Write(input.Contains(new(y, x)) ? '#' : '.');
            }
        }
    }


}

public record Day14Line(
    [Format(Separator = "->")]List<Day14Point> Points
);

public record Day14Point(
    long x,
    [Before(",")]long y
);