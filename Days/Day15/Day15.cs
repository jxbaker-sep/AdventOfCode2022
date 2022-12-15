using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day15;

[UsedImplicitly]
public class Day15 : AdventOfCode<long, List<(Position Sensor, Position Beacon)>>
{
    public override List<(Position Sensor, Position Beacon)> Parse(string input)
    {
        return input.Lines().Parse<Day15Input>()
            .Select(l => (new Position(l.y1, l.x1), new Position(l.y2, l.x2)))
            .ToList();
    }


    // [TestCase(Input.Example, 26)]
    // [TestCase(Input.File, 5832528)]
    public override long Part1(List<(Position Sensor, Position Beacon)> input)
    {
        var row = input[0].Sensor.X == 2 ? 10 : 2000000;
        var beacons = input.Select(it => it.Beacon).ToHashSet();
        var s2md = input.Select(it => new {Sensor = it.Sensor, Md = it.Sensor.ManhattanDistance(it.Beacon)})
            .ToList();
        
        var minx = s2md.Select(it => it.Sensor.X - it.Md).Min();
        var maxx = s2md.Select(it => it.Sensor.X + it.Md).Max();

        var scanned = new HashSet<Position>();
        foreach (var item in s2md)
        {
            var xsq = XsWithinManhattanDistance(item.Sensor, row, item.Md);
            if (xsq is not {} xs) continue;
            for (var x = xs.Item1; x <= xs.Item2; x++)
            {
                scanned.Add(new(row, x));
            }
        }

        return scanned.Count - beacons.Count(beacon => beacon.Y == row);
    }


    // [TestCase(Input.Example, 56000011)]
    // [TestCase(Input.File, 13360899249595)]
    public override long Part2(List<(Position Sensor, Position Beacon)> input)
    {
        var maxCoordinate = input[0].Sensor.X == 2 ? 20 : 4_000_000;
        var tuningMultiplier = 4000000;
        var s2md = input.Select(it => new {Sensor = it.Sensor, Md = it.Sensor.ManhattanDistance(it.Beacon)})
            .ToList();
        
        for(var y = 0; y <= maxCoordinate; y++)
        {
            if (y % 100000 == 0) Console.WriteLine(y);
            var xss = s2md.Select(it => XsWithinManhattanDistance(it.Sensor, y, it.Md))
                .OfType<(long, long)>()
                .ToList();
            foreach(var xs in xss)
            {
                var x = xs.Item1 - 1;
                if (x >= 0 && x <= maxCoordinate && !xss.Any(xs2 => x >= xs2.Item1 && x <= xs2.Item2))
                {
                    return x * tuningMultiplier + y;
                }
                x = xs.Item2 + 1;
                if (x >= 0 && x <= maxCoordinate && !xss.Any(xs2 => x >= xs2.Item1 && x <= xs2.Item2))
                {
                    return x * tuningMultiplier + y;
                }
            }
        }

        throw new ApplicationException();
    }

    private (long, long)? XsWithinManhattanDistance(Position p, long row, long distance)
    {
        var delta = Math.Abs(p.Y - row);
        if (delta > distance) return null;
        var width = distance - delta;
        return (p.X - width, p.X + width);
    }
}

public record Day15Input(
    [Before("Sensor at x=")]long x1,
    [Before(", y=")]long y1,
    [Before(": closest beacon is at x=")]long x2,
    [Before(", y=")]long y2
);