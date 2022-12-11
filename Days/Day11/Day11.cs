using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day11;

[UsedImplicitly]
public class Day11 : AdventOfCode<long, IReadOnlyList<Monkey>>
{
    public override IReadOnlyList<Monkey> Parse(string input)
    {
        var paragraphs = input.Paragraphs();
        var result = new List<Monkey>();
        foreach (var p in paragraphs)
        {
            var lines = p.Parse<Day11Input>();
            result.Add(new(
                lines[0].MonkeyNumber ?? throw new ApplicationException(),
                lines[1].Items ?? throw new ApplicationException(),
                lines[2].Operation ?? throw new ApplicationException(),
                lines[3].Test ?? throw new ApplicationException(),
                lines[4].IfTrue ?? throw new ApplicationException(),
                lines[5].IfFalse ?? throw new ApplicationException()
            ));
        }
        
        return result;
    }

    [TestCase(Input.Example, 10605)]
    [TestCase(Input.File, 119715)]
    public override long Part1(IReadOnlyList<Monkey> input)
    {
        input = input.Select(monkey => monkey with { Items = monkey.Items.ToList() }).ToList();
        var monkeyBusiness = Enumerable.Repeat(0, input.Count).ToList();

        foreach(var _ in Enumerable.Range(0, 20))
        {
            foreach(var (monkey, index) in input.WithIndices())
            {
                monkeyBusiness[index] += monkey.Items.Count;
                foreach(var item in monkey.Items)
                {
                    var wl = ApplyWorry(monkey.Operation, item) / 3;
                    if (wl % monkey.Test == 0) input[monkey.IfTrue].Items.Add(wl);
                    else input[monkey.IfFalse].Items.Add(wl);
                }
                monkey.Items.Clear();
            }
        }

        return monkeyBusiness
            .OrderByDescending(z => z)
            .Take(2)
            .Aggregate((a,c) => a * c);
    }

    [TestCase(Input.Example, 2713310158L)]
    [TestCase(Input.File, 18085004878L)]
    public override long Part2(IReadOnlyList<Monkey> input)
    {
        input = input.Select(monkey => monkey with { Items = monkey.Items.ToList() }).ToList();
        var monkeyBusiness = Enumerable.Repeat(0L, input.Count).ToList();
        var divisor = input.Aggregate(1L, (accum, monkey) => {
            return checked(accum * monkey.Test);
        });

        foreach(var _ in Enumerable.Range(0, 10000))
        {
            foreach(var (monkey, index) in input.WithIndices())
            {
                monkeyBusiness[index] += monkey.Items.Count;
                foreach(var item in monkey.Items)
                {
                    var wl = ApplyWorry(monkey.Operation, item) % divisor;
                    if (wl % monkey.Test == 0) input[monkey.IfTrue].Items.Add(wl);
                    else input[monkey.IfFalse].Items.Add(wl);
                }
                monkey.Items.Clear();
            }
        }

        return monkeyBusiness
            .OrderByDescending(z => z)
            .Take(2)
            .Aggregate((a,c) => a * c);
    }

    private long ApplyWorry(string operation, long item)
    {
        Func<long, long, long> op = operation.StartsWith("*") ? ((a, b) => checked(a * b)) : ((a, b) => checked(a + b));
        if (operation.Contains("old"))
        {
            return op(item, item);
        }
        return op(item, Convert.ToInt64(operation.Substring(1)));
    }
}

public record Day11Input(
    [Alternate, Format(Before = "Monkey", After = ":")] long? MonkeyNumber,
    [Alternate, Format(Before = "Starting items:", Separator = ",")] List<long>? Items,
    [Alternate, Format(Before = "Operation: new = old", Regex = ".*")] string? Operation,
    [Alternate, Format(Before = "Test: divisible by")] long? Test,
    [Alternate, Format(Before = "If true: throw to monkey")] int? IfTrue,
    [Alternate, Format(Before = "If false: throw to monkey")] int? IfFalse
);

public record Monkey(
    long Number,
    List<long> Items,
    string Operation,
    long Test,
    int IfTrue,
    int IfFalse
);