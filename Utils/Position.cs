using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.Utils;

public class Position
{
    public static Position Zero = new(0, 0);
    public long X { get; }
    public long Y { get; }

    public Position(long y, long x)
    {
        X = x;
        Y = y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && other.X == X && other.Y == Y;
    }

    public static Position operator +(Position p, Vector vector)
    {
        return new Position(p.Y + vector.dY, p.X + vector.dX);
    }

    public static bool operator ==(Position p, Position p2)
    {
        return p.Equals(p2);
    }

    public static bool operator !=(Position p, Position p2)
    {
        return !(p == p2);
    }

    public static Position operator -(Position p, Position p2)
    {
        return new Position(p.Y - p2.Y, p.X - p2.X);
    }

    public long ManhattanDistance()
    {
        return Math.Abs(X) + Math.Abs(Y);
    }

    public long ManhattanDistance(Position other)
    {
        return (this - other).ManhattanDistance();
    }

    public IEnumerable<Position> Orthogonals()
    {
        yield return North;
        yield return South;
        yield return East;
        yield return West;
    }

    public Position North => this + Vector.North;
    public Position South => this + Vector.South;
    public Position East => this + Vector.East;
    public Position West => this + Vector.West;

    public override string ToString()
    {
        return $"({Y},{X})";
    }

    public bool OrthoganallyAdjacent(Position other)
    {
        return ManhattanDistance(other) <= 1;
    }

    public bool OrthoganallyOrDiagonallyAdjacent(Position other)
    {
        var delta = this - other;
        return Math.Abs(delta.X) < 2 && Math.Abs(delta.Y) < 2;
    }

    public bool DiagonallyAdjacent(Position other)
    {
        return OrthoganallyOrDiagonallyAdjacent(other) && !OrthoganallyAdjacent(other);
    }

    public IEnumerable<Position> OrthoganalNeighbors() {
        yield return this + Vector.North;
        yield return this + Vector.East;
        yield return this + Vector.South;
        yield return this + Vector.West;
    }

    public IEnumerable<Position> DiagonalNeighbors() {
        yield return this + Vector.North + Vector.East;
        yield return this + Vector.North + Vector.West;
        yield return this + Vector.South + Vector.East;
        yield return this + Vector.South + Vector.West;
    }

    public IEnumerable<Position> DiagonalAndOrthoganalNeighbors() => OrthoganalNeighbors().Concat(DiagonalNeighbors());

    public Vector Unit => new Vector(Math.Sign(Y), Math.Sign(X));
}

public static class PositionExtensions
{
    public static bool TryLookup<T>(this Position self, List<List<T>> array, out T value)
    {
        if (self.Y >= 0 && self.Y < array.Count)
        {
            if (self.X >= 0 && self.X < array[(int)self.Y].Count)
            {
                value = array[(int)self.Y][(int)self.X];
                return true;
            }
        }
        value = default!;
        return false;
    }

    public static T Lookup<T>(this Position self, List<List<T>> array)
    {
        return array[(int)self.Y][(int)self.X];
    }
}