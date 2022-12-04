using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day04;

[UsedImplicitly]
public class Day04 : AdventOfCode<List<ElfPair>>
{
    public override List<ElfPair> Parse(string input) => input
        .Lines()
        .Select(line => {
            var temp = line.Split(",");
            var t1 = temp[0].Split("-");
            var t2 = temp[1].Split("-");
            return new ElfPair(
                new(Convert.ToInt64(t1[0]), Convert.ToInt64(t1[1])),
                new(Convert.ToInt64(t2[0]), Convert.ToInt64(t2[1]))
            );
        })
        .ToList();

    [TestCase(Input.Example, 2)]
    [TestCase(Input.File, 433)]
    public override long Part1(List<ElfPair> input)
    {
        return input.Where(pair =>
        {
            if (pair.Elf1.Last <= pair.Elf2.Last && pair.Elf1.First >= pair.Elf2.First)
                return true;
            if (pair.Elf2.Last <= pair.Elf1.Last && pair.Elf2.First >= pair.Elf1.First)
                return true;
            return false;
        })
        .Count();
    }

    [TestCase(Input.Example, 4)]
    [TestCase(Input.File, 852)]
    public override long Part2(List<ElfPair> input)
    {
        return input.Where(pair =>
        {
            return pair.Elf1.First.IsInRange(pair.Elf2.First, pair.Elf2.Last) ||
                pair.Elf1.Last.IsInRange(pair.Elf2.First, pair.Elf2.Last) ||
                pair.Elf2.First.IsInRange(pair.Elf1.First, pair.Elf1.Last) ||
                pair.Elf2.Last.IsInRange(pair.Elf1.First, pair.Elf1.Last);
        })
        .Count();
    }
}

public record SectionAssignment(long First, long Last);
public record ElfPair(SectionAssignment Elf1, SectionAssignment Elf2);