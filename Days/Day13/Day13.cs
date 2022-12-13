using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day13;

[UsedImplicitly]
public class Day13 : AdventOfCode<long, List<(Day13List, Day13List)>>
{
    public override List<(Day13List, Day13List)> Parse(string input)
    {
        return input.Paragraphs()
            .Select(p => (p[0].Parse<Day13List>(), p[1].Parse<Day13List>()))
            .ToList();
    }

    [TestCase(Input.Example, 13)]
    [TestCase(Input.File, 4821)]
    public override long Part1(List<(Day13List, Day13List)> input)
    {
        var x = input.WithIndices().Where(i => CompareLists(i.Value.Item1, i.Value.Item2) < 0).ToList();
        return x.Sum(it => it.Index + 1);
    }


    [TestCase(Input.Example, 140)]
    [TestCase(Input.File, 21890)]
    public override long Part2(List<(Day13List, Day13List)> input)
    {
        var sentinel1 = "[[2]]".Parse<Day13List>();
        var sentinel2 = "[[6]]".Parse<Day13List>();
        var all = input.SelectMany(it => new[]{it.Item1, it.Item2})
            .Append(sentinel1)
            .Append(sentinel2)
            .ToList();

        all.Sort((a, b) => CompareLists(a, b));

        return all.WithIndices()
            .Where(it => CompareLists(it.Value, sentinel1) == 0 || CompareLists(it.Value, sentinel2) == 0)
            .Take(2)
            .Select(it => it.Index + 1)
            .Product();
    }

    private int CompareLists(Day13List item1, Day13List item2)
    {
        foreach(var (a, b) in item1.Items.Zip(item2.Items))
        {
            var result = ComparePackets(a, b);
            if (result != 0) return result;
        }

        return (item1.Items.Count - item2.Items.Count) switch
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

    private Day13List ConvertToList(Day13Item a)
    {
        if (a.Value is {}) return new(new(){a});
        return a.List!;
    }
}

public record Day13Item(
    [Alternate] long? Value,
    [Alternate] Day13List? List
);

public record Day13List(
    [Format(Before = "[", After = "]", Separator = ",")]
    List<Day13Item> Items
);