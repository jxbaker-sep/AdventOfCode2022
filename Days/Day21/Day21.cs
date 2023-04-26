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
            var m = Regex.Match(line, @"(?<name>\w+): ((?<value>\d+)|((?<m1>\w+) (?<operand>[+*/-]) (?<m2>\w+)))");
            return new Monkey(m.StringGroup("name"), m.OptionalLongGroup("value"), m.OptionalStringGroup("m1"), m.OptionalStringGroup("operand"), m.OptionalStringGroup("m2"));
        }
    ).ToList();

    [TestCase(Input.Example, 152)]
    [TestCase(Input.File, 152479825094094)]
    public override long Part1(IReadOnlyList<Monkey> monkeys)
    {
        var dict = monkeys.ToDictionary(m => m.Name, m => m);
        return EvaluateMonkey(dict, "root");
    }

    [TestCase(Input.Example, 301)]
    [TestCase(Input.File, 3360561285172)]
    public override long Part2(IReadOnlyList<Monkey> monkeys)
    {
        var dict = monkeys.ToDictionary(m => m.Name, m => m);
        if (IsConstantPath(dict, dict["root"].LeftMonkey!))
        {
            return ReverseEvaluateMonkey(dict, dict["root"].RightMonkey!, EvaluateMonkey(dict, dict["root"].LeftMonkey!));
        }
        return ReverseEvaluateMonkey(dict, dict["root"].LeftMonkey!, EvaluateMonkey(dict, dict["root"].RightMonkey!));
    }

    private long EvaluateMonkey(IReadOnlyDictionary<string, Monkey> dict, string v)
    {
        var m = dict[v];
        if (m.Value is {} value) return value;
        var m1 = EvaluateMonkey(dict, m.LeftMonkey!);
        var m2 = EvaluateMonkey(dict, m.RightMonkey!);
        return m.Operand switch {
            "+" => m1 + m2,
            "-" => m1 - m2,
            "*" => m1 * m2,
            "/" => m1 / m2,
            _ => throw new ApplicationException()
        };
    }

    private long ReverseEvaluateMonkey(IReadOnlyDictionary<string, Monkey> dict, string v, long rhs)
    {
        if (v == "humn") return rhs;
        var m = dict[v];
        if (m.Value is {} value) return value;
        if (IsConstantPath(dict, m.LeftMonkey!))
        {
            var constant = EvaluateMonkey(dict, m.LeftMonkey!);
            var newRhs = m.Operand switch {
                "+" => rhs - constant,
                "-" => constant - rhs,
                "*" => rhs / constant,
                "/" => constant / rhs,
                _ => throw new ApplicationException()
            };
            return ReverseEvaluateMonkey(dict, m.RightMonkey!, newRhs);
        }
        else 
        {
            var constant = EvaluateMonkey(dict, m.RightMonkey!);
            var newRhs = m.Operand switch {
                "+" => rhs - constant,
                "-" => rhs + constant,
                "*" => rhs / constant,
                "/" => rhs * constant,
                _ => throw new ApplicationException()
            };
            return ReverseEvaluateMonkey(dict, m.LeftMonkey!, newRhs);
        }
    }

    private bool IsConstantPath(IReadOnlyDictionary<string, Monkey> dict, string v)
    {
        if (v == "humn") return false;
        var m = dict[v];
        if (m.LeftMonkey!.Length > 0)
        {
            return IsConstantPath(dict, m.LeftMonkey) && IsConstantPath(dict, m.RightMonkey!);
        }
        return true;
    }
}

public record Monkey(string Name, long? Value, string? LeftMonkey, string? Operand, string? RightMonkey);
