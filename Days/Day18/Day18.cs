using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day18;

[UsedImplicitly]
public class Day18 : AdventOfCode<long, IReadOnlyList<Position3d>>
{
    public override IReadOnlyList<Position3d> Parse(string input) => input.Split("\n").Select(it =>
    {
        var x = it.Split(",").Select(n => Convert.ToInt64(n)).ToArray();
        return new Position3d(x[0], x[1], x[2]);
    }).ToList();


    [TestCase(Input.Raw, 10, Raw = "1,1,1\n2,1,1")]
    [TestCase(Input.Example, 64)]
    [TestCase(Input.File, 4608)]
    public override long Part1(IReadOnlyList<Position3d> cubes)
    {
        return cubes.Select(cube => cube.Orthoganals().Except(cubes).Count()).Sum();
    }


    [TestCase(Input.Example, 58)]
    [TestCase(Input.File, 2652)]
    public override long Part2(IReadOnlyList<Position3d> cubes)
    {
        var airPockets = ComputeAirPockets(cubes);
        return cubes.Select(cube => cube.Orthoganals().Except(cubes).Except(airPockets).Count()).Sum();
    }

    private IReadOnlyList<Position3d> ComputeAirPockets(IReadOnlyList<Position3d> cubes)
    {
        var bounds = Bounds(cubes);
        var pockets = new HashSet<Position3d>();
        foreach (var cube in cubes)
        {
            foreach (var adjacent in cube.Orthoganals().Except(cubes))
            {
                if (!CanReachAir(adjacent, cubes, bounds))
                {
                    pockets.Add(adjacent);
                }
            }
        }
        return pockets.ToList();
    }

    HashSet<Position3d> knownExposed = new();
    HashSet<Position3d> knownAirPockets = new();
    private bool CanReachAir(Position3d p, IReadOnlyList<Position3d> cubes, Bounds b)
    {
        var closed = new HashSet<Position3d> { p };
        var open = new Queue<Position3d>();
        open.Enqueue(p);
        var result = false;
        while (open.TryDequeue(out var current))
        {
            if (knownExposed.Contains(current))
            {
                result = true;
                break;
            }

            if (knownAirPockets.Contains(current))
            {
                break;
            }

            foreach (var adjacent in current.Orthoganals())
            {
                if (!InBounds(adjacent, b) || knownExposed.Contains(adjacent))
                {
                    result = true;
                    break;
                }
                if (cubes.Contains(adjacent)) continue;
                if (closed.Contains(adjacent)) continue;
                closed.Add(adjacent);
                open.Enqueue(adjacent);
            }
        }

        if (result)
        {
            foreach (var item in closed)
            {
                knownExposed.Add(item);
            }

            return true;
        }

        foreach (var item in closed)
        {
            knownAirPockets.Add(item);
        }

        return false;
    }

    private Bounds Bounds(IReadOnlyList<Position3d> cubes)
    {
        var x1 = cubes.Select(cube => cube.X).Min();
        var x2 = cubes.Select(cube => cube.X).Max();
        var y1 = cubes.Select(cube => cube.Y).Min();
        var y2 = cubes.Select(cube => cube.Y).Max();
        var z1 = cubes.Select(cube => cube.Z).Min();
        var z2 = cubes.Select(cube => cube.Z).Max();
        return new(x1, x2, y1, y2, z1, z2);
    }

    private bool InBounds(Position3d p, Bounds b)
    {
        if (p.X < b.X1) return false;
        if (p.X > b.X2) return false;
        if (p.Y < b.Y1) return false;
        if (p.Y > b.Y2) return false;
        if (p.Z < b.Z1) return false;
        if (p.Z > b.Z2) return false;
        return true;
    }
}

public record Bounds(long X1, long X2, long Y1, long Y2, long Z1, long Z2);
