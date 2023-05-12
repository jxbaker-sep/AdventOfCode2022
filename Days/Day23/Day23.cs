using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day23;

[UsedImplicitly]
public class Day23 : AdventOfCode<long, IReadOnlySet<Position>>
{
    public override IReadOnlySet<Position> Parse(string input) {
        var hs = new HashSet<Position>();
        var p = input.Split("\n").Select(l => l.TrimEnd()).ToList();
        foreach(var row in p.WithIndices()) {
            foreach (var col in row.Value.WithIndices())
            {
                if (col.Value == '#') hs.Add(new Position(row.Index, col.Index));
            }
        }
        return hs;
    }

    [TestCase(Input.Example, 110)]
    [TestCase(Input.File, 3990)]
    public override long Part1(IReadOnlySet<Position> ps)
    {
        foreach(var turn in Enumerable.Range(0, 10))
        {
            ps = TakeOneTurn(ps, turn);
        }
        var minX = ps.Select(p => p.X).Min();
        var minY = ps.Select(p => p.Y).Min();
        var maxX = ps.Select(p => p.X).Max();
        var maxY = ps.Select(p => p.Y).Max();
        return (maxX - minX + 1) * (maxY - minY + 1) - ps.Count();
    }

    [TestCase(Input.Example, 20)]
    [TestCase(Input.File, 1057)]
    public override long Part2(IReadOnlySet<Position> ps)
    {
        foreach(var turn in Enumerable.Range(0, int.MaxValue))
        {
            var copy = TakeOneTurn(ps, turn);
            if (copy.Union(ps).Count() == ps.Count) return turn + 1;
            ps = copy;
        }
        throw new ApplicationException();
    }

    private IReadOnlySet<Position> TakeOneTurn(IReadOnlySet<Position> positions, int turn)
    {
        var proposed = new Dictionary<Position, List<Position>>();
        foreach(var position in positions)
        {
            if (position.DiagonalAndOrthoganalNeighbors().All(adjacent => !positions.Contains(adjacent)))
            {
                proposed.InsertIntoList(position, position);
                continue;
            }

            var rules = new[] {
                new { checkedVectors = new [] { Vector.North, Vector.North + Vector.East, Vector.North + Vector.West }, proposedVector = Vector.North  },
                new { checkedVectors = new [] { Vector.South, Vector.South + Vector.East, Vector.South + Vector.West }, proposedVector = Vector.South  },
                new { checkedVectors = new [] { Vector.West,  Vector.North + Vector.West, Vector.South + Vector.West }, proposedVector = Vector.West  },
                new { checkedVectors = new [] { Vector.East,  Vector.North + Vector.East, Vector.South + Vector.East }, proposedVector = Vector.East  },
            };
            var someProposed = false;
            foreach (var offset in Enumerable.Range(0, 4))
            {
                var rule = rules[(offset + turn) % 4];
                if (rule.checkedVectors.All(v => !positions.Contains(position + v))) {
                    proposed.InsertIntoList(position + rule.proposedVector, position);
                    someProposed = true;
                    break;
                }
            }
            if (!someProposed) proposed.InsertIntoList(position, position);
        }

        var unmoved = proposed.Where(kv => kv.Value.Count != 1).SelectMany(kv => kv.Value);
        var moved = proposed.Where(kv => kv.Value.Count == 1).Select(kv => kv.Key);
        var result = unmoved.Concat(moved).ToHashSet();
        if (result.Count() != positions.Count) throw new ApplicationException();
        return result;
    }
}
