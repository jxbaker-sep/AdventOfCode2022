using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day08;

[UsedImplicitly]
public class Day08 : AdventOfCode<long, List<List<long>>>
{
    public override List<List<long>> Parse(string input) => input.Lines()
        .Select(line => line.Select(c => Convert.ToInt64($"{c}")).ToList())
        .ToList();


    [TestCase(Input.Example, 21)]
    [TestCase(Input.File, 1823)]
    public override long Part1(List<List<long>> input)
    {
        var count = 0;
        foreach(var (row, rowindex) in input.WithIndices())
        {
            foreach(var (col, colindex) in row.WithIndices())
            {
                if (input.Take(rowindex).Select(row => row[colindex])
                    .All(n => n < col)) {count += 1; continue;}
                if (input.Skip(rowindex+1).Select(row => row[colindex])
                    .All(s => s < col)) {count += 1; continue;}
                if (input[rowindex].Take(colindex)
                    .All(w => w < col)) {count += 1; continue;}
                if (input[rowindex].Skip(colindex+1)
                    .All(e => e < col)) {count += 1; continue;}
            }
        }

        return count;
    }

    [TestCase(Input.Example, 8)]
    [TestCase(Input.File, 211680)]
    public override long Part2(List<List<long>> input)
    {
        var max = 0;
        foreach(var (row, rowindex) in input.WithIndices())
        {
            foreach(var (col, colindex) in row.WithIndices())
            {
                var n = input.Take(rowindex).Select(row => row[colindex])
                    .Reverse()
                    .TakeWhilePlusOne(n => n < col)
                    .Count();
                var s = input.Skip(rowindex+1).Select(row => row[colindex])
                    .TakeWhilePlusOne(s => s < col)
                    .Count();
                var w = input[rowindex].Take(colindex)
                    .Reverse()
                    .TakeWhilePlusOne(w => w < col)
                    .Count();
                var e = input[rowindex].Skip(colindex+1)
                    .TakeWhilePlusOne(e => e < col)
                    .Count();
                if (max < n*s*e*w) max = n*e*s*w;
            }
        }

        return max;
    }
}

