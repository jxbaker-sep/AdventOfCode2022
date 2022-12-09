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

        public int dX { get; }
        public int dY { get; }

        public Vector(int dy, int dx)
        {
            dX = dx;
            dY = dy;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.dY + b.dY, a.dX + b.dX);
        }

        public static Vector operator *(Vector a, int magnitude)
        {
            return new Vector(a.dY * magnitude, a.dX * magnitude);
        }

        public Vector RotateRight() => new(dX, -dY);
        public Vector RotateLeft() => new(-dX, dY);

        public int Magnitude => Math.Abs(dX) + Math.Abs(dY);

        public Vector Unit => new Vector(Math.Sign(dY), Math.Sign(dX));
    }
}