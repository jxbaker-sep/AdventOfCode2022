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

        public Vector RotateRight() => new(dX, -dY);
        public Vector RotateLeft() => new(-dX, dY);

        public Vector Unit => new Vector(LMath.Sign(dY), LMath.Sign(dX));
        public long Magnitude => LMath.Abs(dX) + LMath.Abs(dY);
    }
}