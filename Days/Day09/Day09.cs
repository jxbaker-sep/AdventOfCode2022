using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day09;

[UsedImplicitly]
public class Day09 : AdventOfCode<long, IReadOnlyList<Vector>>
{
    public override IReadOnlyList<Vector> Parse(string input) => input.Lines().Select(line => 
        line[0] switch
        {
            'D' => Vector.South,
            'U' => Vector.North,
            'L' => Vector.West,
            'R' => Vector.East,
            _ => throw new ApplicationException()
        } * Convert.ToInt32(line.Substring(2))
    ).ToList();


    [TestCase(Input.Example, 13)]
    [TestCase(Input.File, 5878)]
    public override long Part1(IReadOnlyList<Vector> input)
    {
        return Snake(2, input);
    }

    [TestCase(Input.Example, 1)]
    [TestCase(Input.Raw, 36, Raw = Example2)]
    [TestCase(Input.File, 2405)]
    public override long Part2(IReadOnlyList<Vector> input)
    {
        return Snake(10, input);
    }

    private long Snake(int snakeLength, IReadOnlyList<Vector> input)
    {
        var rope = Enumerable.Range(0, snakeLength).Select(_ => new Position(0,0)).ToList();
        var visited = new HashSet<Position>{rope.Last()};
        foreach(var vector in input)
        {
            foreach(var _ in Enumerable.Range(0, (int)vector.Magnitude))
            {
                rope = MoveRope(rope, vector.Unit);
                visited.Add(rope.Last());
            }
        }
        return visited.Count;
    }

    List<Position> MoveRope(IReadOnlyList<Position> rope, Vector v)
    {
        var result = new List<Position>{rope[0] + v};
        foreach (var tail in rope.Skip(1))
        {
            var head = result.Last();

            result.Add(tail + (head.OrthoganallyOrDiagonallyAdjacent(tail) ? Vector.Zero : (head - tail).Unit));
        }
        return result;
    }

    const string Example2 = @"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20";
}