﻿namespace AdventOfCode2022.Utils
{
    public static class LMath
    {
        public static long Sign(long a) => a switch
        {
            < 0 => -1,
            > 0 => 1,
            _ => 0
        };
        public static long Abs(long a) => a < 0 ? -a : a;

        public static long Triangle(long value)
        {
            if (value <= 0) return 0;
            return value * (value + 1) / 2;
        }

        public static bool IsInRange(this long item, long first, long last)
        {
            return item >= first && item <= last;
        }
    }
}