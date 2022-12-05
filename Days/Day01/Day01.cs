using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day01;

[UsedImplicitly]
public class Day01 : AdventOfCode<long,List<List<long>>>
{
    public override List<List<long>> Parse(string input) => 
        input
            .Paragraphs()
            .Select(group => group.Select(line => Convert.ToInt64(line)).ToList()).ToList();

    [TestCase(Input.Example, 24000)]
    [TestCase(Input.File, 66616)]
    public override long Part1(List<List<long>> input)
    {
        return input.Select(group=> group.Sum()).Max();
    }

    [TestCase(Input.Example, 45000)]
    [TestCase(Input.File, 199172)]
    public override long Part2(List<List<long>> input)
    {
        return input.Select(group=> group.Sum())
            .OrderByDescending(it => it)
            .Take(3)
            .Sum();
    }
}