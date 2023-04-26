using System;

namespace AdventOfCode2022.Utils
{
    public class Vector
    {
        public static readonly Vector Zero = new(0, 0);
        public static readonly Vector North = new(-1, 0);
        public static readonly Vector East = new(0, 1);
        public static readonly Vector South = new(1, 0);
        public static readonly Vector West = new(0, -1);

        public long dX { get; }
        public long dY { get; }

        public Vector(long dy, long dx)
        {
            dX = dx;
            dY = dy;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.dY + b.dY, a.dX + b.dX);
        }

        public static Vector operator *(Vector a, long magnitude)
        {
            return new Vector(a.dY * magnitude, a.dX * magnitude);
        }

        public static bool operator==(Vector a, Vector b)
        {
            return a.dX == b.dX && a.dY == b.dY;
        }

        public static bool operator!=(Vector a, Vector b) => !(a == b);

        public override bool Equals(object? obj)
        {
            return obj is Vector v2 && this == v2;
        }

        public Vector RotateRight() => new(dX, -dY);
        public Vector RotateLeft() => new(-dX, dY);
        public Vector Flip() => new(-dY, -dX);

        public Vector Unit => new Vector(LMath.Sign(dY), LMath.Sign(dX));
        public long Magnitude => LMath.Abs(dX) + LMath.Abs(dY);

        public override int GetHashCode() => HashCode.Combine(dX, dY);

        public override string ToString()
    {
        return $"({dY},{dX})";
    }
    }
}