using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day22;

[UsedImplicitly]
public class Day22 : AdventOfCode<long, MapAndInstructions>
{
    public override MapAndInstructions Parse(string input) {
        var p = input.Split("\n").Select(l => l.TrimEnd()).ToList();
        var lastLine = p.Last();
        p = p.Take(p.Count - 2).ToList();
        var d = new Dictionary<Position, Tile>();
        foreach(var row in p.WithIndices()) {
            foreach(var col in row.Value.WithIndices()) {
                if (col.Value == '.') d[new(row.Index + 1, col.Index + 1)] = Tile.Open;
                if (col.Value == '#') d[new(row.Index + 1, col.Index + 1)] = Tile.Wall;
            }
        }
        var instructions = Regex.Matches(lastLine, @"(?<distance>\d+)|(?<turn>R|L)").Select(m => new Instruction(m.OptionalLongGroup("distance"), m.OptionalStringGroup("turn")));
        return new MapAndInstructions(d, instructions.ToList(), p[0].Length == 150 ? 50 : 4 );
    }

    [TestCase(Input.Example, 6032)]
    [TestCase(Input.File, 67390)]
    public override long Part1(MapAndInstructions mapAndInstructions)
    {
        return Walk(mapAndInstructions, NextPositionLinear);
    }

    [TestCase(Input.Example, 5031)]
    [TestCase(Input.File, 95291)]
    public override long Part2(MapAndInstructions mapAndInstructions)
    {
        return Walk(mapAndInstructions, (a, b, c) => NextPositionCubic(a, b, c, mapAndInstructions.cubeLength));
    }

    private long Walk(MapAndInstructions mapAndInstructions, Func<IReadOnlyDictionary<Position, Tile>, Position, Vector, (Position, Vector)> nextPosition)
    {
        var map = mapAndInstructions.Map;
        var instructions = mapAndInstructions.Instructions;
        var facing = Vector.East;
        var position = new Position(1, 1 + (mapAndInstructions.cubeLength == 50 ? 50 :  8));
        // Console.WriteLine($"");
        // Console.WriteLine($"{position} ; {FacingWord(facing)}");
        foreach (var instruction in instructions)
        {
            if (instruction is { Turn: "R" })
            {
                facing = facing.RotateRight();
            }
            if (instruction is { Turn: "L" })
            {
                facing = facing.RotateLeft();
            }
            if (instruction.Distance is { } distance)
            {
                foreach (var _ in Enumerable.Range(0, (int)distance))
                {
                    // if (position.Y==1 && position.X==114)
                    //     Console.WriteLine($"{position}, {facing}");
                    var next = nextPosition(map, position, facing);
                    
                    var nextSpace = map[next.Item1];

                    if (nextSpace == Tile.Wall)
                    {
                        break;
                    }
                    facing = next.Item2;
                    position = next.Item1;
                }
            }
            // Console.WriteLine($"{instruction.Distance?.ToString() ?? instruction.Turn ?? throw new ApplicationException()}: {position} {FacingWord(facing)}");
        }
        return position.Y * 1000 + position.X * 4 + facing switch
        {
            _ when facing == Vector.East => 0,
            _ when facing == Vector.South => 1,
            _ when facing == Vector.West => 2,
            _ when facing == Vector.North => 3,
            _ => throw new ApplicationException()
        };
    }

    private string FacingWord(Vector facing)
    {
        return new Dictionary<Vector, string> { 
            { Vector.North, "^" },
            { Vector.South, "v" },
            { Vector.East, ">" },
            { Vector.West, "<" },
        }[facing];
    }

    private (Position, Vector) NextPositionLinear(IReadOnlyDictionary<Position, Tile> map, Position start, Vector facing)
    {
        var next = start + facing;
        if (map.ContainsKey(next)) return (next, facing);

        var x = facing switch {
            _ when facing == Vector.East => map.Where(p => p.Key.Y == start.Y).MinBy(p => p.Key.X),
            _ when facing == Vector.South => map.Where(p => p.Key.X == start.X).MinBy(p => p.Key.Y),
            _ when facing == Vector.West => map.Where(p => p.Key.Y == start.Y).MaxBy(p => p.Key.X),
            _ when facing == Vector.North => map.Where(p => p.Key.X == start.X).MaxBy(p => p.Key.Y),
            _ => throw new ApplicationException()
        };
        return (x.Key, facing);
    }

    private (Position, Vector) NextPositionCubic(IReadOnlyDictionary<Position, Tile> map, Position start, Vector facing, long cubeLength)
    {
        var next = start + facing;
        if (map.ContainsKey(next)) return (next, facing);

        var cl = GridToCube(start, cubeLength);

        var GetNcl = (int side, Vector destination) => GetNewCubeLocation(cl, side, facing, destination, cubeLength);

        if (cubeLength == 4) {
            var (newCubeLocation, newFacing) = cl.Quadrant switch {
                1 when facing == Vector.North => GetNcl(2, Vector.North),
                1 when facing == Vector.East =>  GetNcl(6, Vector.East),
                1 when facing == Vector.South => GetNcl(4, Vector.North),
                1 when facing == Vector.West =>  GetNcl(3, Vector.North),

                2 when facing == Vector.North => GetNcl(1, Vector.North),
                2 when facing == Vector.East =>  GetNcl(3, Vector.West),
                2 when facing == Vector.South => GetNcl(5, Vector.South),
                2 when facing == Vector.West =>  GetNcl(6, Vector.South),

                3 when facing == Vector.North => GetNcl(1, Vector.West),
                3 when facing == Vector.East =>  GetNcl(4, Vector.West),
                3 when facing == Vector.South => GetNcl(5, Vector.West),
                3 when facing == Vector.West =>  GetNcl(2, Vector.East),

                4 when facing == Vector.North => GetNcl(1, Vector.South),
                4 when facing == Vector.East =>  GetNcl(6, Vector.North),
                4 when facing == Vector.South => GetNcl(5, Vector.North),
                4 when facing == Vector.West =>  GetNcl(3, Vector.East),

                5 when facing == Vector.North => GetNcl(4, Vector.South),
                5 when facing == Vector.East =>  GetNcl(6, Vector.West),
                5 when facing == Vector.South => GetNcl(2, Vector.South),
                5 when facing == Vector.West =>  GetNcl(3, Vector.South),

                6 when facing == Vector.North => GetNcl(4, Vector.East),
                6 when facing == Vector.East =>  GetNcl(1, Vector.East),
                6 when facing == Vector.South => GetNcl(2, Vector.West),
                6 when facing == Vector.West =>  GetNcl(5, Vector.East),

                _ => throw new ApplicationException()
            };
            return (CubeToGrid(newCubeLocation, cubeLength), newFacing);
        }
        else {
            var (newCubeLocation, newFacing) = cl.Quadrant switch {
                1 when facing == Vector.North => GetNcl(6, Vector.West),
                1 when facing == Vector.East =>  GetNcl(2, Vector.West),
                1 when facing == Vector.South => GetNcl(3, Vector.North),
                1 when facing == Vector.West =>  GetNcl(4, Vector.West),

                2 when facing == Vector.North => GetNcl(6, Vector.South),
                2 when facing == Vector.East =>  GetNcl(5, Vector.East),
                2 when facing == Vector.South => GetNcl(3, Vector.East),
                2 when facing == Vector.West =>  GetNcl(1, Vector.East),

                3 when facing == Vector.North => GetNcl(1, Vector.South),
                3 when facing == Vector.East =>  GetNcl(2, Vector.South),
                3 when facing == Vector.South => GetNcl(5, Vector.North),
                3 when facing == Vector.West =>  GetNcl(4, Vector.North),

                4 when facing == Vector.North => GetNcl(3, Vector.West),
                4 when facing == Vector.East =>  GetNcl(5, Vector.West),
                4 when facing == Vector.South => GetNcl(6, Vector.North),
                4 when facing == Vector.West =>  GetNcl(1, Vector.West),

                5 when facing == Vector.North => GetNcl(3, Vector.South),
                5 when facing == Vector.East =>  GetNcl(2, Vector.East),
                5 when facing == Vector.South => GetNcl(6, Vector.East),
                5 when facing == Vector.West =>  GetNcl(4, Vector.East),

                6 when facing == Vector.North => GetNcl(4, Vector.South),
                6 when facing == Vector.East =>  GetNcl(5, Vector.South),
                6 when facing == Vector.South => GetNcl(2, Vector.North),
                6 when facing == Vector.West =>  GetNcl(1, Vector.North),

                _ => throw new ApplicationException()
            };
            return (CubeToGrid(newCubeLocation, cubeLength), newFacing);
        }
    }

    private (CubeLocation, Vector) GetNewCubeLocation(CubeLocation cl, int quadrant, Vector originalVector, Vector destinationSide, long cubeLength) {
        var (x, y) = Angulate(cl, originalVector, destinationSide, cubeLength);
        if (destinationSide == Vector.North) y = 0;
        if (destinationSide == Vector.South) y = cubeLength - 1;
        if (destinationSide == Vector.West) x = 0;
        if (destinationSide == Vector.East) x = cubeLength - 1;
        return (new CubeLocation(quadrant, x, y), destinationSide.Flip());
    }

    private (long X, long Y) Angulate(CubeLocation cl, Vector original, Vector destinationSide, long cubeLength)
    {
        var (x, y) = (cl.X, cl.Y);
        while (original != destinationSide.Flip())
        {
            (original, x, y) = (original.RotateRight(), Invert(y, cubeLength), x);
        }

        return (x, y);
    }

    private long Invert(long d, long cubeLength) => cubeLength - d - 1;

    private Position CubeToGrid(CubeLocation q, long cubeLength)
    {
        if (cubeLength == 4) {
            var dx = q.X % cubeLength;
            var dy = q.Y % cubeLength;
            var (y, x) = q.Quadrant switch
            {
                1 => (0, 2),
                2 => (1, 0),
                3 => (1, 1),
                4 => (1, 2),
                5 => (2, 2),
                6 => (2, 3),
                _ => throw new ApplicationException()
            };
            return new Position(1 + y * cubeLength + dy, 1 + x * cubeLength + dx);
        } else {
            var dx = q.X % cubeLength;
            var dy = q.Y % cubeLength;
            var (y, x) = q.Quadrant switch
            {
                1 => (0, 1),
                2 => (0, 2),
                3 => (1, 1),
                4 => (2, 0),
                5 => (2, 1),
                6 => (3, 0),
                _ => throw new ApplicationException()
            };
            return new Position(1 + y * cubeLength + dy, 1 + x * cubeLength + dx);
        }
    }

    private CubeLocation GridToCube(Position p, long cubeLength)
    {
        if (cubeLength == 4) {
            var x = (p.X - 1) / cubeLength;
            var y = (p.Y - 1) / cubeLength;
            var dx = (p.X - 1) % cubeLength;
            var dy = (p.Y - 1) % cubeLength;
            return (y, x) switch {
                (0, 2) => new(1, dx, dy),
                (1, 0) => new(2, dx, dy),
                (1, 1) => new(3, dx, dy),
                (1, 2) => new(4, dx, dy),
                (2, 2) => new(5, dx, dy),
                (2, 3) => new(6, dx, dy),
                _ => throw new ApplicationException()
            };
        }
        else {
            var x = (p.X - 1) / cubeLength;
            var y = (p.Y - 1) / cubeLength;
            var dx = (p.X - 1) % cubeLength;
            var dy = (p.Y - 1) % cubeLength;
            return (y, x) switch {
                (0, 1) => new(1, dx, dy),
                (0, 2) => new(2, dx, dy),
                (1, 1) => new(3, dx, dy),
                (2, 0) => new(4, dx, dy),
                (2, 1) => new(5, dx, dy),
                (3, 0) => new(6, dx, dy),
                _ => throw new ApplicationException()
            };
        }
    }
}

public enum Tile { Open, Wall };
public record Instruction(long? Distance, string? Turn);
public record MapAndInstructions(IReadOnlyDictionary<Position, Tile> Map, IReadOnlyList<Instruction> Instructions, long cubeLength);
public record CubeLocation(int Quadrant, long X, long Y);