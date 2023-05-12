using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day23;

[UsedImplicitly]
public class Day25 : AdventOfCode<string, IReadOnlyList<string>>
{
    public override IReadOnlyList<string> Parse(string input) => input.Lines();

    [TestCase(Input.Example, "2=-1=0")]
    [TestCase(Input.File, "2-0=11=-0-2-1==1=-22")]
    public override string Part1(IReadOnlyList<string> snafuNumbers)
    {
        return snafuNumbers.Aggregate((accumulator, current) => SnafuAdd(accumulator, current));
    }

    public override string Part2(IReadOnlyList<string> snafuNumbers)
    {
        return "";
    }

    private string SnafuAdd(string lhs1, string rhs1)
    {
        var lhs = lhs1.Reverse().ToList();
        var rhs = rhs1.Reverse().ToList();
        var carry = 0;
        var current = "";
        var position = 0;
        while (position < lhs.Count || position < rhs.Count || carry != 0)
        {
            var left = SnafuDigitToDecimal(position < lhs.Count ? lhs[position] : '0');
            var right = SnafuDigitToDecimal(position < rhs.Count ? rhs[position] : '0');
            position += 1;
            var x = left + right + carry;
            var x2 = x;
            if (x2 < -2)
            {
                x2 = x2 + 5;
                carry = -1;
            }
            else if (x2 > 2)
            {
                x2 = x2 - 5;
                carry = 1;
            }
            else {
                carry = 0;
            }
            current += x2 switch {2 => '2', 1 => '1', 0 => '0', -1 => '-', -2 => '=', _ => throw new ApplicationException()};
        }
        return current.Reverse().Join();
    }

    private long SnafuDigitToDecimal(char snafuDigit)
    {
        return snafuDigit switch {
            '2' => 2,
            '1' => 1,
            '0' => 0,
            '-' => -1,
            '=' => -2,
            _ => throw new ApplicationException()
        };
    }
}

