using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day03;

[UsedImplicitly]
public class Day03 : AdventOfCode<long,List<string>>
{
    public override List<string> Parse(string input) => input
        .Lines();

    [TestCase(Input.Example, 157)]
    [TestCase(Input.File, 8085)]
    public override long Part1(List<string> input)
    {
        return input
            .Select(line => Priority(
                line.Substring(0, line.Length / 2)
                    .Intersect(line.Substring(line.Length / 2))
                    .Single()
            ))
            .Sum();
    }

    [TestCase(Input.Example, 70)]
    [TestCase(Input.File, 2515)]
    public override long Part2(List<string> input)
    {
        return input.InGroupsOf(3)
            .Select(group => Priority(
                group[0].Intersect(group[1])
                    .Intersect(group[2])
                    .Single()
            ))
            .Sum();
    }

    private long Priority(char v)
    {
        if (v is >= 'a' and <= 'z') return v - 'a' + 1;
        return v - 'A' + 27;
    }
}
