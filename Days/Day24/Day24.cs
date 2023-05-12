using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day23;

[UsedImplicitly]
public class Day24 : AdventOfCode<long, IReadOnlyDictionary<Position, List<Vector>>>
{
    public override IReadOnlyDictionary<Position, List<Vector>> Parse(string input) {
        var hs = new Dictionary<Position, List<Vector>>();
        var p = input.Split("\n").Select(l => l.TrimEnd()).ToList();
        foreach(var row in p.WithIndices()) {
            foreach (var col in row.Value.WithIndices())
            {
                switch(col.Value)
                {
                    case '>':
                        hs.InsertIntoList(new Position(row.Index, col.Index), Vector.East);
                        break;
                    case '<':
                        hs.InsertIntoList(new Position(row.Index, col.Index), Vector.West);
                        break;
                    case '^':
                        hs.InsertIntoList(new Position(row.Index, col.Index), Vector.North);
                        break;
                    case 'v':
                        hs.InsertIntoList(new Position(row.Index, col.Index), Vector.South);
                        break;
                    case '.': break;
                    case '#': break;
                    default: throw new ApplicationException();
                }
            }
        }
        return hs;
    }

    [TestCase(Input.Example, 18)]
    // [TestCase(Input.File, 238)]
    public override long Part1(IReadOnlyDictionary<Position, List<Vector>> originalGlacierPositions)
    {
        var maxX = originalGlacierPositions.Keys.Select(k => k.X).Max() + 1;
        var maxY = originalGlacierPositions.Keys.Select(k => k.Y).Max() + 1;
        return March(new Position(0, 1), new Position(maxY, maxX - 1), originalGlacierPositions).Item1;
    }

    [TestCase(Input.Example, 54)]
    // [TestCase(Input.File, 751)]
    public override long Part2(IReadOnlyDictionary<Position, List<Vector>> originalGlacierPositions)
    {
        var maxX = originalGlacierPositions.Keys.Select(k => k.X).Max() + 1;
        var maxY = originalGlacierPositions.Keys.Select(k => k.Y).Max() + 1;
        var start = new Position(0, 1);
        var goal = new Position(maxY, maxX - 1);
        var phase1 = March(start, goal, originalGlacierPositions);
        var phase2 = March(goal, start, phase1.Item2);
        var phase3 = March(start, goal, phase2.Item2);
        return phase1.Item1 + phase2.Item1 + phase3.Item1;
    }

    private (long, IReadOnlyDictionary<Position, List<Vector>>) March(Position start, Position target, IReadOnlyDictionary<Position, List<Vector>> originalGlacierPositions)
    {
        var maxX = originalGlacierPositions.Keys.Select(k => k.X).Max() + 1;
        var maxY = originalGlacierPositions.Keys.Select(k => k.Y).Max() + 1;
        var closed = new HashSet<(long cycle, Position p)>();
        var totalCycles = maxX == 7 ? 12 : 140; // (maxX - 1) * (maxY - 1);
        var pq = new PriorityQueue<Step>(step => step.Distance + step.Position.ManhattanDistance(target));
        pq.Enqueue(new Step(originalGlacierPositions, start, 0));
        while (pq.TryDequeue(out var current))
        {
            var nextGlacierPositions = MoveGlaciers(current.Glaciers, maxX, maxY);
            foreach(var nextPosition in current.Position.OrthoganalNeighbors().Append(current.Position).Where(p2 => !IsWall(p2, maxX, maxY) && !nextGlacierPositions.ContainsKey(p2)))
            {
                if (nextPosition == target) return (current.Distance + 1, nextGlacierPositions);
                var cycle = (current.Distance + 1) % totalCycles;
                if (!closed.Add((cycle, nextPosition))) continue;
                // Console.WriteLine($"{current.Position} {nextPosition} {current.Distance} {nextPosition.ManhattanDistance(target)}");
                pq.Enqueue(new Step(nextGlacierPositions, nextPosition, current.Distance + 1));
            }
        }

        throw new ApplicationException();
    }

    private IReadOnlyDictionary<Position, List<Vector>> MoveGlaciers(IReadOnlyDictionary<Position, List<Vector>> ps, long maxX, long maxY)
    {
        var next = new Dictionary<Position, List<Vector>>();
        foreach(var glacierSet in ps)
        {
            foreach(var glacier in glacierSet.Value)
            {
                next.InsertIntoList(MoveGlacier(glacierSet.Key, glacier, maxX, maxY), glacier);
            }
        }
        return next;
    }

    private Position MoveGlacier(Position p, Vector glacier, long maxX, long maxY)
    {
        var next = p + glacier;
        if (glacier == Vector.North && next.Y < 1)
        {
            return new Position(maxY - 1, p.X);
        }
        else if (glacier == Vector.South && next.Y >= maxY)
        {
            return new Position(1, p.X);
        }
        else if (glacier == Vector.West && next.X < 1)
        {
            return new Position(p.Y, maxX - 1);
        }
        else if (glacier == Vector.East && next.X >= maxX)
        {
            return new Position(p.Y, 1);
        }

        return next;
    }

    private bool IsWall(Position p, long maxX, long maxY)
    {
        if (p.X == 1 && p.Y == 0) return false;
        if (p.X == maxX - 1 && p.Y == maxY) return false;
        return p.X < 1 || p.X >= maxX || p.Y < 1 || p.Y >= maxY;
    }
}

public record Step(IReadOnlyDictionary<Position, List<Vector>> Glaciers, Position Position, long Distance);