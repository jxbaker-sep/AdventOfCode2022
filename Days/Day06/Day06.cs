using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day06;

[UsedImplicitly]
public class Day06 : AdventOfCode<long, string>
{
    public override string Parse(string input) => input;


    [TestCase(Input.Raw, 7, Raw = "mjqjpqmgbljsphdztnvjfqwrcgsmlb")]
    [TestCase(Input.Raw, 5, Raw = "bvwbjplbgvbhsrlpgdmjqwftvncz")]
    [TestCase(Input.Raw, 6, Raw = "nppdvjthqldpwncqszvftbrmjlhg")]
    [TestCase(Input.Raw, 10, Raw = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg")]
    [TestCase(Input.Raw, 11, Raw = "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw")]
    [TestCase(Input.File, 1198)]
    public override long Part1(string input)
    {
        return input.Windows(4).WithIndices()
            .First(window => new HashSet<char>(window.Value).Count == 4).Index + 4;
    }

    [TestCase(Input.Raw, 19, Raw = "mjqjpqmgbljsphdztnvjfqwrcgsmlb")]
    [TestCase(Input.Raw, 23, Raw = "bvwbjplbgvbhsrlpgdmjqwftvncz")]
    [TestCase(Input.Raw, 23, Raw = "nppdvjthqldpwncqszvftbrmjlhg")]
    [TestCase(Input.Raw, 29, Raw = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg")]
    [TestCase(Input.Raw, 26, Raw = "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw")]
    [TestCase(Input.File, 3120)]
    public override long Part2(string input)
    {
        return input.Windows(14).WithIndices()
            .First(window => new HashSet<char>(window.Value).Count == 14).Index + 14;
    }
}
