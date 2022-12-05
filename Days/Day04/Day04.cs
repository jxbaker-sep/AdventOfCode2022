using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day04;

[UsedImplicitly]
public class Day04 : AdventOfCode<long, List<ElfPair>>
{
    public override List<ElfPair> Parse(string input) => input.Lines().Parse<ElfPair>();

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

public record SectionAssignment(long First, [Format(Before="-")]long Last);
public record ElfPair(SectionAssignment Elf1, [Format(Before = ",")]SectionAssignment Elf2);