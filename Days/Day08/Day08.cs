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
                if (Enumerable.Range(0, rowindex)
                    .All(n => input[n][colindex] < col)) {count += 1; continue;}
                if (Enumerable.Range(rowindex + 1, input.Count - rowindex - 1)
                    .All(s => input[s][colindex] < col)) {count += 1; continue;}
                if (Enumerable.Range(0, colindex)
                    .All(w => input[rowindex][w] < col)) {count += 1; continue;}
                if (Enumerable.Range(colindex + 1, input[0].Count - colindex - 1)
                    .All(e => input[rowindex][e] < col)) {count += 1; continue;}
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
                var n = Enumerable.Range(0, rowindex)
                    .Select(n => input[n][colindex])
                    .Reverse()
                    .TakeWhilePlusOne(n => n < col)
                    .Count();
                var s = Enumerable.Range(rowindex + 1, input.Count - rowindex - 1)
                    .Select(s => input[s][colindex])
                    .TakeWhilePlusOne(s => s < col)
                    .Count();
                var w = Enumerable.Range(0, colindex)
                    .Select(w => input[rowindex][w])
                    .Reverse()
                    .TakeWhilePlusOne(w => w < col)
                    .Count();
                var e = Enumerable.Range(colindex + 1, input[0].Count - colindex - 1)
                    .Select(e => input[rowindex][e])
                    .TakeWhilePlusOne(e => e < col)
                    .Count();
                if (max < n*s*e*w) max = n*e*s*w;
            }
        }

        return max;
    }
}

