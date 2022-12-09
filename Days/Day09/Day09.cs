using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day09;

[UsedImplicitly]
public class Day09 : AdventOfCode<long, List<Day09Input>>
{
    public override List<Day09Input> Parse(string input) => input.Lines().Parse<Day09Input>();


    [TestCase(Input.Example, 13)]
    [TestCase(Input.File, 5878)]
    public override long Part1(List<Day09Input> input)
    {
        return Snake(2, input);
    }

    [TestCase(Input.Example, 1)]
    [TestCase(Input.Raw, 36, Raw = Example2)]
    [TestCase(Input.File, 2405)]
    public override long Part2(List<Day09Input> input)
    {
        return Snake(10, input);
    }

    private long Snake(int snakeLength, List<Day09Input> input)
    {
        var rope = Enumerable.Range(0, snakeLength).Select(_ => new Position(0,0)).ToList();
        var visited = new HashSet<Position>{rope.Last()};
        var dmap = new Dictionary<Direction, Vector>
        {
            { Direction.D, Vector.South },
            { Direction.L, Vector.West },
            { Direction.R, Vector.East },
            { Direction.U, Vector.North }
        };
        foreach(var item in input)
        {
            foreach(var _ in Enumerable.Range(0, item.Magnitude))
            {
                rope = MoveRope(rope, dmap[item.Direction]);
                visited.Add(rope.Last());
            }
        }
        return visited.Count;
    }

    List<Position> MoveRope(List<Position> rope, Vector start)
    {
        var result = new List<Position>{rope[0] + start};
        for (var current = 1; current < rope.Count; current++)
        {
            var head = result.Last();
            var tail = rope[current];
            var delta = head - tail;
            if (Math.Abs(delta.X) == 2 || Math.Abs(delta.Y) == 2)
            {
                if (delta.X > 0) tail += Vector.East;
                else if (delta.X < 0) tail += Vector.West;
                if (delta.Y > 0) tail += Vector.South;
                else if (delta.Y < 0) tail += Vector.North;
            }
            result.Add(tail);
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

public enum Direction {U, D, R, L};
public record Day09Input(Direction Direction, int Magnitude);