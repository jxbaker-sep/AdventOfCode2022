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


    [TestCase(Input.Example, 26)]
    [TestCase(Input.File, 5832528)]
    public override long Part1(List<(Position Sensor, Position Beacon)> input)
    {
        var row = input[0].Sensor.X == 2 ? 10 : 2000000;
        var beacons = input.Select(it => it.Beacon).ToHashSet();
        var sensorToManhattanDistance = input.Select(it => (it.Sensor, it.Sensor.ManhattanDistance(it.Beacon)))
            .ToDictionary(it => it.Item1, it => it.Item2);
        
        var minx = sensorToManhattanDistance.Select(it => it.Key.X - it.Value).Min();
        var maxx = sensorToManhattanDistance.Select(it => it.Key.X + it.Value).Max();

        var count = 0;
        for (var x = minx; x <= maxx; x++)
        {
            if (sensorToManhattanDistance.Any(it => it.Key.ManhattanDistance(new(row, x)) <= it.Value))
            {
                count += 1;
            }
        }

        return count - beacons.Count(beacon => beacon.Y == row);
    }


    [TestCase(Input.Example, 0)]
    [TestCase(Input.File, 0)]
    public override long Part2(List<(Position Sensor, Position Beacon)> input)
    {
        return 0;
    }
}

public record Day15Input(
    [Before("Sensor at x=")]long x1,
    [Before(", y=")]long y1,
    [Before(": closest beacon is at x=")]long x2,
    [Before(", y=")]long y2
);