using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day13;

[UsedImplicitly]
public class Day13 : AdventOfCode<long, List<(Day13Item, Day13Item)>>
{
    public override List<(Day13Item, Day13Item)> Parse(string input)
    {
        return input.Paragraphs()
            .Select(p => (p[0].Parse<Day13Item>(), p[1].Parse<Day13Item>()))
            .ToList();
    }

    [TestCase(Input.Example, 13)]
    [TestCase(Input.File, 4821)]
    public override long Part1(List<(Day13Item, Day13Item)> input)
    {
        var x = input.WithIndices().Where(i => ComparePackets(i.Value.Item1, i.Value.Item2) < 0).ToList();
        return x.Sum(it => it.Index + 1);
    }


    [TestCase(Input.Example, 140)]
    [TestCase(Input.File, 21890)]
    public override long Part2(List<(Day13Item, Day13Item)> input)
    {
        var sentinel1 = "[[2]]".Parse<Day13Item>();
        var sentinel2 = "[[6]]".Parse<Day13Item>();
        var all = input.SelectMany(it => new[]{it.Item1, it.Item2})
            .Append(sentinel1)
            .Append(sentinel2)
            .ToList();

        all.Sort((a, b) => ComparePackets(a, b));

        return all.WithIndices()
            .Where(it => ComparePackets(it.Value, sentinel1) == 0 || ComparePackets(it.Value, sentinel2) == 0)
            .Take(2)
            .Select(it => it.Index + 1)
            .Product();
    }

    private int CompareLists(List<Day13Item> item1, List<Day13Item> item2)
    {
        foreach(var (a, b) in item1.Zip(item2))
        {
            var result = ComparePackets(a, b);
            if (result != 0) return result;
        }

        return (item1.Count - item2.Count) switch
        {
            < 0 => -1,
            0 => 0,
            > 0 => 1
        };
    }

    private int ComparePackets(Day13Item a, Day13Item b)
    {
        if (a.Value is {} a1 && b.Value is {} b1)
        {
            return (a1 - b1) switch
            {
                < 0 => -1,
                0 => 0,
                > 0 => 1
            };
        }
        return CompareLists(ConvertToList(a), ConvertToList(b));
    }

    private List<Day13Item> ConvertToList(Day13Item a)
    {
        if (a.Value is {}) return new(){a};
        return a.List!;
    }
}

public record Day13Item(
    [Alternate] long? Value,
    [Alternate, Format(Before = "[", After = "]", Separator = ",")] List<Day13Item>? List
);
