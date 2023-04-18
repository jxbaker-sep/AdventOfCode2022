using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day21;

[UsedImplicitly]
public class Day21 : AdventOfCode<long, IReadOnlyList<Monkey>>
{
    public override IReadOnlyList<Monkey> Parse(string input) => input.Lines().Select(
        line => {
            var m = Regex.Match(line, @"(?<name>\w+): ((?<value>\d+)|((?<m1>\w+) (?<operand>[+-*/]) (?<m2>\w+)))");
            return new Monkey(m.StringGroup("name"), m.OptionalLongGroup("value"), m.OptionalStringGroup("m1"), m.OptionalStringGroup("operand"), m.OptionalStringGroup("m2"));
        }
    ).ToList();

    [TestCase(Input.Example, 152)]
    [TestCase(Input.File, 0)]
    public override long Part1(IReadOnlyList<Monkey> monkeys)
    {
        var dict = monkeys.ToDictionary(m => m.Name, m => m);
        return EvaluateMonkey(dict, "root");
    }

    public override long Part2(IReadOnlyList<Monkey> monkeys)
    {
        return 0;
    }

    private long EvaluateMonkey(IReadOnlyDictionary<string, Monkey> dict, string v)
    {
        var m = dict[v];
        if (m.OtherMonkey1.Length > 0)
        {
            var m1 = EvaluateMonkey(dict, m.OtherMonkey1);
            var m2 = EvaluateMonkey(dict, m.OtherMonkey2);
            return m.Operand switch {
                "+" => m1 + m2,
                "-" => m1 - m2,
                "*" => m1 * m2,
                "/" => m1 / m2,
                _ => throw new ApplicationException()
            };
        }
        else return m.Value;
    }
}

public record Monkey(string Name, long Value, string OtherMonkey1, string Operand, string OtherMonkey2);
